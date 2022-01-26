﻿/*
 * Description:             AssetBundleCollectSetting.cs
 * Author:                  TONYTANG
 * Create Date:             2020//10/25
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using MotionFramework.Editor;

/// <summary>
/// 单个搜集打包设定
/// </summary>
[Serializable]
public class Collector
{
    /// <summary>
    /// 搜集设定相对目录路径
    /// </summary>
    public string CollectFolderPath;

    /// <summary>
    /// 收集规则
    /// </summary>
    public EAssetBundleCollectRule CollectRule = EAssetBundleCollectRule.Collect;

    /// <summary>
    /// 搜集打包规则
    /// </summary>
    public EAssetBundleBuildRule BuildRule;

    /// <summary>
    /// 固定名字(仅当收集打包规则为LoadByConstName时有效)
    /// </summary>
    public string ConstName;

    public Collector()
    {

    }

    public Collector(string collectrelativefolderpath, EAssetBundleCollectRule collectrule = EAssetBundleCollectRule.Collect, EAssetBundleBuildRule buildrule = EAssetBundleBuildRule.ByFilePath)
    {
        CollectFolderPath = collectrelativefolderpath;
        CollectRule = collectrule;
        BuildRule = buildrule;
        ConstName = string.Empty;
    }

    /// <summary>
    /// 获取当前Collect对应的搜集类名
    /// </summary>
    /// <returns></returns>
    public string GetCollectorClassName()
    {
        if (BuildRule == EAssetBundleBuildRule.ByFilePath)
        {
            return typeof(LabelByFilePath).Name;
        }
        else if (BuildRule == EAssetBundleBuildRule.ByFolderPath)
        {
            return typeof(LabelByFolderPath).Name;
        }
        else if (BuildRule == EAssetBundleBuildRule.ByConstName)
        {
            return typeof(LableByConstName).Name;
        }
        else if(BuildRule == EAssetBundleBuildRule.ByFileOrSubFolder)
        {
            return typeof(LabelByFileAndSubFolderPath).Name;
        }
        else
        {
            return typeof(LabelNone).Name;
        }
    }
}

/// <summary>
/// 黑名单信息
/// </summary>
[Serializable]
public class BlackListInfo
{
    /// <summary>
    /// 后缀名黑名单列表
    /// </summary>
    public List<string> PostFixBlackList;

    /// <summary>
    /// 文件名黑名单列表
    /// </summary>
    public List<string> FileNameBlackList;

    public BlackListInfo()
    {
        PostFixBlackList = new List<string>();
        FileNameBlackList = new List<string>();
    }
}

/// <summary>
/// AssetBundleCollectSetting.cs
/// AB打包搜集规则数据序列化类
/// </summary>
public class AssetBundleCollectSetting : ScriptableObject
{
    /// <summary>
    /// 所有的AB搜集信息
    /// </summary>
    public List<Collector> AssetBundleCollectors = new List<Collector>();

    /// <summary>
    /// 黑名单信息
    /// </summary>
    public BlackListInfo BlackListInfo = new BlackListInfo();
}