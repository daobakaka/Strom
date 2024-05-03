using UnityEngine;
using LitJson;
using DashGame;

public class URLRequestData
{

    private bool encrypt;
    private DictList<string, object> dataList;
    public string rawData;

    public URLRequestData(bool encrypt = false)
    {
        this.encrypt = encrypt;
        dataList = new DictList<string, object>();
    }

    public void Add(string key, object value)
    {
        dataList.Add(key, value);
    }

    public void AddRaw(string data)
    {
        rawData = data;
    }

    public bool HasData()
    {
        return dataList.Count > 0 || rawData != null;
    }


    public string GetDataString()
    {
        string info = "";
        string result = "";

        if (encrypt)
        {
            JsonData json = JsonMapper.ToObject("{}");
            for (int i = 0; i < dataList.Count; i++)
            {
                string key = dataList.GetKeyByIndex(i);
                object value = dataList.GetValueByIndex(i);
                if (value is JsonData)
                {
                    JsonData valueJson = value as JsonData;
                    json[key] = valueJson;
                    info += "[" + key + " : " + valueJson.ToJson() + "] ";
                }
                else
                {
                    info += "[" + key + " : " + value + "] ";
                    json[key] = value.ToString();
                }
            }
            result = "v=" + DecryptionUtil.Encryption(json.ToJson());
        }
        else
        {
            string[] strArr = new string[dataList.Count];
            for (int i = 0; i < dataList.Count; i++)
            {
                string key = dataList.GetKeyByIndex(i);
                object value = dataList.GetValueByIndex(i);
                info += "[" + key + " : " + value + "] ";
                strArr[i] = key + "=" + value;
            }
            result = string.Join("&", strArr);
        }
        Debuger.Log("Url data : " + info);
        return result;
    }


    public WWWForm GetDataForm()
    {
        string info = "";
        WWWForm form = new WWWForm();
        if (encrypt)
        {
            JsonData json = JsonMapper.ToObject("{}");
            for (int i = 0; i < dataList.Count; i++)
            {
                string key = dataList.GetKeyByIndex(i);
                object value = dataList.GetValueByIndex(i);
                if (value is JsonData)
                {
                    JsonData valueJson = value as JsonData;
                    json[key] = valueJson;
                    info += "[" + key + " : " + valueJson.ToJson() + "] ";
                }
                else
                {
                    info += "[" + key + " : " + value + "] ";
                    json[key] = value.ToString();
                }
            }
            form.AddField("v", DecryptionUtil.Encryption(json.ToJson()));
        }
        else
        {
            for (int i = 0; i < dataList.Count; i++)
            {
                string key = dataList.GetKeyByIndex(i);
                object value = dataList.GetValueByIndex(i);
                info += "[" + key + " : " + value + "] ";
                form.AddField(key, value.ToString());
            }
        }

        Debuger.Log("Url data : " + info);
        Debuger.Log("from : " + form.data.ToString());
        return form;
    }
}
