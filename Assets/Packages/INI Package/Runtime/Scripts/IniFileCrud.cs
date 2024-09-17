/** 
  * @file IniFileCrud.cs
  * @brief Class for creating and reading .ini files
  * 
  * @author Logan Bender
  * 
  * @copyright Universal Creative. All rights reserved.
**/

using System.Collections.Generic;
using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
#endif

namespace CreativeAti.Config
{
    public static class IniFileCrud
    {
        private static string destinationPathForConfigAssetFromStreamingAssetsFolder = "Config/CreativeATI";
        private static bool debugOn = true;
        public static bool DebugOn { get => debugOn; }

        private static Dictionary <string, IniFile> _cachedIniFiles = new Dictionary<string, IniFile>();

#if UNITY_EDITOR
        /// <summary>
        /// Creates all INI Config files from ini scriptable objects in the asset folder
        /// </summary>
        /// <param listName="target"></param>
        /// <param listName="pathToBuiltProject"></param>
        [PostProcessBuildAttribute(1)]
        public static void CreateIniFiles(BuildTarget target, string pathToBuiltProject)
        {
        // CREATE DIRECTORIES FOR INI FILES

            // get build path
            FileInfo buildPath = new FileInfo(pathToBuiltProject);
            string buildName = buildPath.Name.Replace(buildPath.Extension, "");
            DirectoryInfo buildDirectory = buildPath.Directory;

            // get data directory
            string dataDirectory = Path.Combine(buildDirectory.FullName, buildName + "_Data");

            // if the data directory doesn't exist, check for the vs debug data directory
            if (Directory.Exists(dataDirectory) == false)
            {
                string vsDebugDataDirectory = Path.Combine(buildDirectory.FullName, "build/bin/" + buildName + "_Data");
                if (Directory.Exists(vsDebugDataDirectory) == false)
                    Debug.LogError("[VR]: Could not find data directory at [" + dataDirectory + "]. Also checked vs debug at: [" + vsDebugDataDirectory + "].");
                else
                    dataDirectory = vsDebugDataDirectory;
            }

            // find or create the streaming assets folder
            string streamingAssetsPath = Path.Combine(dataDirectory, "StreamingAssets");
            if (Directory.Exists(streamingAssetsPath) == false)
                Directory.CreateDirectory(streamingAssetsPath);

            // create the destination path for the config asset
            string[] splitPath = destinationPathForConfigAssetFromStreamingAssetsFolder.Split('/');
            // foreach (string path in splitPath), create the directory if it doesn't exist
            for (int i = 0; i < splitPath.Length; i++)
            {
                streamingAssetsPath = Path.Combine(streamingAssetsPath, splitPath[i]);
                if (Directory.Exists(streamingAssetsPath) == false)
                    Directory.CreateDirectory(streamingAssetsPath);
            }

            if (debugOn) Debug.Log($"[Config IO]: Streaming assets path : [{streamingAssetsPath}] ");

        // CREATE INI DATA OBJECTS

            // get all ini scriptable object assets from asset folder
            string[] guids = AssetDatabase.FindAssets("t:IniScriptableObject", new[] { "Assets" });
            if (debugOn) Debug.Log($"[Config IO]: Found {guids.Length} ini assets");

            // if we don't have any ini scriptable object assets, log a warning and return
            if (guids.Length <= 0)
            {
                Debug.LogWarning("[Config IO]: No ini scriptable object assets found in asset folder.");
                return;
            }

            // create a list of ini files to write to
            List<IniFile> iniFiles = new List<IniFile>();
    
            // for each ini scriptable object asset, create a new ini file or add to an existing one
            for (int i = 0; i < guids.Length; i++)
            {
                // load the ini scriptable object asset
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                IniScriptableObject iniAsset = AssetDatabase.LoadAssetAtPath<IniScriptableObject>(assetPath);

                // find ini file in the list with a matching listName to this ini SO asset
                bool found = false;
                for (int j = 0; j < iniFiles.Count; j++)
                {
                    if (iniFiles[j].fileName == iniAsset.IniFileName)
                    {
                        found = true;
                        // write the ini data to the existing ini file
                        // iniAsset.GetIniData() is implemented in the child class
                        iniFiles[j].writeMultipleValues(iniAsset.IniSectionName, iniAsset.GetIniData());
                        continue;
                    }
                }

                // if we didn't find a matching ini file, create a new one
                if (!found)
                {
                    IniFile newIniFile = new IniFile(iniAsset.IniFileName);
                    // write the ini data to the new ini file
                    // iniAsset.GetIniData() is implemented in the child class
                    newIniFile.writeMultipleValues(iniAsset.IniSectionName, iniAsset.GetIniData());
                    iniFiles.Add(newIniFile);
                }
            }

        // WRITE INI FILES

            if (debugOn) Debug.Log($"[Config IO]: Creating {iniFiles.Count} ini files");

            foreach (IniFile iniFile in iniFiles)
            { 
                FileInfo newSettingsPath = new FileInfo(Path.Combine(streamingAssetsPath, $"{iniFile.fileName}.ini"));

                if (newSettingsPath.Exists)
                {
                    newSettingsPath.IsReadOnly = false;
                    newSettingsPath.Delete();
                }

                // Create a new ini file in the resources folder
                File.WriteAllText(newSettingsPath.FullName, iniFile.ToString());
                if (debugOn) Debug.Log($"[Config IO]: Created new ini [{newSettingsPath.FullName}] ");
                if (debugOn) Debug.Log($"[Config IO]: {newSettingsPath.FullName}\n" +
                    $"{iniFile.ToString()}");
            }
            if (debugOn) Debug.Log($"[Config IO]: Created new ini files at [{streamingAssetsPath}] ");
            

        }
#endif

