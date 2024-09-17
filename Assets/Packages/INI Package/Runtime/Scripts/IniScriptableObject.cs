/** 
  * @file IniScriptableObject.cs
  * @brief Scriptable object type that can be saved to an .ini file
  * 
  * @author Logan Bender
  * 
  * @copyright Universal Creative. All rights reserved.
**/

using System;
using System.Collections.Generic;
using UnityEngine;

namespace CreativeAti.Config
{
    public enum IniFileNames
    {
        CONFIG, //default
        TESTING
    }

    // Template create asset menu decorator for child classes
    // [CreateAssetMenu(fileName = "NewConfig", menuName = "Scriptable Object/INI Config")]
    public abstract class IniScriptableObject : ScriptableObject
    {
        [Header("Config File Settings")]
        [Tooltip("The .ini file to save this scriptable object to")]
        [SerializeField] private IniFileNames iniCategory = IniFileNames.CONFIG;

        public string IniFileName
        {
            get
            {
                string enumName = Enum.GetName(typeof(IniFileNames), iniCategory);
                return char.ToUpper(enumName[0]) + ((enumName.ToLower()).Substring(1)).Replace('_', ' ');
            }
        }

        public string IniSectionName { get => name; }

        /// <summary>
        /// Abstract method to get the data from the scriptable object to be saved to the .ini file. Called when a build is created.
        /// Only populate the dictionary with data that is read at initialization.
        /// See: <seealso cref="IniFileCrud.CreateIniFiles(UnityEditor.BuildTarget, string)"/>
        /// </summary>
        /// <returns>A dictionary of property/value pairs that will be written to in the ini file</returns>
        public abstract Dictionary<string, string> GetIniData();

        /// <summary>
        /// Abstract method to initialize the scriptable object from the data in the .ini file. Called when the scriptable object is loaded.
        /// </summary>
        /// <param listName="data">A dictionary of property/value pairs read from the ini file</param>
        public abstract void InitializeFromIniData(Dictionary<string, string> data);

        private void OnEnable()
        {
#if !UNITY_EDITOR
        IniFile iniFile = IniFileCrud.ReadIniFile(IniFileName);
        Dictionary<string, string> data = new Dictionary<string, string>();

        if (iniFile.TryGetSection(IniSectionName, ref data) == false)
            Debug.LogError($"[IniScriptableObject]: Could not find section [{IniSectionName}] in file [{iniCategory}]");
        else
            InitializeFromIniData(data);
#endif
        }

        /// <summary>
        /// Save the scriptable object to the .ini file. Should only be called in a build.
        /// </summary>
        public void SaveToIniFile()
        {
#if !UNITY_EDITOR
            Debug.LogWarning("SaveToIniFile() should not be called in the editor.");
#else
            IniFileCrud.UpdateIniFile(IniFileName, IniSectionName, GetIniData());
#endif
        }
    }
}