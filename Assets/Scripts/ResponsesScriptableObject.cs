/** 
  * @file ResponsesScriptableObject.cs
  * @brief Insert brief description here
  * 
  * @author Logan Bender
  * 
  * @copyright Universal Creative. All rights reserved.
**/

using CreativeAti.Config;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ResponsesConfig", menuName = "Scriptable Object/INI Config/Responses")]
public class ResponsesScriptableObject : IniScriptableObject
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

    public override Dictionary<string, string> GetIniData()
    {
        Dictionary<string, string> data = new Dictionary<string, string>();
        data.Add("name", listName);
        for (int i = 0; i < responses.Count; i++)
        {
            data.Add("upperResponse" + i, responses[i].upperLineResponseText);
            data.Add("lowerResponse" + i, responses[i].lowerLineResponseText);
            data.Add("orientation" + i, responses[i].orientation.ToString());
            data.Add("responseType" + i, responses[i].type.ToString());
        }

        return data;
    }

    public override void InitializeFromIniData(Dictionary<string, string> data)
    {
        listName = data["name"];
        responses = new List<Response>();
        for (int i = 0; data.ContainsKey("response" + i); i++)
        {
            Response response = new Response();
            response.upperLineResponseText = data["upperResponse" + i];
            response.lowerLineResponseText = data["lowerResponse" + i];
            response.orientation = (Orientation)Enum.Parse(typeof(Orientation), data["orientation" + i]);
            response.type = (ResponseType)Enum.Parse(typeof(ResponseType), data["responseType" + i]);
            responses.Add(response);
        }
    }
}