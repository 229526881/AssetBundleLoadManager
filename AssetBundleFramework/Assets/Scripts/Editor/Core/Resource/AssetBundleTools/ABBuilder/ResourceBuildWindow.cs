﻿/*
 * Description:             ResourceBuildWindow.cs
 * Author:                  TONYTANG
 * Create Date:             2020//10/25
 */

using MotionFramework.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEngine;

/// <summary>
/// ResourceBuildWindow.cs
/// 资源打包工具
/// </summary>
public class ResourceBuildWindow : BaseEditorWindow
{
    /// <summary>
    /// 操作类型
    /// </summary>
    public enum EOperationType
    {
        ResourceBuild = 1,          // 资源打包窗口
        ResourceCollect,            // 资源搜集窗口
    }

    /// <summary>
    /// 整体UI滚动位置
    /// </summary>
    private Vector2 mWindowUiScrollPos;

    /// <summary>
    /// 当前窗口操作类型
    /// </summary>
    private EOperationType CurrentOperationType = EOperationType.ResourceBuild;

    /// <summary>
    /// 窗口操作类型名字数组
    /// </summary>
    private string[] mOperationTypeNameArray;

    /// <summary>
    /// 操作面板
    /// </summary>
    private string[] mToolBarStrings = { "资源打包", "资源搜集" };

    /// <summary>
    /// 操作面板选择索引
    /// </summary>
    private int mToolBarSelectIndex;

    /// <summary>
    /// 上次打开的文件夹路径
    /// </summary>
    private string LastOpenFolderPath = "Assets/";

    [MenuItem("Tools/AssetBundle/资源打包窗口", priority = 200)]
    static void ShowWindow()
    {
        var resourceBuildWindow = EditorWindow.GetWindow<ResourceBuildWindow>(false, "资源打包窗口");
        resourceBuildWindow.Show();
    }

    /// <summary>
    /// 初始化数据
    /// </summary>
    protected override void InitData()
    {
        base.InitData();
        mOperationTypeNameArray = Enum.GetNames(typeof(EOperationType));
        mToolBarSelectIndex = 0;
        CurrentOperationType = (EOperationType)Enum.Parse(typeof(EOperationType), mOperationTypeNameArray[mToolBarSelectIndex]);


        mFoldMap = new Dictionary<EFoldType, bool>();
        foreach (var foldType in Enum.GetValues(typeof(EFoldType)))
        {
            mFoldMap.Add((EFoldType)foldType, false);
        }

        InitResourceData();
    }

    /// <summary>
    /// 保存数据
    /// </summary>
    protected override void SaveData()
    {
        base.SaveData();

        SaveResourceData();
    }

    public void OnGUI()
    {
        mWindowUiScrollPos = GUILayout.BeginScrollView(mWindowUiScrollPos);
        GUILayout.BeginVertical();
        DisplayTagArea();
        if (CurrentOperationType == EOperationType.ResourceBuild)
        {
            DisplayResourceBuildArea();
        }
        else if (CurrentOperationType == EOperationType.ResourceCollect)
        {
            DisplayResourceCollectArea();
            DisplayBlackListArea();
            DisplayCommonArea();
        }
        GUILayout.EndVertical();
        GUILayout.EndScrollView();
    }

    /// <summary>
    /// 显示标签部分
    /// </summary>
    private void DisplayTagArea()
    {
        GUILayout.BeginHorizontal();
        var pretollbarselectindex = mToolBarSelectIndex;
        mToolBarSelectIndex = GUILayout.Toolbar(mToolBarSelectIndex, mToolBarStrings, EditorStyles.toolbarButton, GUILayout.ExpandWidth(true));
        if(pretollbarselectindex != mToolBarSelectIndex)
        {
            CurrentOperationType = (EOperationType)Enum.Parse(typeof(EOperationType), mOperationTypeNameArray[mToolBarSelectIndex]);
        }
        GUILayout.EndHorizontal();
    }

    #region 资源打包部分
    /// <summary>
    /// 压缩格式设置本地存储Key
    /// </summary>
    private const string ABBuildSettingCompressOptionKey = "ABBuildSettingCompressOption";

    /// <summary>
    /// 是否强制重新打包设置本地存储Key
    /// </summary>
    private const string ABBuildSettingIsForceRebuildKey = "ABBuildSettingIsForceRebuild";

    /// <summary>
    /// 是否AppendHash设置本地存储Key
    /// </summary>
    private const string ABBuildSettingIsAppendHashKey = "ABBuildSettingIsAppendHash";

