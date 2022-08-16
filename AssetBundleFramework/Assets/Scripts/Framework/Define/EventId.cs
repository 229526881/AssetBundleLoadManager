using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EventId
{
    //切换场景
    Change_Scene = 0,

    Login_Check=1,

    //更新加载界面进度条
    Update_Loading_Progress,

    /// <summary>
    /// 资源压缩包结束
    /// </summary>
    Zip_Finish,

    //引导相关-------
    /// <summary>
    /// 引导的动画结束
    /// </summary>
    Guide_Slow_End,

    /// <summary>
    /// 引导遮罩点击
    /// </summary>
    Guide_Mask_Click,

    /// <summary>
    /// 遮罩处空白点击
    /// </summary>
    Guide_White_Click,

    /// <summary>
    /// 所有引导结束
    /// </summary>
    Guide_All_Finish,

    //场景切换
    Change_SceneEnd,

}
