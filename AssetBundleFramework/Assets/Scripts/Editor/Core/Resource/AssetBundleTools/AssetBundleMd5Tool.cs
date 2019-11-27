/*
 * Description:             AssetBundleMd5Tool.cs
 * Author:                  TONYTANG
 * Create Date:             2019//11/26
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEngine;

/// <summary>
/// AssetBundleMd5Tool.cs
/// AB��Md5�Աȷ�������
/// </summary>
public class AssetBundleMd5Tool : EditorWindow
{
    /// <summary>
    /// �ļ��ı�״̬
    /// </summary>
    private enum EChangedFileStatus
    {
        Changed = 1,            // �ı�
        Delete,                 // �Ƴ�
        Add,                    // ����
    }

    /// <summary>
    /// ABĿ¼
    /// </summary>
    private string mABFolderPath;

    /// <summary>
    /// AB��Md5��Ϣ���Ŀ¼
    /// </summary>
    private string mABMd5OutputFolderPath;

    /// <summary>
    /// AB��Md5�Ա������ļ�1·��
    /// </summary>
    private string mABMd5Compare1FilePath;

    /// <summary>
    /// AB��Md5�Ա������ļ�2·��
    /// </summary>
    private string mABMd5Compare2FilePath;

    /// <summary>
    /// MD5ֵ�иı���ļ����б�
    /// </summary>
    private List<KeyValuePair<string, EChangedFileStatus>> mMD5ChangedABFileNameList = new List<KeyValuePair<string, EChangedFileStatus>>();

    /// <summary>
    /// ����λ��
    /// </summary>
    private Vector2 uiScrollPos;

    [MenuItem("Tools/Assetbundle/AB MD5�������", false)]
    public static void abMd5Analyzing()
    {
        var assetbundlemd5analyzewindow = EditorWindow.GetWindow<AssetBundleMd5Tool>();
        assetbundlemd5analyzewindow.Show();
    }

    public void OnGUI()
    {
        GUILayout.BeginVertical();
        if (GUILayout.Button("ѡ��ABĿ¼"))
        {
            mABFolderPath = EditorUtility.OpenFolderPanel("ABĿ¼", "��ѡ����Ҫ������AB����Ŀ¼!", "OK");
        }
        GUILayout.Label("ABĿ¼:");
        GUILayout.Label(mABFolderPath);
        if(GUILayout.Button("ѡ��MD5���Ŀ¼"))
        {
            mABMd5OutputFolderPath = EditorUtility.OpenFolderPanel("MD5���Ŀ¼", "��ѡ����ҪAB��MD5�������Ŀ¼!", "OK");
        }
        GUILayout.Label("MD5���Ŀ¼:");
        GUILayout.Label(mABMd5OutputFolderPath);
        if (GUILayout.Button("����Ŀ��Ŀ¼AB��Md5", GUILayout.MaxWidth(150.0f)))
        {
            doAssetBundleMd5Caculation();
        }
        if (GUILayout.Button("ѡ��MD5�ԱȾ��ļ�"))
        {
            mABMd5Compare1FilePath = EditorUtility.OpenFilePanel("MD5�ԱȾ��ļ�", "��ѡ����Ҫ�Աȵľ�MD5�����ļ�·��!", "OK");
        }
        GUILayout.Label("MD5�ԱȾ��ļ�:");
        GUILayout.Label(mABMd5Compare1FilePath);
        if (GUILayout.Button("ѡ��MD5�Ա����ļ�"))
        {
            mABMd5Compare2FilePath = EditorUtility.OpenFilePanel("MD5�Ա����ļ�", "��ѡ����Ҫ�Աȵ���MD5�����ļ�·��!", "OK");
        }
        GUILayout.Label("MD5�Ա����ļ�:");
        GUILayout.Label(mABMd5Compare2FilePath);
        if (GUILayout.Button("�Ա����ϰ汾��MD5", GUILayout.MaxWidth(150.0f)))
        {
            doAssetBundleMd5Comparison();
        }
        GUILayout.EndVertical();
        displayComparisonResult();
    }

    /// <summary>
    /// ִ��AB��MD5����
    /// </summary>
    private void doAssetBundleMd5Caculation()
    {
        if(Directory.Exists(mABFolderPath))
        {
            if(Directory.Exists(mABMd5OutputFolderPath))
            {
                var targetplatform = EditorUserBuildSettings.activeBuildTarget;
                var md5filename = "ABMD5-" + Application.version + "-" + targetplatform + ".txt";
                var md5filefullpath = mABMd5OutputFolderPath + Path.DirectorySeparatorChar + md5filename;
                var abfilespath = Directory.GetFiles(mABFolderPath, "*.*", SearchOption.TopDirectoryOnly).Where(f =>
                    !f.EndsWith(".meta") && !f.EndsWith(".manifest")
                );
                if(!File.Exists(md5filefullpath))
                {
                    using (File.Create(md5filefullpath))
                    {

                    }
                }
                using (var md5sw = new StreamWriter(md5filefullpath, false, Encoding.UTF8))
                {
                    var md5hash = MD5.Create();
                    var sb = new StringBuilder();
                    foreach(var abfilepath in abfilespath)
                    {
                        using (var abfilefs = File.OpenRead(abfilepath))
                        {
                            sb.Clear();
                            var abfilename = Path.GetFileName(abfilepath);
                            var md5value = md5hash.ComputeHash(abfilefs);
                            foreach(var md5byte in md5value)
                            {
                                sb.Append(md5byte.ToString("x2"));
                            }
                            md5sw.WriteLine(abfilename + ":" + sb.ToString());
                        }
                    }
                }
            }
            else
            {
                Debug.LogError("MD5���Ŀ¼�����ڣ���ѡ����ЧAB��MD5�������Ŀ¼!");
            }
        }
        else
        {
            Debug.LogError("Ŀ��ABĿ¼�����ڣ���ѡ����ЧĿ¼!");
        }
    }

    /// <summary>
    /// ִ��MD5�Ա�
    /// </summary>
    private void doAssetBundleMd5Comparison()
    {
        if (File.Exists(mABMd5Compare1FilePath))
        {
            if (File.Exists(mABMd5Compare2FilePath))
            {
                mMD5ChangedABFileNameList.Clear();
                var md51map = new Dictionary<string, string>();
                var md52map = new Dictionary<string, string>();
                using (var md51sr = new StreamReader(mABMd5Compare1FilePath))
                {
                    using (var md52sr = new StreamReader(mABMd5Compare2FilePath))
                    {
                        while(!md51sr.EndOfStream)
                        {
                            var lineinfo = md51sr.ReadLine().Split(':');
                            md51map.Add(lineinfo[0], lineinfo[1]);
                        }
                        while (!md52sr.EndOfStream)
                        {
                            var lineinfo = md52sr.ReadLine().Split(':');
                            md52map.Add(lineinfo[0], lineinfo[1]);
                        }
                    }
                }
                // ���жԱ�
                foreach(var md51 in md51map)
                {
                    if(md52map.ContainsKey(md51.Key) && !md52map[md51.Key].Equals(md51.Value))
                    {
                        mMD5ChangedABFileNameList.Add(new KeyValuePair<string, EChangedFileStatus>(md51.Key, EChangedFileStatus.Changed));
                    }
                    else
                    {
                        mMD5ChangedABFileNameList.Add(new KeyValuePair<string, EChangedFileStatus>(md51.Key, EChangedFileStatus.Delete));
                    }
                }
                foreach (var md52 in md52map)
                {
                    if (!md51map.ContainsKey(md52.Key))
                    {
                        mMD5ChangedABFileNameList.Add(new KeyValuePair<string, EChangedFileStatus>(md52.Key, EChangedFileStatus.Add));
                    }
                }
            }
            else
            {
                Debug.LogError("�Ա�Ŀ���ļ�2�����ڣ���ѡ����Ч�ļ�·��!");
            }
        }
        else
        {
            Debug.LogError("�Ա�Ŀ���ļ�1�����ڣ���ѡ����Ч�ļ�·��!");
        }
    }

    /// <summary>
    /// ��ʾMD5�Աȷ������
    /// </summary>
    private void displayComparisonResult()
    {
        GUILayout.BeginVertical();
        uiScrollPos = GUILayout.BeginScrollView(uiScrollPos);
        foreach (var mdchangedabfilename in mMD5ChangedABFileNameList)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("�ļ���:" + mdchangedabfilename);
            GUILayout.Space(10);
            GUILayout.Label("״̬:" + mdchangedabfilename);
            GUILayout.EndHorizontal();
        }
        GUILayout.EndScrollView();
        GUILayout.EndVertical();
    }
}