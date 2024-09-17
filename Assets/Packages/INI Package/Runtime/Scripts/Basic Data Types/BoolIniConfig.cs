/** 
  * @file BoolIniConfig.cs
  * @brief Configurable bool value that can be saved to an .ini file
  * 
  * @author Logan Bender
  * 
  * @copyright Universal Creative. All rights reserved.
**/

using UnityEngine;

namespace CreativeAti.Config
{
    [CreateAssetMenu(fileName = "BoolIniConfig", menuName = "Scriptable Object/INI Config/Basic Types/Bool INI Config")]
    public class BoolIniConfig : GenericIniConfig<bool>
    {
        public override void InitializeFromIniData(System.Collections.Generic.Dictionary<string, string> data)
        {
            Value = bool.Parse(data[propertyName]);
        }
    }
}