/** 
  * @file DoubleIniConfig.cs
  * @brief Configurable double value that can be saved to an .ini file
  * 
  * @author Logan Bender
  * 
  * @copyright Universal Creative. All rights reserved.
**/

using UnityEngine;

namespace CreativeAti.Config
{
    [CreateAssetMenu(fileName = "DoubleIniConfig", menuName = "Scriptable Object/INI Config/Basic Types/Double INI Config")]
    public class DoubleIniConfig : GenericIniConfig<double>
    {
        public override void InitializeFromIniData(System.Collections.Generic.Dictionary<string, string> data)
        {
            Value = double.Parse(data[propertyName]);
        }
    }
}