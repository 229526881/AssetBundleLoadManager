using System;
using UnityEngine;
using static NetApis;

public class NetRequest
{
    public static Coroutine TestLogin<T>(string name, string openId, string avatar, Action<T> callBack, Action<string> errEvent)
    {
        string url = NetApis.Apis["testLogin"] + "?name=" + name;
        if (!string.IsNullOrEmpty(openId))
        {
            url += "&openId=" + openId;
        }
        if (!string.IsNullOrEmpty(avatar))
        {
            url += "&avatar=" + avatar;
        }
        return NetBase.Inst.Get(url, callBack, errEvent);
    }
}
