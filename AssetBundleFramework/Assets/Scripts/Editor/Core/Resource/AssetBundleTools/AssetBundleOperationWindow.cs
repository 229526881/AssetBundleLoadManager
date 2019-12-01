/*
 * Description:             AssetBundleOperationWindow.cs
 * Author:                  TONYTANG
 * Create Date:             2019//12/01
 */

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// AssetBundleOperationWindow.cs
/// AB����������
/// </summary>
public class AssetBundleOperationWindow : EditorWindow
{
    /// <summary>
    /// ABĿ¼�洢Key
    /// </summary>
    private const string ABOT_ABFolderPathPreferenceKey = "ABOT_ABFolderPathKey";

    /// <summary>
    /// ABĿ¼
    /// </summary>
    private string ABFolderPath
    {
        get
        {
            return mABFolderPath;
        }
        set
        {
            mABFolderPath = value;
        }
    }
    private string mABFolderPath;

    /// <summary>
    /// ����UI����λ��
    /// </summary>
    private Vector2 mWindowUiScrollPos;

    /// <summary>
    /// ɾ����AB�ļ����б�
    /// </summary>
    private List<KeyValuePair<string, string>> mNeedDeleteABFileNameList = new List<KeyValuePair<string, string>>();

    /// <summary>
    /// ��ɾ����AB�ļ����б�
    /// </summary>
    private List<KeyValuePair<string, string>> mDeletedABFileNameList = new List<KeyValuePair<string, string>>();

    [MenuItem("Tools/AssetBundle/AssetBundle��������", false, 102)]
    public static void assetBundleOpterationWindow()
    {
        var assetbundleoperationwindow = EditorWindow.GetWindow<AssetBundleOperationWindow>();
        assetbundleoperationwindow.Show();
    }

    private void OnEnable()
    {
        InitData();
    }

    private void OnDisable()
    {
        SaveData();
    }

    private void OnDestroy()
    {
        SaveData();
    }

    /// <summary>
    /// ��ʼ����������
    /// </summary>
    private void InitData()
    {
        ABFolderPath = PlayerPrefs.GetString(ABOT_ABFolderPathPreferenceKey);
        Debug.Log("AssetBundle�������ڶ�ȡ����:");
        Debug.Log("ABĿ¼:" + ABFolderPath);
    }

    /// <summary>
    /// ��������
    /// </summary>
    private void SaveData()
    {
        PlayerPrefs.SetString(ABOT_ABFolderPathPreferenceKey, ABFolderPath);
        Debug.Log("AssetBundle�������ڱ�������:");
        Debug.Log("ABĿ¼:" + ABFolderPath);
    }

    public void OnGUI()
    {
        mWindowUiScrollPos = GUILayout.BeginScrollView(mWindowUiScrollPos);
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("ABĿ¼:", GUILayout.Width(50.0f));
        EditorGUILayout.TextField("", ABFolderPath);
        if (GUILayout.Button("ѡ��ABĿ¼", GUILayout.Width(150.0f)))
        {
            ABFolderPath = EditorUtility.OpenFolderPanel("ABĿ¼", "��ѡ����Ҫ������AB����Ŀ¼!", "");
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("������ɾ����ɾ��AB", GUILayout.Width(150.0f)))
        {
            doAnalyzeAndDeleteDeletedABFiles();
        }
        GUILayout.EndHorizontal();
        displayDeletedABResult();
        GUILayout.EndVertical();
        GUILayout.EndScrollView();
    }

    /// <summary>
    /// ͳ�Ʒ�����Ҫɾ����AB�ļ�
    /// </summary>
    private void doAnalyzeNeedDeleteAB()
    {
        if (Directory.Exists(ABFolderPath))
        {
            mNeedDeleteABFileNameList.Clear();
            var foldername = new DirectoryInfo(ABFolderPath).Name;
            var ab = AssetBundle.LoadFromFile(ABFolderPath + Path.DirectorySeparatorChar + foldername);
            if (ab != null)
            {
                var abmanifest = ab.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                var valideallabnames = abmanifest.GetAllAssetBundles();
                var existabfilespath = Directory.GetFiles(ABFolderPath, "*.*", SearchOption.TopDirectoryOnly).Where(f =>
                    !f.EndsWith(".meta") && !f.EndsWith(".manifest")
                ).ToList<string>();
                foreach (var existabfilepath in existabfilespath)
                {
                    var existabfilename = Path.GetFileName(existabfilepath);
                    // GetAllAssetBundles�ò���������ϢAB����Ŀ¼�µ�ͬ��������Ϣ�ļ���Ҫ�����ų�
                    if (!valideallabnames.Contains(existabfilename) && !existabfilename.Equals(foldername))
                    {
                        mNeedDeleteABFileNameList.Add(new KeyValuePair<string, string>(existabfilename, existabfilepath));
                        Debug.LogError($"��Ҫɾ����AB�ļ�:{existabfilepath}!");
                    }
                }
                ab.Unload(true);
            }
            else
            {
                Debug.LogError($"�Ҳ���ABĿ¼:{ABFolderPath}�µ�Manifest:{foldername}�ļ�!");
            }
        }
        else
        {
            Debug.LogError($"ABĿ¼:{ABFolderPath}������,�޷�������Ҫɾ����AB!");
        }
    }

    /// <summary>
    /// ������ɾ����Ҫ�Ƴ���AB�ļ�
    /// </summary>
    private bool doAnalyzeAndDeleteDeletedABFiles()
    {
        mDeletedABFileNameList.Clear();
        doAnalyzeNeedDeleteAB();
        if (mNeedDeleteABFileNameList != null)
        {
            if (mNeedDeleteABFileNameList.Count > 0)
            {
                foreach (var deleteabfilename in mNeedDeleteABFileNameList)
                {
                    if (File.Exists(deleteabfilename.Value))
                    {
                        File.Delete(deleteabfilename.Value);
                        mDeletedABFileNameList.Add(deleteabfilename);
                    }
                    else
                    {
                        Debug.LogError($"AB�ļ�������:{deleteabfilename.Value}��ɾ��ʧ��!");
                        return false;
                    }
                }
                Debug.Log("������ɾ����Ҫ�Ƴ���AB�ļ��������!");
                return true;
            }
            else
            {
                Debug.Log("û����Ҫɾ����AB!");
                return true;
            }
        }
        else
        {
            Debug.Log("����ִ��AB�ļ�ɾ������!");
            return false;
        }
    }

    /// <summary>
    /// ��ʾɾ����AB���
    /// </summary>
    private void displayDeletedABResult()
    {
        GUILayout.BeginVertical();
        if (mDeletedABFileNameList.Count > 0)
        {
            foreach (var deleteabfilename in mDeletedABFileNameList)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("��ɾ��AB�ļ���:" + deleteabfilename.Key, GUILayout.Width(300.0f));
                GUILayout.Label("��ɾ��ABȫ·��:" + deleteabfilename.Value, GUILayout.Width(600.0f));
                GUILayout.EndHorizontal();
            }
        }
        else
        {
            GUILayout.Label("δɾ���κ�AB!");
        }
        GUILayout.EndVertical();
    }
}