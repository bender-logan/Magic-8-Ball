/** 
  * @file StringIniConfig.cs
  * @brief Configurable string value that can be saved to an .ini file
  * 
  * @author Logan Bender
  * 
  * @copyright Universal Creative. All rights reserved.
**/

using UnityEngine;

namespace CreativeAti.Config
{
    [CreateAssetMenu(fileName = "StringIniConfig", menuName = "Scriptable Object/INI Config/Basic Types/String INI Config")]
    public class StringIniConfig : GenericIniConfig<string>
    {
        public override void InitializeFromIniData(System.Collections.Generic.Dictionary<string, string> data)
        {
            Value = data[propertyName];
        }
    }
}