﻿/*
 * Description:             ResourceBuildTool.cs
 * Author:                  TONYTANG
 * Create Date:             2021//12/19
 */

using MotionFramework.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEditor;
using UnityEngine;

/// <summary>
/// ResourceBuildTool.cs
/// 资源打包工具
/// </summary>
public static class ResourceBuildTool 
{
    /// <summary>
    /// 执行AssetBundle打包
    /// </summary>
    /// <param name="buildTarget">打包平台</param>
    /// <param name="isForceBuild">是否强制重新打包</param>
    /// <param name="buildVersion">打包版本</param>
    public static void DoBuildAssetBundle(BuildTarget buildTarget, bool isForceBuild = false, string buildVersion = "1")
    {
        if(string.IsNullOrEmpty(buildVersion))
        {
            buildVersion = Application.version;
        }
        var assetBundleBuilder = new AssetBundleBuilder(buildTarget, buildVersion);
        assetBundleBuilder.CompressOption = AssetBundleBuilder.ECompressOption.ChunkBasedCompressionLZ4;
        assetBundleBuilder.IsForceRebuild = isForceBuild;
        assetBundleBuilder.IsAppendHash = false;
        assetBundleBuilder.IsDisableWriteTypeTree = false;
        assetBundleBuilder.IsIgnoreTypeTreeChanges = false;

        ExecuteAssetBundleBuild(assetBundleBuilder);
    }

    /// <summary>
    /// 使用指定AssetBundleBuilder执行AssetBundle打包
    /// </summary>
    /// <param name="assetBundleBuilder">AssetBundle打包</param>
    public static void DoBuildAssetBundleByBuilder(AssetBundleBuilder assetBundleBuilder)
    {
        ExecuteAssetBundleBuild(assetBundleBuilder);
    }

    /// <summary>
    /// 执行构建
    /// </summary>
    /// <param name="assetBundleBuilder"></param>
    private static void ExecuteAssetBundleBuild(AssetBundleBuilder assetBundleBuilder)
    {
        double buildVersion = 0;
        if (!double.TryParse(assetBundleBuilder.BuildVersion, out buildVersion))
        {
            Debug.LogError($"解析版本号:{assetBundleBuilder.BuildVersion}失败,格式无效，执行资源打包失败!");
            return;
        }
        assetBundleBuilder.BuildVersion = buildVersion.ToString("N1", CultureInfo.CreateSpecificCulture("en-US"));
        var timecounter = new TimeCounter();
        timecounter.Start("AssetBundleBuild");
        assetBundleBuilder.PreAssetBuild();
        assetBundleBuilder.PostAssetBuild();
        timecounter.End();
    }
}