    /// <summary>
    /// 是否Disable Write Type Tree设置本地存储Key
    /// </summary>
    private const string ABBuildSettingIsDisableWriteTypeTreeKey = "ABBuildSettingIsDisableWriteTypeTree";

    /// <summary>
    /// 是否Ignore Type Tree Change设置本地存储Key
    /// </summary>
    private const string ABBuildSettingIsIgnoreTypeTreeChangesKey = "ABBuildSettingIsIgnoreTypeTreeChanges";

    /// <summary>
    /// 是否受用PlayerSettingVersion设置本地存储Key
    /// </summary>
    private const string ABBuildSettingIsUsePlayerSettingVersionKey = "ABBuildSettingIsUsePlayerSettingVersion";

    /// <summary>
    /// 项目路径Hash值(用于使得PlayerPrefs存储的Key值唯一)
    /// </summary>
    private int mProjectPathHashValue;

    /// <summary>
    /// 初始化资源数据
    /// </summary>
    private void InitResourceData()
    {
        Debug.Log($"NewBuildWindow:InitResourceData()");
        mProjectPathHashValue = Application.dataPath.GetHashCode();
        // 创建资源打包器
        var buildTarget = EditorUserBuildSettings.activeBuildTarget;
        mAssetBuilder = new AssetBundleBuilder(buildTarget);

        // 读取配置
        LoadSettingsFromPlayerPrefs(mAssetBuilder);
    }

    /// <summary>
    /// 保存打包数据
    /// </summary>
    private void SaveResourceData()
    {

    }

    /// <summary>
    /// 存储配置
    /// </summary>
    private void SaveSettingsToPlayerPrefs(AssetBundleBuilder builder)
    {
        PlayerPrefs.SetString(ABBuildSettingCompressOptionKey, builder.CompressOption.ToString());
        PlayerPrefs.SetInt(ABBuildSettingIsForceRebuildKey, builder.IsForceRebuild ? 1 : 0);
        PlayerPrefs.SetInt(ABBuildSettingIsAppendHashKey, builder.IsAppendHash ? 1 : 0);
        PlayerPrefs.SetInt(ABBuildSettingIsDisableWriteTypeTreeKey, builder.IsDisableWriteTypeTree ? 1 : 0);
        PlayerPrefs.SetInt(ABBuildSettingIsIgnoreTypeTreeChangesKey, builder.IsIgnoreTypeTreeChanges ? 1 : 0);
    }

    /// <summary>
    /// 读取配置
    /// </summary>
    private void LoadSettingsFromPlayerPrefs(AssetBundleBuilder builder)
    {
        builder.CompressOption = (AssetBundleBuilder.ECompressOption)Enum.Parse(typeof(AssetBundleBuilder.ECompressOption), PlayerPrefs.GetString(ABBuildSettingCompressOptionKey, AssetBundleBuilder.ECompressOption.Uncompressed.ToString()));
        builder.IsForceRebuild = PlayerPrefs.GetInt($"{mProjectPathHashValue}_{ABBuildSettingIsForceRebuildKey}", 0) != 0;
        builder.IsAppendHash = PlayerPrefs.GetInt($"{mProjectPathHashValue}_{ABBuildSettingIsAppendHashKey}", 0) != 0;
        builder.IsDisableWriteTypeTree = PlayerPrefs.GetInt($"{mProjectPathHashValue}_{ABBuildSettingIsDisableWriteTypeTreeKey}", 0) != 0;
        builder.IsIgnoreTypeTreeChanges = PlayerPrefs.GetInt($"{mProjectPathHashValue}_{ABBuildSettingIsIgnoreTypeTreeChangesKey}", 0) != 0;
    }

    /// <summary>
    /// 构建器
    /// </summary>
    private AssetBundleBuilder mAssetBuilder = null;

    /// <summary>
    /// 是否展开设置
    /// </summary>
    private bool mShowSettingFoldout = true;

