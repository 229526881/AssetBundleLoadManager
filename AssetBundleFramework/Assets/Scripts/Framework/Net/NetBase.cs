using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SFramework;
using Utility = SFramework.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class NetBase :MonoBehaviour
{
    private static NetBase _instance;
    public static NetBase Inst
    {
        get
        {
            if (_instance == null)
            {
                GameObject netBase = new GameObject("NetBase");
                _instance = netBase.AddComponent<NetBase>();
                GameObject.DontDestroyOnLoad(netBase);
            }
            return _instance;
        }
    }

    //private static string baseUrl = "http://10.0.1.179:11090";
    //public  string baseUrl = "http://sandbox.platform.moxigame.cn/dev/mountainsea";
    //protected static string baseUrl = "https://hserver.moxigame.cn/mountainsea/";         
 
    //服务器调用地址
    public string baseUrl = "";
    //同步服务器时间
    public long serverTime=-1;


    /// <summary>
    /// 根据游戏状态获得对应的服务器地址 TODO
    /// </summary>
    public void Init()
    {
        
    }

    /// <summary>
    /// 获得服务器地址
    /// </summary> 
    /// <returns></returns>
    public string GetNetUrl()
    {
        return baseUrl;
    }

    /// <summary>
    /// HTTP POST请求
    /// </summary>
    /// <param name="methodName">请求接口名</param>
    /// <param name="dic">需要上传的数据 key - value</param>
    /// <param name="callback">回调函数</param>
    public Coroutine Post<T>(string methodName, WWWForm form, Action<T> callback, Action<string> errEvent)
    {
        return StartCoroutine(NetPost(methodName, form, callback, errEvent));
    }

    /// <summary>
    /// HTTP GET请求
    /// </summary> 
    /// <param name="methodName">请求接口名</param>
    /// <param name="callback">回调函数</param>
    public Coroutine Get<T>(string methodName, Action<T> callback, Action<string> errEvent, string reqUrl = "", string paramName = "data")
    {
        if (string.IsNullOrEmpty(reqUrl))
        {
            reqUrl = baseUrl;
        }
        return StartCoroutine(NetGet(methodName, callback, errEvent, reqUrl, paramName));
    }

    protected  virtual IEnumerator NetPost<T>(string methodName, WWWForm form, Action<T> callBack,Action<string>errEvent=null)
    {
        string url = baseUrl + methodName;
        if (null == form) form = new WWWForm();
        //form.AddField("gameId",PlayerModel.Instance.GameId);
        //form.AddField("token",PlayerModel.Instance.Token);
        using (UnityWebRequest request = UnityWebRequest.Post(url, form))
        {
            request.timeout = 5;
            yield return request.SendWebRequest();

            if (!Utility.CheckRequestState(request))
            {
                string errMsg = request.error + "\n" + request.downloadHandler.text;
                Debug.LogError(errMsg);
                errEvent?.Invoke(errMsg);
            }
            else
            {
                JObject data = JsonConvert.DeserializeObject<JObject>(request.downloadHandler.text);
                if ((int)(data["code"]) != 0)
                {
                    errEvent?.Invoke((string)(data["errMsg"]));
                    Debug.LogError((methodName + data["errMsg"]));
                    yield break;
                }
                callBack?.Invoke(JsonUtility.FromJson<T>((string)data["data"]));
            }
        }
    }

    protected virtual IEnumerator NetGet<T>(string methodName, Action<T> callBack, Action<string> errEvent, string baseUrl,string paramName )
    {
        string url =baseUrl + methodName;
        //if (PlayerModel.Instance.LoginState)
        //{
        //    url += "?gameId=" + PlayerModel.Instance.GameId;
        //    url += "&token=" + PlayerModel.Instance.Token;
        //}
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.timeout = 5;  
            yield return request.SendWebRequest();
            if (!Utility.CheckRequestState(request))
            {
                string errmsg = request.error + "\n" + request.downloadHandler.text;
                Debug.LogError(errmsg);
                errEvent?.Invoke(errmsg);
            }
            else
            {
                JObject data = JsonConvert.DeserializeObject<JObject>(request.downloadHandler.text);
                if ((int)(data["code"]) != 0)
                {
                    errEvent?.Invoke((string)(data["errMsg"]));
                    Debug.LogError((string)(methodName + data["errMsg"]));
                    yield break;
                }
               serverTime= (long)data["serverTime"];
               callBack?.Invoke(JsonUtility.FromJson<T>((string)data[paramName]));
            }
        }
    }

}
