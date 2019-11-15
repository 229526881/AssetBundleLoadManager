/*
 * Description:             ExtractBuildInResourceWindow.cs
 * Author:                  TANGHUAN
 * Create Date:             2019//11/13
 */

using System;
using UnityEditor;
using UnityEngine;

/// <summary>
/// ������Դ��ȡ���ܴ���
/// </summary>
public class ExtractBuildInResourceWindow : EditorWindow
{
    /// <summary>
    /// ������Դ·��
    /// </summary>
    private const string BuildInResourcePath = "Resources/unity_builtin_extra";

    [MenuItem("Tools/Assets/��ȡ������Դ", false)]
    public static void dpAssetBrowser()
    {
        var exbuildinreswindow = EditorWindow.GetWindow<ExtractBuildInResourceWindow>();
        exbuildinreswindow.Show();
    }

    public void OnGUI()
    {
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("��ȡ������Դ", GUILayout.MaxWidth(200.0f)))
        {
            extractBuildInResource();
        }
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
    }

    /// <summary>
    /// ��ȡ������Դ
    /// </summary>
    private void extractBuildInResource()
    {
        DIYLog.Log("�˹���δ���!");
        //DIYLog.Log("BuildInResourcePath:" + BuildInResourcePath);
        //UnityEngine.Object[] unityassets = AssetDatabase.LoadAllAssetsAtPath(BuildInResourcePath);
        //DIYLog.Log("unityassets.Length = " + unityassets.Length);
    }
}