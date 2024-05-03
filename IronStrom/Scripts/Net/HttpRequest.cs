using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using LitJson;
using DashGame;

public class HttpRequestEvent : UnityEvent<bool, JsonData> { }

public class HttpRequest
{

    public static string urlAddress = "https://xwf.zhijianhuyou.com/api/";

    public HttpRequestEvent ResultEvent;

    protected Dictionary<string, string> headers;
    protected bool showPending;

    protected string m; 

    public HttpRequest()
    {
        ResultEvent = new HttpRequestEvent();
    }

    protected virtual void Send(URLRequestData data, bool showPending = false, URLRequest.Method method = URLRequest.Method.POST)
    {
        this.showPending = showPending;
        if (showPending)
        {
            //ProgressPanel.Show();
        }

        Dictionary<string, string> headers = new Dictionary<string, string>();
        URLRequest.Enqueue(GetURL(), data, method, null, headers).ResultEvent.AddListener(Received);
    }

    protected virtual void Received(URLRequestResult requestResult)
    {
        if (showPending)
        {
            //ProgressPanel.Hide();
        }

        try
        {
            if (requestResult.success)
            {
                JsonData data = requestResult.GetJson();
                if (data == null)
                {
                    Debuger.LogError("invilid data : " + requestResult.GetString());
                    Debuger.LogError("error : " + requestResult.GetError());
                    ResultEvent.Invoke(false, null);
                }
                else if (JsonUtil.ContainKey(data, "code"))
                {
                    int code = JsonUtil.ToInt(data, "code");
                    if (code == 1)
                    {
                        JsonData resultData = data["data"];

                        //if(JsonUtil.ContainKey(data, "time"))
                        //    GameTime.SetSyncDateTime(JsonUtil.ToInt(data, "time"));

                        ResultEvent.Invoke(true, resultData);
                    }
                    else
                    {
                        Debuger.LogError("Error code : " + code);
                        //AlertPanel.Show(Language.GetStr("HttpError", code.ToString()));
                        ResultEvent.Invoke(false, data);
                    }
                }
                else
                {
                    Debuger.LogError("Error : " + data.ToJson());
                    ResultEvent.Invoke(false, data);
                }
            }
            else
            {
                if (requestResult.GetError().Contains("401"))
                {
                    //Session.Logout();
                    //GameController.instance.SetState(GameController.GameState.Loading);
                }
                ResultEvent.Invoke(false, null);
            }
        }
        catch (System.Exception e)
        {
            Debuger.LogException(e);
        }

        ResultEvent.RemoveAllListeners();
    }

    protected virtual string GetURL()
    {
        return urlAddress + m;
    }
}
