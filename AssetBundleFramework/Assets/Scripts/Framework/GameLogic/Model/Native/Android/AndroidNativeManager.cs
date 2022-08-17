/*
 * Description:             AndroidNativeManager.cs
 * Author:                  TONYTANG
 * Create Date:             2018/08/10
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_ANDROID
/// <summary>
/// AndroidNativeManager.cs
/// Android原生管理类
/// </summary>
public class AndroidNativeManager : NativeManager
{
    /// <summary>
    /// Android主Activity对象
    /// </summary>
    private AndroidJavaObject mAndroidActivity;

    /// <summary>
    /// 工具类相关的Andorid对象（存在工程中）
    /// </summary>
    AndroidJavaObject customToolObject;

    /// <summary>
    /// 初始化
    /// </summary>
    public override void init()
    {
        Debug.Log("AndroidNativeManager:init()");
        if (Application.platform == RuntimePlatform.Android)
        {
            //这里使用using的目的是确保AndroidJavaClass对象尽快被删除
            using (AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                //获得当前Activity(这里是为了获得MainActivity)
                mAndroidActivity = jc.GetStatic<AndroidJavaObject>("currentActivity");
                if (mAndroidActivity == null)
                {
                    Debug.Log("获取UnityMainActivity::mAndroidActivity成员失败！");
                }
                else
                {
                    Debug.Log("获取UnityMainActivity::mAndroidActivity成员成功!");
                }
            }
            customToolObject = new AndroidJavaObject("com.Unity.Tools.UTool");
        }
    }

    /// <summary>
    /// 调用原生方法
    /// </summary>
    public override void callNativeMethod()
    {
        Debug.Log("AndroidNativeManager:callNativeMethod()");
        if (mAndroidActivity != null)
        {
            mAndroidActivity.Call("javaMethod", "cs param");
        }
    }

    /// <summary>
    /// 调用原生方法
    /// </summary>
    /// <param name="apkfilepath">APK文件路径</param>
    public void installAPK(string apkfilepath)
    {
        Debug.Log("AndroidNativeManager:installAPK()");
        if (mAndroidActivity != null)
        {
            mAndroidActivity.Call("installAPK", apkfilepath);
        }

    }
    #region 工具类方法

    /// <summary>
    /// 强更APK
    /// </summary>
    /// <param name="apkfilepath">APK文件路径,注意下载路径要和FileProvider路径对应</param>
    public bool InstallApk(string apkfilepath)
    {
        bool state = false;
        if (customToolObject != null)
        {
             state = customToolObject.Call<bool>("installApk", apkfilepath);
        }
        Debug.Log("apk安装:" + state);
        return state;
    }
    #endregion

}
#endif