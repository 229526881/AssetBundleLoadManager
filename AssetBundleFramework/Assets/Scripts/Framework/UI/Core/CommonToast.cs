using SFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 显示消息队列
/// </summary>
public class CommonToast :MonoBehaviour
{
    //public Transform tipPrefab;
    //public Transform noticePrefab;
    //public Transform notice;
    //PoolGroup tipPool;
    //public int limitNum = 3;

    //public  void Start()
    //{
    //    PrefabPool prefabPool2 = PoolManager.Instance.CreatePrefabPool("GameTip", tipPrefab);
    //    prefabPool2.limitCount = 3;
    //    prefabPool2.limitInstances = true;
    //}

    //public void ShowToast(string msg,float delta=0.35f,float  waitTime=1f)
    //{
    //    //因此需要判断队列
    //    tipPool= PoolManager.Instance["GameTip"];
    //    Transform go= tipPool.Spawn("tip",transform);
    //    go.GetComponent<SingleTip>().ShowTip(msg, "tipShow",delta);
    //}

    ///// <summary>
    ///// 展示公告
    ///// </summary>
    ///// <param name="msg"></param>
    ///// <param name="delta">停留时间</param>
    //public void ShowNotice(string msg, float delta = 1f)
    //{
    //    tipPool = PoolManager.Instance["GameTip"];
    //    Transform go = tipPool.Spawn("tip", notice);
    //    go.GetComponent<SingleTip>().ShowTip(msg, "noticeShow", delta);
    //}
}
