/** 
  * @file GenericIniConfig.cs
  * @brief Configurable float value that can be saved to an .ini file
  * 
  * @author Logan Bender
  * 
  * @copyright Universal Creative. All rights reserved.
**/

using UnityEngine;

namespace CreativeAti.Config
{
    public abstract class GenericIniConfig<T> : IniScriptableObject
    {
        [Tooltip("Description of this scriptable object")]
        [SerializeField] protected string description = string.Empty;

        [Header("Config Value")]
        [Tooltip("The value to save to the .ini file")]
        [SerializeField] private T _value = default(T);

        protected string propertyName = $"{typeof(T).Name}_Value";

        public T Value
        {
            get => _value;
            set
            {
                _value = value;
/**
 * Removing the following code block because it allows you to change the ini file from runtime in a build
 * This is not what we want, we only want to change the ini file from the editor only or from the ini file itself
 * 
 * #if !UNITY_EDITOR
 *                 IniFileCrud.UpdateIniFile(IniFileName, IniSectionName, propertyName, _value.ToString());
 * #endif
 * 
 **/
            }
        }

        public override System.Collections.Generic.Dictionary<string, string> GetIniData()
        {
            System.Collections.Generic.Dictionary<string, string> data = new System.Collections.Generic.Dictionary<string, string>
            {
                { propertyName, Value.ToString() }
            };
            return data;
        }
    }
}