﻿/*
 * Description:             BundleAssetLoader.cs
 * Author:                  TONYTANG
 * Create Date:             2021//10/30
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TResource
{
    /// <summary>
    /// BundleAssetLoader.cs
    /// AB模式下的Asset加载器
    /// </summary>
    public class BundleAssetLoader : AssetLoader
    {
        /// <summary>
        /// 主AssetBundle路径
        /// </summary>
        public string MainAssetBundlePath
        {
            get;
            protected set;
        }

        /// <summary>
        /// 依赖的AB路径数组
        /// </summary>
        public string[] DepABPaths
        {
            get;
            protected set;
        }

        /// <summary>
        /// 当前AB加载信息
        /// </summary>
        protected AssetBundleInfo mABInfo;

        /// <summary>
        /// 当前AB依赖的AB对应的AB信息列表(用于构建当前AssetBundleInfo)
        /// </summary>
        protected List<AssetBundleInfo> mDepAssetBundleInfoList;

        /// <summary>
        /// 所在AB是否加载完成
        /// </summary>
        protected bool mIsABLoaded;

        /// <summary>
        /// 主Bundle请求UID
        /// </summary>
        protected int mMainBundleLoaderUID;

        /// <summary>
        /// 主Bundle加载器
        /// </summary>
        protected BundleLoader mMainBundleLoader;

        public BundleAssetLoader() : base()
        {
            MainAssetBundlePath = null;
            DepABPaths = null;
            mABInfo = null;
            mDepAssetBundleInfoList = new List<AssetBundleInfo>();
            mIsABLoaded = false;
            mMainBundleLoader = null;
        }

        public override void onCreate()
        {
            base.onCreate();
            MainAssetBundlePath = null;
            DepABPaths = null;
            mABInfo = null;
            mDepAssetBundleInfoList.Clear();
            mIsABLoaded = false;
            mMainBundleLoader = null;
        }

        public override void onDispose()
        {
            base.onDispose();
            MainAssetBundlePath = null;
            DepABPaths = null;
            mABInfo = null;
            mDepAssetBundleInfoList.Clear();
            mIsABLoaded = false;
            mMainBundleLoader = null;
        }

        /// <summary>
        /// 初始化Bundle路径信息
        /// </summary>
        /// <param name="ownerAssetBundlePath"></param>
        /// <param name="depAssetBundlePaths"></param>
        public void initBundleInfo(string ownerAssetBundlePath, string[] depAssetBundlePaths)
        {
            MainAssetBundlePath = ownerAssetBundlePath;
            DepABPaths = depAssetBundlePaths;
            mIsABLoaded = false;
            // 创建加载器时就添加相关AssetBundle计数，确保资源加载管理正确
            // 后续加载取消时会返还对应计数，AB的计数会在AB加载完成后返还(因为AB的计数会在AB加载器创建时添加计数)
            // 仅主AB采取和Asset加载方式一致的方式，依赖AB采用NormalLoad方式
            mABInfo = ResourceModuleManager.Singleton.CurrentResourceModule.getOrCreateAssetBundleInfo(MainAssetBundlePath, LoadType);
            mABInfo.retain();
            // 关联AssetInfo和AssetBundleInfo
            mABInfo.addAssetInfo(mAssetInfo);
            AssetBundleInfo depAssetBundleInfo;
            for(int i = 0, length = DepABPaths.Length; i < length; i++)
            {
                depAssetBundleInfo = ResourceModuleManager.Singleton.CurrentResourceModule.getOrCreateAssetBundleInfo(DepABPaths[i], ResourceLoadType.NormalLoad);
                mDepAssetBundleInfoList.Add(depAssetBundleInfo);
                depAssetBundleInfo.retain();
            }
        }

        /// <summary>
        /// 响应资源加载
        /// </summary>
        protected override void onLoad()
        {
            base.onLoad();
            // Note:
            // 只有主AB采用Asset的加载方式，依赖AB一律采取Normal加载方式
            if(LoadMethod == ResourceLoadMethod.Sync)
            {
                // BundlerLoader会负责加载自身AB和依赖AB，这里只需触发主AB加载即可
                mMainBundleLoaderUID = ResourceModuleManager.Singleton.requstAssetBundleSync(MainAssetBundlePath, out mMainBundleLoader, onAssetBundleLoadComplete, LoadType);
            }
            else if(LoadMethod == ResourceLoadMethod.Async)
            {
                mMainBundleLoaderUID = ResourceModuleManager.Singleton.requstAssetBundleAsync(MainAssetBundlePath, out mMainBundleLoader, onAssetBundleLoadComplete, LoadType);
            }
            else
            {
                Debug.LogError($"不支持的加载方式:{LoadMethod}");
                failed();
            }
        }

        /// <summary>
        /// 响应AB加载完成
        /// </summary>
        /// <param name="assetBundleLader"></param>
        protected void onAssetBundleLoadComplete(BundleLoader assetBundleLader, int requestUid)
        {
            mIsABLoaded = true;
            onAssetBundleLoadComplete();
        }

        /// <summary>
        /// 响应所属AB加载完成
        /// </summary>
        protected void onAssetBundleLoadComplete()
        {
            ResourceLogger.log($"Asset:{ResourcePath}的所在AB:{MainAssetBundlePath}加载完成!");
            if(LoadMethod == ResourceLoadMethod.Sync)
            {
                var asset = mMainBundleLoader.getAssetBundle().LoadAsset(mAssetInfo.AssetName, mAssetInfo.AssetType);
                onAssetLoadComplete(asset);
            }
            else if(LoadMethod == ResourceLoadMethod.Async)
            {
                mAssetAsyncRequest = mMainBundleLoader.getAssetBundle().LoadAssetAsync(mAssetInfo.AssetName, mAssetInfo.AssetType);
                mAssetAsyncRequest.completed += onAssetAsyncLoadComplete;
            }
        }

        /// <summary>
        /// Asset异步加载完成
        /// </summary>
        /// <param name="asyncOperation"></param>
        protected void onAssetAsyncLoadComplete(AsyncOperation asyncOperation)
        {
            onAssetLoadComplete(mAssetAsyncRequest.asset);
        }

        /// <summary>
        /// 响应Asset加载完成
        /// </summary>
        /// <param name="asset"></param>
        protected void onAssetLoadComplete(Object asset)
        {
            ResourceLogger.log($"Asset:{ResourcePath}加载完成!");
            // 加载完成后无论都要设置setResource确保后续的正常使用
            mAssetInfo.setResource(asset);
            if (asset != null)
            {
                complete();
            }
            else
            {
                failed();
            }
        }

        /// <summary>
        /// 响应资源加载取消
        /// </summary>
        protected override void onCancel()
        {
            base.onCancel();
        }

        /// <summary>
        /// 响应加载完成
        /// </summary>
        protected override void onComplete()
        {
            // 上层多个加载逻辑回调，在完成后根据调用getAsset或bindAsset情况去添加计数和绑定
            // 返还提前添加的Asset以及AssetBundle计数信息，确保正确的资源管理
            // 依赖AB的真正计数添加由BundleLoader去负责(确保单个AB的依赖AB计数只添加一次)
            mABInfo.release();
            for (int i = 0, length = mDepAssetBundleInfoList.Count; i < length; i++)
            {
                mDepAssetBundleInfoList[i].release();
            }
            base.onComplete();
        }
    }
}