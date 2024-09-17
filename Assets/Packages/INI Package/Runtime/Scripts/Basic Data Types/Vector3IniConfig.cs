/** 
  * @file Vector3IniConfig.cs
  * @brief Configurable Vector3 value that can be saved to an .ini file
  * 
  * @author Logan Bender
  * 
  * @copyright Universal Creative. All rights reserved.
**/

using UnityEngine;

namespace CreativeAti.Config
{
    [CreateAssetMenu(fileName = "Vector3IniConfig", menuName = "Scriptable Object/INI Config/Basic Types/Vector3 INI Config")]
    public class Vector3IniConfig : GenericIniConfig<Vector3>
    {
        public override void InitializeFromIniData(System.Collections.Generic.Dictionary<string, string> data)
        {
            string[] values = data[propertyName].Split(',');
            Value = new Vector3(float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2]));
        }
    }
}