    /// <summary>
    /// 显示打包区域
    /// </summary>
    private void DisplayResourceBuildArea()
    {
        // 标题
        EditorGUILayout.LabelField("Build setup");
        EditorGUILayout.Space();

        // 输出路径
        EditorGUILayout.LabelField("Build Output", mAssetBuilder.OutputDirectory);

        // 构建选项
        EditorGUILayout.Space();
        mAssetBuilder.IsForceRebuild = GUILayout.Toggle(mAssetBuilder.IsForceRebuild, "Froce Rebuild", GUILayout.MaxWidth(120));

        // 高级选项
        using (new EditorGUI.DisabledScope(false))
        {
            EditorGUILayout.Space();
            mShowSettingFoldout = EditorGUILayout.Foldout(mShowSettingFoldout, "Advanced Settings");
            if (mShowSettingFoldout)
            {
                int indent = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 1;
                mAssetBuilder.CompressOption = (AssetBundleBuilder.ECompressOption)EditorGUILayout.EnumPopup("Compression", mAssetBuilder.CompressOption);
                mAssetBuilder.IsAppendHash = EditorGUILayout.ToggleLeft("Append Hash", mAssetBuilder.IsAppendHash, GUILayout.MaxWidth(120));
                mAssetBuilder.IsDisableWriteTypeTree = EditorGUILayout.ToggleLeft("Disable Write Type Tree", mAssetBuilder.IsDisableWriteTypeTree, GUILayout.MaxWidth(200));
                mAssetBuilder.IsIgnoreTypeTreeChanges = EditorGUILayout.ToggleLeft("Ignore Type Tree Changes", mAssetBuilder.IsIgnoreTypeTreeChanges, GUILayout.MaxWidth(200));
                EditorGUI.indentLevel = indent;
            }
        }

        // 构建按钮
        EditorGUILayout.Space();
        if (GUILayout.Button("Build", GUILayout.MaxHeight(40)))
        {
            string title;
            string content;
            if (mAssetBuilder.IsForceRebuild)
            {
                title = "警告";
                content = "确定开始强制构建吗，这样会删除所有已有构建的文件";
            }
            else
            {
                title = "提示";
                content = "确定开始增量构建吗";
            }
            if (EditorUtility.DisplayDialog(title, content, "Yes", "No"))
            {
                // 清空控制台
                EditorUtilities.ClearUnityConsole();
                //var appVersion = float.Parse(Application.version);
                //mAssetBuilder.BuildVersion = Application.version;

                // 存储配置
                SaveSettingsToPlayerPrefs(mAssetBuilder);

                EditorApplication.delayCall += ExecuteBuild;
            }
            else
            {
                Debug.LogWarning("[Build] 打包已经取消");
            }
        }
    }

    /// <summary>
    /// 执行构建
    /// </summary>
    private void ExecuteBuild()
    {
        ResourceBuildTool.DoBuildAssetBundleByBuilder(mAssetBuilder);
    }
    #endregion

    #region 资源搜集部分
    /// <summary>
    /// 折叠类型
    /// </summary>
    public enum EFoldType
    {
        BuildRule = 1,          // 打包策略
        BlackList,              // 黑名单
        PostFixBlackList,       // 后缀名黑名单
        FileNameBlackList,      // 文件名黑名单
    }

    /// <summary>
    /// 重写EFoldType比较相关接口函数，避免EFoldType作为Dictionary Key时，
    /// 底层调用默认Equals(object obj)和DefaultCompare.GetHashCode()导致额外的堆内存分配
    /// 参考:
    /// http://gad.qq.com/program/translateview/7194373
    /// </summary>
    public class EFoldTypeComparer : IEqualityComparer<EFoldType>
    {
        public bool Equals(EFoldType x, EFoldType y)
        {
            return x == y;
        }

        public int GetHashCode(EFoldType x)
        {
            return (int)x;
        }
    }

    /// <summary>
    /// 折叠Map
    /// </summary>
    private Dictionary<EFoldType, bool> mFoldMap;

    /// <summary>
    /// 后缀名黑名单每行显示个数
    /// </summary>
    private const int POST_FIX_NUM_PER_ROW = 10;

    /// <summary>
    /// 文件名黑名单每行显示个数
    /// </summary>
    private const int FILE_NAME_NUM_PER_ROW = 5;

    /// <summary>
    /// 显示资源搜集区域
    /// </summary>
    private void DisplayResourceCollectArea()
    {
        GUILayout.BeginVertical();
        mFoldMap[EFoldType.BuildRule] = EditorGUILayout.Foldout(mFoldMap[EFoldType.BuildRule], "AB打包资源搜集");
        if (!mFoldMap[EFoldType.BuildRule])
        {
            if (AssetBundleCollectSettingData.Setting.AssetBundleCollectors.Count != 0)
            {
                for (int i = 0; i < AssetBundleCollectSettingData.Setting.AssetBundleCollectors.Count; i++)
                {
                    DisplayOneCollect(AssetBundleCollectSettingData.Setting.AssetBundleCollectors[i]);
                }
                if (GUILayout.Button("+", GUILayout.ExpandWidth(true), GUILayout.Height(20.0f)))
                {
                    var chosenfolderpath = EditorUtility.OpenFolderPanel("选择搜集目录", LastOpenFolderPath, "");
                    if (string.IsNullOrEmpty(chosenfolderpath) == false && AssetBundleCollectSettingData.AddAssetBundleCollector(chosenfolderpath))
                    {
                        var relativefolderpath = PathUtilities.GetAssetsRelativeFolderPath(chosenfolderpath);
                        LastOpenFolderPath = relativefolderpath;
                        Debug.Log($"添加资源搜集目录:{chosenfolderpath}成功!");
                    }
                }
            }
        }
        else
        {
            EditorGUILayout.LabelField("无打包策略配置", GUILayout.ExpandWidth(true), GUILayout.Height(20f));
        }
        GUILayout.EndVertical();
    }

