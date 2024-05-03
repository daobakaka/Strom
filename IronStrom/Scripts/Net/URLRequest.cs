using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class URLRequest : MonoBehaviour
{
    public enum Method
    {
        GET,
        POST,
    }

    public string url;
    public Method method;
    public Dictionary<string, string> headers;
    public WWWForm form;
    public string rawData;
    private bool _isLoading;

    public URLRequestResultEvent ResultEvent;

    private URLRequestQueueData urlQueueData;
    private UnityWebRequest www;

    private void Init(string url, URLRequestData data, Method method, Dictionary<string, string> headers = null)
    {
        form = null;
        rawData = null;
        if (headers != null)
        {
            string headerStr = "";
            foreach (KeyValuePair<string, string> kvp in headers)
            {
                headerStr += "[" + kvp.Key + " : " + kvp.Value + "] ";
            }
            Debuger.Log("Url headers : " + headerStr);
        }

        if (data != null && data.HasData())
        {
            if (method == Method.GET)
            {
                if (url.IndexOf("?") == -1)
                    url += "?";

                if (url.Substring(url.Length - 1) != "?")
                    url += "&";
                url += data.GetDataString();
            }
            else if (method == Method.POST)
            {
                if (data.rawData != null)
                {
                    rawData = data.rawData;
                    Debuger.Log("Url raw : " + rawData);
                }
                else
                {
                    form = data.GetDataForm();
                }
            }
        }

        this.url = url;
        this.method = method;
        this.headers = headers;
    }


    public float downloadProgress
    {
        get
        {
            if (www != null)
            {
                return www.downloadProgress;
            }
            return 0;
        }
    }

    public float uploadProgress
    {
        get
        {
            if (www != null)
            {
                return www.uploadProgress;
            }
            return 0;
        }
    }


    IEnumerator Load()
    {
        if(method == Method.GET || (form == null && rawData == null))
        {
            www = UnityWebRequest.Get(url);
        }
        else
        {
            if (rawData != null)
            {
                www = UnityWebRequest.Post(url, rawData, "application/json;charset=utf-8");
                //www.downloadHandler = new DownloadHandlerBuffer();
            }
            else
            {
                www = UnityWebRequest.Post(url, form);
            }
        }

        if(headers  != null)
        {
            foreach(var kvp in headers)
            {
                www.SetRequestHeader(kvp.Key, kvp.Value);
            }
        }

        yield return www.SendWebRequest();

        Debuger.Log("Url loaded : " + url);
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debuger.LogError(www.error);
        }
        else
        {
            Debuger.Log("Url result : " + www.downloadHandler.text);
        }


        URLRequestResult result = new URLRequestResult(this, www);
        if (ResultEvent != null)
        {
            ResultEvent.Invoke(result);
            ResultEvent.RemoveAllListeners();
        }

        if (urlQueueData != null)
        {
            urlQueueData.EndLoad();
            urlQueueData = null;
        }

        _isLoading = false;
        www.Dispose();
        www = null;

        Dequeue();
    }

    public bool isLoading
    {
        get
        {
            return _isLoading;
        }
    }


    private void OnDestroy()
    {
        requestList.Remove(this);
    }





    private static List<URLRequest> requestList = new List<URLRequest>();
    private static List<URLRequestQueueData> queueList = new List<URLRequestQueueData>();
    private static int numRequests = 3;



    public static URLRequestQueueData Enqueue(string url, URLRequestData requestData = null, Method method = Method.GET, object extra = null, Dictionary<string, string> headers = null)
    {
        URLRequestQueueData queueData = new URLRequestQueueData(url, requestData, method, headers);
        queueData.extra = extra;
        queueList.Add(queueData);
        Dequeue();
        return queueData;
    }


    public static void Dequeue()
    {
        if (queueList.Count == 0)
        {
            return;
        }

        URLRequestQueueData queueData = queueList[0];

        URLRequest request = null;

        if (requestList.Count < numRequests)
        {
            request = CreateBaseURLRequest(queueData.url, queueData.requestData, queueData.method, queueData.headers);
            requestList.Add(request);
        }
        else
        {
            for (int i = 0; i < requestList.Count; i++)
            {
                URLRequest req = requestList[i];
                if (!req.isLoading)
                {
                    request = req;
                    Debuger.Log("Url request : " + queueData.url);
                    request.Init(queueData.url, queueData.requestData, queueData.method, queueData.headers);
                    break;
                }
            }
        }

        if (request != null)
        {
            queueData.StartLoad(request);
            request.urlQueueData = queueData;
            request._isLoading = true;
            request.StartCoroutine(request.Load());
            queueList.RemoveAt(0);
        }
    }

    private static URLRequest CreateBaseURLRequest(string url, URLRequestData requestData = null, Method method = Method.GET, Dictionary<string, string> headers = null)
    {
        GameObject gameObj = new GameObject("URLRequest");
        URLRequest urlRequest = gameObj.AddComponent<URLRequest>();

        Debuger.Log("Url request : " + url);
        urlRequest.Init(url, requestData, method, headers);

        return urlRequest;
    }
}



public class URLRequestQueueData
{
    public string url;
    public URLRequestData requestData;
    public URLRequest.Method method;
    public URLRequestResultEvent ResultEvent = new URLRequestResultEvent();
    public object extra;
    public Dictionary<string, string> headers;

    private URLRequest request;
    private bool loaded;

    public URLRequestQueueData(string url, URLRequestData requestData, URLRequest.Method method, Dictionary<string, string> headers = null)
    {
        this.url = url;
        this.requestData = requestData;
        this.method = method;
        this.headers = headers;
    }

    public void StartLoad(URLRequest request)
    {
        this.request = request;
        if (request != null)
        {
            request.ResultEvent = this.ResultEvent;
        }
    }

    public void EndLoad()
    {
        this.request = null;
        loaded = true;
    }

    public bool isLoading
    {
        get
        {
            return !loaded;
        }
    }

    public float progress
    {
        get
        {
            if (loaded)
                return 1f;
            else if (request != null)
                return request.downloadProgress;
            return 0;
        }
    }

    public float uploadProgress
    {
        get
        {
            if (loaded)
                return 1f;
            else if (request != null)
                return request.uploadProgress;
            return 0;
        }
    }
}