        /// <summary>
        /// Read an ini file from the streaming assets folder
        /// </summary>
        /// <param listName="fileName">The listName of the ini file we want</param>
        /// <returns>An <see cref="IniFile"/> data object representing the found file. Returns null if file not found.</returns>
        public static IniFile ReadIniFile(string fileName)
        {
#if UNITY_EDITOR
            Debug.LogError("[Config IO]: ReadIniFile should not be called in editor. Ini files created after build process.");
            return null;
#else
            // get the ini file from the streaming assets folder
            FileInfo iniFileInfo = new FileInfo(Path.Combine(Application.streamingAssetsPath, destinationPathForConfigAssetFromStreamingAssetsFolder, fileName + ".ini"));

            if (iniFileInfo.Exists == false)
            {
                Debug.LogError("[Config IO]: Could not find ini file at [" + iniFileInfo.FullName + "].");
                return null;
            }

            if (_cachedIniFiles.ContainsKey(fileName))
            {
                return _cachedIniFiles[fileName];
            }

            // create a new ini file object
            IniFile iniFile = new IniFile(fileName);

            // read the lines of the ini file
            string[] lines = File.ReadAllLines(iniFileInfo.FullName);
            Section currentSection = null;
                
            // for each line in the file, determine if it's a section or a property
            foreach (string line in lines)
            {
                // if the line is a section, create a new section object
                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    // if we have a current section cached, add it to the ini file
                    if (currentSection != null)
                        iniFile.sections.Add(currentSection);
                    currentSection = new Section(line.Replace("[", "").Replace("]", ""));
                }
                // if the line is a property, create a new property object and add it to the current section
                else if (line.Contains("="))
                {
                    string[] splitLine = line.Split('=');
                    Property newProperty = new Property();
                    newProperty.name = splitLine[0];
                    newProperty.value = splitLine[1];
                    currentSection.properties.Add(newProperty);
                }
            }
            // add the last section to the ini file
            iniFile.sections.Add(currentSection);

            _cachedIniFiles.Add(fileName, iniFile);

            return iniFile;
#endif
        }

