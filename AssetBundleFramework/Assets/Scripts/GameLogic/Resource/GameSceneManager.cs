﻿/*
 * Description:             GameSceneManager.cs
 * Author:                  TONYTANG
 * Create Date:             2018//10/20
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// GameSceneManager.cs
/// 游戏场景管理单例类
/// </summary>
public class GameSceneManager : SingletonTemplate<GameSceneManager>, IModuleInterface
{
    /// <summary>
    /// 模块名
    /// </summary>
    public string ModuleName
    {
        get
        {
            return this.GetType().ToString();
        }
    }

    /// <summary>
    /// 当前场景的AssetBundle信息
    /// </summary>
    private AssetBundleInfo mCurrentSceneABI;

    /// <summary>
    /// 初始化
    /// </summary>
    public void init()
    {
        // hook场景加载与切换回调
        SceneManager.sceneLoaded += onSceneLoaded;
        SceneManager.sceneUnloaded += onSceneUnloaded;
    }

    /// <summary>
    /// 同步加载场景
    /// </summary>
    /// <param name="scenename"></param>
    public void loadSceneSync(string scenename)
    {
        // 预加载资源类型需要在切换场景前卸载掉，切换场景后可能有新的预加载资源加载进来
        ModuleManager.Singleton.getModule<ResourceModuleManager>().unloadAllUnsedPreloadLoadedAB();

        // 场景资源计数采用手动管理计数的方式
        // 切场景时手动计数减1
        // 加载时手动计数加1，不绑定对象
        if (mCurrentSceneABI != null)
        {
            mCurrentSceneABI.release();
            mCurrentSceneABI = null;
        }

        // 减掉场景计数后，切换场景前强制卸载所有不再使用的正常加载的Unsed AB(递归判定释放)
        ModuleManager.Singleton.getModule<ResourceModuleManager>().unloadAllUnsedNormalLoadedAB();

        ModuleManager.mSingleton.getModule<ResourceModuleManager>().requstResource(scenename,
            (abi) =>
            {
                mCurrentSceneABI = abi;
                mCurrentSceneABI.retain();
            });

        SceneManager.LoadScene(scenename);
    }

    /// <summary>
    /// 异步加载场景
    /// </summary>
    /// <param name="scenename"></param>
    public void loadSceneAync(string scenename)
    {
        // 预加载资源类型需要在切换场景前卸载掉，切换场景后可能有新的预加载资源加载进来
        ModuleManager.Singleton.getModule<ResourceModuleManager>().unloadAllUnsedPreloadLoadedAB();

        // 场景资源计数采用手动管理计数的方式
        // 切场景时手动计数减1
        // 加载时手动计数加1，不绑定对象
        if (mCurrentSceneABI != null)
        {
            mCurrentSceneABI.release();
            mCurrentSceneABI = null;
        }

        ModuleManager.mSingleton.getModule<ResourceModuleManager>().requstResource(scenename,
            (abi) =>
            {
                mCurrentSceneABI = abi;
                mCurrentSceneABI.retain();
                SceneManager.LoadSceneAsync(scenename);
            },
            ABLoadType.NormalLoad,
            ABLoadMethod.Async);
    }

    /// <summary>
    /// 场景加载回调
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="mode"></param>
    private void onSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log(string.Format("场景:{0}被加载!", scene.name));
        //新场景加载后DO Something

    }

    /// <summary>
    /// 场景卸载回调
    /// </summary>
    /// <param name="scene"></param>
    private void onSceneUnloaded(Scene scene)
    {
        Debug.Log(string.Format("场景:{0}被卸载!", scene.name));
        // 场景卸载后，递归释放所有不再使用的正常加载的AB
        // 确保所有上一个场景不再使用的正常加载AB资源正确释放
        ModuleManager.Singleton.getModule<ResourceModuleManager>().unloadAllUnsedNormalLoadedAB();
    }
}