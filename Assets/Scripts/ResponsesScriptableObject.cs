/** 
  * @file ResponsesScriptableObject.cs
  * @brief holds info on responses
  * 
  * @author Logan Bender
**/

using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ResponsesConfig", menuName = "Scriptable Object/INI Config/Responses")]
public class ResponsesScriptableObject : ScriptableObject
{
    [Serializable]
    public class Response
    {
        public string upperLineResponseText;
        public string lowerLineResponseText;
        public Orientation orientation;
        public ResponseType type;
    }

    [Serializable]
    public enum ResponseType
    {
        AFFIRMATIVE,
        NON_COMMITTAL,
        NEGATIVE
    }

    [Serializable]
    public enum Orientation
    {
        LONG_LINE_FIRST,
        SHORT_LINE_FIRST
    }
    [SerializeField] private string listName;
    [SerializeField] private List<Response> responses;

    public Response GetRandomResponse()
    {
        return responses[UnityEngine.Random.Range(0, responses.Count)];
    }
}