    /// <summary>
    /// 显示单个搜集信息
    /// </summary>
    /// <param name="collector"></param>
    private void DisplayOneCollect(Collector collector)
    {
        GUILayout.BeginHorizontal("Box");
        EditorGUILayout.LabelField(collector.CollectFolderPath, GUILayout.ExpandWidth(true), GUILayout.Height(20.0f));
        collector.CollectRule = (EAssetBundleCollectRule)EditorGUILayout.EnumPopup(collector.CollectRule, GUILayout.Width(120.0f), GUILayout.Height(20.0f));
        collector.BuildRule = (EAssetBundleBuildRule)EditorGUILayout.EnumPopup(collector.BuildRule, GUILayout.Width(150f), GUILayout.Height(20.0f));
        // 强制Igore规则的目录打包规则为Ignore
        if (collector.CollectRule == EAssetBundleCollectRule.Ignore)
        {
            collector.BuildRule = EAssetBundleBuildRule.Ignore;
        }
        if(collector.BuildRule == EAssetBundleBuildRule.ByConstName)
        {
            collector.ConstName = EditorGUILayout.TextField(collector.ConstName, GUILayout.Width(120.0f), GUILayout.Height(20.0f));
        }
        else
        {
            collector.ConstName = string.Empty;
        }
        if (GUILayout.Button("-", GUILayout.Width(30.0f), GUILayout.Height(20.0f)))
        {
            if (AssetBundleCollectSettingData.RemoveAssetBundleCollector(collector))
            {
                Debug.Log($"移除资源搜集目录:{collector.CollectFolderPath}成功!");
            }
            else
            {
                Debug.LogError($"移除资源搜集目录:{collector.CollectFolderPath}失败!");
            }
        }
        GUILayout.EndHorizontal();
    }

    /// <summary>
    /// 显示黑名单区域
    /// </summary>
    private void DisplayBlackListArea()
    {
        GUILayout.BeginVertical("box");
        mFoldMap[EFoldType.BlackList] = EditorGUILayout.Foldout(mFoldMap[EFoldType.BlackList], "AB打包资源搜集");
        if (!mFoldMap[EFoldType.BlackList])
        {
            DisplayPostFixBlackListArea();
            DisplayFileNameBlackListArea();
        }
        GUILayout.EndVertical();
    }

    /// <summary>
    /// 显示后缀名黑名单区域
    /// </summary>
    private void DisplayPostFixBlackListArea()
    {
        GUILayout.BeginVertical("box");
        mFoldMap[EFoldType.PostFixBlackList] = EditorGUILayout.Foldout(mFoldMap[EFoldType.PostFixBlackList], "后缀名黑名单");
        if (!mFoldMap[EFoldType.PostFixBlackList])
        {
            if (AssetBundleCollectSettingData.Setting.BlackListInfo.PostFixBlackList.Count != 0)
            {
                int mod;
                for (int i = 0; i < AssetBundleCollectSettingData.Setting.BlackListInfo.PostFixBlackList.Count; i++)
                {
                    mod = i % POST_FIX_NUM_PER_ROW;
                    if (mod == 0)
                    {
                        GUILayout.BeginHorizontal("box");
                    }
                    DisplayOnePostFix(AssetBundleCollectSettingData.Setting.BlackListInfo.PostFixBlackList, i);
                    if (mod == (POST_FIX_NUM_PER_ROW - 1) || (i == AssetBundleCollectSettingData.Setting.BlackListInfo.PostFixBlackList.Count - 1))
                    {
                        GUILayout.EndHorizontal();
                    }
                }
            }
            else
            {
                EditorGUILayout.LabelField("无后缀名黑名单配置", GUILayout.ExpandWidth(true), GUILayout.Height(20f));
            }

            if (GUILayout.Button("+", GUILayout.ExpandWidth(true), GUILayout.Height(20f)))
            {
                AssetBundleCollectSettingData.AddPostFixBlackList();
            }
        }
        GUILayout.EndVertical();
    }

