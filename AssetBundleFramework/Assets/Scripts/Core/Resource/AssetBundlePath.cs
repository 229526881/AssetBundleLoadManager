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
    /// <summary> AB������Դ·�� /// </summary>
    private static string ABBuildinPath = Application.streamingAssetsPath + "/PC/";
    /// <summary> AB�ȸ�����Դ·�� /// </summary>
    private static string ABHotUpdatePath = Application.persistentDataPath + "/PC/";
    /// <summary> ������Ϣ�ļ��� /// </summary>
    public const string DependencyFileName = "pc";
#elif UNITY_ANDROID
    /// <summary> AB������Դ·�� /// </summary>
    private static string ABBuildinPath = Application.streamingAssetsPath + "/Android/";
    /// <summary> AB�ȸ�����Դ·�� /// </summary>
    private static string ABHotUpdatePath = Application.persistentDataPath + "/Android/";
    /// <summary> ������Ϣ�ļ��� /// </summary>
    public const string DependencyFileName = "android";
#elif UNITY_IOS
    /// <summary> AB������Դ·�� /// </summary>
    private static string ABBuildinPath = Application.streamingAssetsPath + "/IOS/";
    /// <summary> AB�ȸ�����Դ·�� /// </summary>
    private static string ABHotUpdatePath = Application.persistentDataPath + "/IOS/";
    /// <summary> ������Ϣ�ļ��� /// </summary>
    public const string DependencyFileName = "ios";
#endif

    /// <summary> ������ϢAsset�� /// </summary>
    public const string DependencyAssetName = "AssetBundleManifest";

    /// <summary>
    /// ��ȡAB���ڼ���·��
    /// </summary>
    /// <param name="resName"></param>
    /// <param name="wwwPath"></param>
    /// <returns></returns>
    public static string GetABInnerPath()
    {
        return ABBuildinPath;
    }

    /// <summary>
    /// ��ȡAB����ȫ·��(���ȸ������߼��ж�)
    /// </summary>
    /// <param name="abname"></param>
    /// <returns></returns>
    public static string GetABLoadFullPath(string abname)
    {
        //TODO:
        //�ȸ��߼�·���ж�
        //��ʱĬ�Ϸ��ذ���·��
        return ABBuildinPath + abname;
    }
    #endregion
}
