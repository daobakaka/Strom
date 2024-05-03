using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using LitJson;
using UnityEngine.Networking;

public class URLRequestResultEvent : UnityEvent<URLRequestResult> { }

public class URLRequestResult
{

    public URLRequest target;
    private bool _success;
    private UnityWebRequest www;

    public URLRequestResult(URLRequest target, UnityWebRequest www)
    {
        this.target = target;
        this.www = www;
        this._success = www.result == UnityWebRequest.Result.Success;
    }

    public bool success
    {
        get { return _success; }
    }

    public string GetError()
    {
        return www.error;
    }

    public string GetString()
    {
        return www.downloadHandler.text;
    }

    public JsonData GetJson()
    {
        return ParseRecieveJsonData(www.downloadHandler.text);
    }

    public static JsonData ParseRecieveJsonData(string data)
    {
        int index = data.IndexOf("{");
        if (index >= 0)
        {
            data = data.Substring(index);
        }

        try
        {
            return JsonMapper.ToObject(data);
        }
        catch (JsonException e)
        {
            Debuger.LogException(e);
            Debuger.LogError(data);
        }

        JsonData json = new JsonData();
        json["c"] = -1;
        json["d"] = data;
        return json;
    }
}