    /// <summary>
    /// 显示单个后缀名黑名单
    /// </summary>
    /// <param name="postFixBlackList"></param>
    /// <param name="index"></param>
    private void DisplayOnePostFix(List<string> postFixBlackList, int index)
    {
        EditorGUI.BeginChangeCheck();
        postFixBlackList[index] = EditorGUILayout.TextField(postFixBlackList[index], GUILayout.Width(70f), GUILayout.Height(20f));
        if (EditorGUI.EndChangeCheck())
        {
            if (!postFixBlackList[index].StartsWith("."))
            {
                Debug.LogWarning($"后缀名必须已.开头!");
            }
        }
        if (GUILayout.Button("-", GUILayout.Width(30.0f), GUILayout.Height(20.0f)))
        {
            var postFix = postFixBlackList[index];
            if (AssetBundleCollectSettingData.RemovePostFixBlackList(index))
            {
                Debug.Log($"移除后缀名黑名单索引:{index}后缀名:{postFix}成功!");
            }
            else
            {
                Debug.LogError($"移除后缀名黑名单索引:{index}后缀名:{postFix}失败!");
            }
        }
    }

    /// <summary>
    /// 显示文件名黑名单区域
    /// </summary>
    private void DisplayFileNameBlackListArea()
    {
        GUILayout.BeginVertical("box");
        mFoldMap[EFoldType.FileNameBlackList] = EditorGUILayout.Foldout(mFoldMap[EFoldType.FileNameBlackList], "文件名黑名单");
        if (!mFoldMap[EFoldType.FileNameBlackList])
        {
            if (AssetBundleCollectSettingData.Setting.BlackListInfo.FileNameBlackList.Count != 0)
            {
                int mod;
                for (int i = 0; i < AssetBundleCollectSettingData.Setting.BlackListInfo.FileNameBlackList.Count; i++)
                {
                    mod = i % FILE_NAME_NUM_PER_ROW;
                    if (mod == 0)
                    {
                        GUILayout.BeginHorizontal("box");
                    }
                    DisplayOneFileName(AssetBundleCollectSettingData.Setting.BlackListInfo.FileNameBlackList, i);
                    if (mod == (FILE_NAME_NUM_PER_ROW - 1) || (i == AssetBundleCollectSettingData.Setting.BlackListInfo.FileNameBlackList.Count - 1))
                    {
                        GUILayout.EndHorizontal();
                    }
                }
            }
            else
            {
                EditorGUILayout.LabelField("无文件名黑名单配置", GUILayout.ExpandWidth(true), GUILayout.Height(20f));
            }

            if (GUILayout.Button("+", GUILayout.ExpandWidth(true), GUILayout.Height(20f)))
            {
                AssetBundleCollectSettingData.AddFileNameBlackList();
            }
        }
        GUILayout.EndVertical();
    }

    /// <summary>
    /// 显示单个名字黑名单
    /// </summary>
    /// <param name="fileNameBlackList"></param>
    /// <param name="index"></param>
    private void DisplayOneFileName(List<string> fileNameBlackList, int index)
    {
        EditorGUI.BeginChangeCheck();
        fileNameBlackList[index] = EditorGUILayout.TextField(fileNameBlackList[index], GUILayout.Width(140f), GUILayout.Height(20f));
        if (EditorGUI.EndChangeCheck())
        {
            if (fileNameBlackList[index].Equals(string.Empty))
            {
                Debug.LogWarning($"文件名不能为空!");
            }
        }
        if (GUILayout.Button("-", GUILayout.Width(30.0f), GUILayout.Height(20.0f)))
        {
            var fileName = fileNameBlackList[index];
            if (AssetBundleCollectSettingData.RemoveFileNameBlackList(index))
            {
                Debug.Log($"移除文件名黑名单索引:{index}后缀名:{fileName}成功!");
            }
            else
            {
                Debug.LogError($"移除文件名黑名单索引:{index}后缀名:{fileName}失败!");
            }
        }
    }

    /// <summary>
    /// 显示公共区域
    /// </summary>
    private void DisplayCommonArea()
    {
        GUILayout.BeginVertical("box");
        if (GUILayout.Button("保存", GUILayout.ExpandWidth(true), GUILayout.Height(20.0f)))
        {
            AssetBundleCollectSettingData.SaveFile();
        }
        GUILayout.EndVertical();
    }
    #endregion
}