        /// <summary>
        /// update an ini file section with new values
        /// </summary>
        /// <param listName="fileName"></param>
        /// <param listName="sectionName"></param>
        /// <param listName="propertyValuePairs"></param>
        public static void UpdateIniFile(string fileName, string sectionName, Dictionary<string, string> propertyValuePairs)
        {
#if UNITY_EDITOR
            Debug.LogError("[Config IO]: UpdateIniFile should not be called in editor. Ini files created after build process.");
# else
            IniFile iniFile = ReadIniFile(fileName);
            iniFile.writeMultipleValues(sectionName, propertyValuePairs);
            FileInfo newSettingsPath = new FileInfo(Path.Combine(Application.streamingAssetsPath, destinationPathForConfigAssetFromStreamingAssetsFolder, fileName + ".ini"));
            File.WriteAllText(newSettingsPath.FullName, iniFile.ToString());
#endif
        }

        /// <summary>
        /// update an ini file property with a new value
        /// </summary>
        /// <param listName="fileName"></param>
        /// <param listName="sectionName"></param>
        /// <param listName="propertyValuePairs"></param>
        public static void UpdateIniFile(string fileName, string sectionName, string property, string name)
        {
#if UNITY_EDITOR
            Debug.LogError("[Config IO]: UpdateIniFile should not be called in editor. Ini files created after build process.");
# else
            IniFile iniFile = ReadIniFile(fileName);
            iniFile.writeValue(sectionName, property, name);
            FileInfo newSettingsPath = new FileInfo(Path.Combine(Application.streamingAssetsPath, destinationPathForConfigAssetFromStreamingAssetsFolder, fileName + ".ini"));
            File.WriteAllText(newSettingsPath.FullName, iniFile.ToString());
#endif
        }
    }

    public class IniFile
    {
        public string fileName;
        public List<Section> sections;

        public IniFile(string fileName)
        {
            this.fileName = fileName;
            sections = new List<Section>();
        }

        /// <summary>
        /// Read a value from the ini file as a string
        /// </summary>
        /// <param fileName="sectionName">Name of the sectionName the property is under</param>
        /// <param fileName="property">Name of the property</param>
        /// <returns></returns>
        public string readValue(string section, string property)
        {
            if(sections == null)
            {
                Debug.LogError($"[Config IO]: No sections found in file [{fileName}]");
                return "";
            }
            if(!sections.Exists(x => x.name == section))
            {
                Debug.LogError($"[Config IO]: No section with name [{section}] found in file [{fileName}]");
                return "";
            }
            Section targetSection = sections.Find(x => x.name == section);
            if(targetSection.properties == null)
            {
                Debug.LogError($"[Config IO]: No properties found in section [{section}] in file [{fileName}]");
                return "";
            }
            if(!targetSection.properties.Exists(x => x.name == property))
            {
                Debug.LogError($"[Config IO]: No property with name [{property}] found in section [{section}] in file [{fileName}]");
                return "";
            }
            Property targetProperty = targetSection.properties.Find(x => x.name == property);
            if(IniFileCrud.DebugOn) Debug.Log($"[Config IO]: Value of property [{property}] in section [{section}] in file [{fileName}] is [{targetProperty.value}]");
            return targetProperty.value;
        }

        /// <summary>
        /// Write a value to the ini file as a string
        /// </summary>
        /// <param fileName="sectionName">Name of the sectionName the property is under</param>
        /// <param fileName="property">Name of the property</param>
        /// <param fileName="value">Value of the property as a string</param>
        public void writeValue(string sectionName, string property, string value)
        {
            // TODO : add support for comments?
            // remove string after first semicolon in sectionName and store it in _sectionComment
            // if (sectionName.Contains(";"))
            // {
            //     _sectionComment = sectionName.Substring(sectionName.IndexOf(";"));
            //     sectionName = sectionName.Substring(0, sectionName.IndexOf(";"));
            // }

            // find or make sectionName
            if(sections == null)
                sections = new List<Section>();
            Section targetSection;
            if(!sections.Exists(x => x.name == sectionName))
                targetSection = new Section(sectionName);
            else
                targetSection = sections.Find(x => x.name == sectionName);

            // TODO : add support for comments?
            // remove string after first semicolon in property and store it in _propertyComment
            // if (property.Contains(";"))
            // {
            //     _propertyComment = property.Substring(property.IndexOf(";"));
            //     property = property.Substring(0, property.IndexOf(";"));
            // }

            // find or make property
            if(targetSection.properties == null)
                targetSection.properties = new List<Property>();
            Property newProperty;
            if(!targetSection.properties.Exists(x => x.name == property))
            {
                newProperty = new Property();
                newProperty.name = property;
                newProperty.value = value;
                targetSection.properties.Add(newProperty);
            }
            else
            {
                newProperty = targetSection.properties.Find(x => x.name == property);
                newProperty.name = property;
                newProperty.value = value;
            }
        }

        /// <summary>
        /// Write multiple values to the ini file as a string
        /// </summary>
        /// <param fileName="sectionName">Name of the sectionName the properties are under</param>
        /// <param fileName="values"></param>
        public void writeMultipleValues(string sectionName, Dictionary<string, string> values)
        {
            if(IniFileCrud.DebugOn) Debug.Log($"[Config IO]: Writing multiple values to section {sectionName} in file {fileName}");

            // find or make sectionName
            if (sections == null)
                sections = new List<Section>();
            Section targetSection;
            if (!sections.Exists(x => x.name == sectionName))
            {
                if (IniFileCrud.DebugOn) Debug.Log($"[Config IO]: No section with name {sectionName} found in file {fileName}");
                targetSection = new Section(sectionName);
                sections.Add(targetSection);
            }
            else
            {
                targetSection = sections.Find(x => x.name == sectionName);
                if (IniFileCrud.DebugOn) Debug.Log("[Config IO]: Found section with name " + sectionName);
            }

            foreach (KeyValuePair<string, string> setting in values)
            {
                // find or make property
                if (targetSection.properties == null)
                    targetSection.properties = new List<Property>();
                Property newProperty;
                if (!targetSection.properties.Exists(x => x.name == setting.Key))
                {
                    newProperty = new Property();
                    newProperty.name = setting.Key;
                    newProperty.value = setting.Value;
                    targetSection.properties.Add(newProperty);
                }
                else
                {
                    newProperty = targetSection.properties.Find(x => x.name == setting.Key);
                    newProperty.name = setting.Key;
                    newProperty.value = setting.Value;
                }

            }
        }

        public bool TryGetSection(string sectionName, ref Dictionary<string, string> data)
        {
            if (data == null)
                data = new Dictionary<string, string>();

            foreach (Section section in sections)
            {
                if(section.name == sectionName)
                {
                    foreach (Property property in section.properties)
                    {
                        data.Add(property.name, property.value);
                    }
                    return true;
                }
            }
            return false;
        }

        public override string ToString()
        {
            string output = "";
            foreach (Section section in sections)
            {
                // TODO : add support for comments?
                // if (section.comment != string.Empty)
                //     output += $"; {section.comment}\n";

                output += $"[{section.name}]\n";
                foreach (Property property in section.properties)
                {
                    // TODO : add support for comments?
                    // if (property.comment != string.Empty)
                    //     output += $"; {property.comment}\n";
                    // else
                    output += $"{property.name}={property.value}\n";
                }
                output += "\n";
            }
            return output;
        }
    }

    public class Section
    {
        public string name;
        public List<Property> properties;

        // TODO : add support for comments?
        // public string comment = string.Empty;

        public Section(string name)
        {
            this.name = name;
            this.properties = new List<Property>();
        }
    }

    public class Property
    {
        // TODO : add support for comments?
        // public string comment = string.Empty;
        public string name;
        public string value;
    }
}