/** 
  * @file FloatIniConfig.cs
  * @brief Configurable float value that can be saved to an .ini file
  * 
  * @author Logan Bender
  * 
  * @copyright Universal Creative. All rights reserved.
**/

using UnityEngine;

namespace CreativeAti.Config
{
    [CreateAssetMenu(fileName = "FloatIniConfig", menuName = "Scriptable Object/INI Config/Basic Types/Float INI Config")]
    public class FloatIniConfig : GenericIniConfig<float>
    {
        public override void InitializeFromIniData(System.Collections.Generic.Dictionary<string, string> data)
        {
            Value = float.Parse(data[propertyName]);
        }
    }
}