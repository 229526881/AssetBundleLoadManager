/*
 * Description:             AssetBundlePath.cs
 * Author:                  TONYTANG
 * Create Date:             2018//09/28
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AssetBundlePath.cs
/// AB��Դ·����ؾ�̬�࣬�����ƽ̨·������
/// </summary>
public static class AssetBundlePath {

    #region AssetBundle
#if UNITY_STANDALONE
    private static string ABBuildinPath = Application.streamingAssetsPath + "/PC/";
#elif UNITY_ANDROID
    private static string ABBuildinPath = Application.streamingAssetsPath + "/Android/";
#elif UNITY_IOS
    private static string ABBuildinPath = Application.streamingAssetsPath + "/IOS/";
#endif

    /// <summary> ������Ϣ�ļ��� /// </summary>
    public const string DependencyFileName = "allabdep";

    /// <summary>
    /// ��ȡAB����·��
    /// </summary>
    /// <param name="resName"></param>
    /// <param name="wwwPath"></param>
    /// <returns></returns>
    public static string GetABPath()
    {
        return ABBuildinPath;
    }
    #endregion
}
