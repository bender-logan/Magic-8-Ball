/** 
  * @file IntIniConfig.cs
  * @brief Configurable int value that can be saved to an .ini file
  * 
  * @author Logan Bender
  * 
  * @copyright Universal Creative. All rights reserved.
**/

using UnityEngine;

namespace CreativeAti.Config
{
    [CreateAssetMenu(fileName = "IntIniConfig", menuName = "Scriptable Object/INI Config/Basic Types/Int INI Config")]
    public class IntIniConfig : GenericIniConfig<int>
    {
        public override void InitializeFromIniData(System.Collections.Generic.Dictionary<string, string> data)
        {
            Value = int.Parse(data[propertyName]);
        }
    }
}