﻿/*
 * Description:             ResourceModuleEnum.cs
 * Author:                  TONYTANG
 * Create Date:             2021//10/13
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TResource
{
    /// <summary>
    /// 资源加载模式
    /// </summary>
    public enum ResourceLoadMode
    {
        AssetBundle = 0,            // AssetBundle模式
        AssetDatabase = 1,          // 编辑器AssetDatabase模式
        Invalide = 2,               // 非法模式
    }

    /// <summary>
    /// 资源加载方式
    /// </summary>
    public enum ResourceLoadMethod
    {
        Sync = 1,          // 同步
        Async = 2          // 异步
    }

    /// <summary>
    /// 资源加载类型
    /// Note:
    /// 已加载的资源的加载类型不允许更改
    /// </summary>
    public enum ResourceLoadType
    {
        NormalLoad = 1,         // 正常加载(可通过Tick检测判定正常卸载)
        PermanentLoad = 2,      // 永久加载(常驻内存永不卸载)
    }

    /// <summary>
    /// 重写ResourceLoadType比较相关接口函数，避免ResourceLoadType作为Dictionary Key时，
    /// 底层调用默认Equals(object obj)和DefaultCompare.GetHashCode()导致额外的堆内存分配
    /// 参考:
    /// http://gad.qq.com/program/translateview/7194373
    /// </summary>
    public class ResourceLoadTypeComparer : IEqualityComparer<ResourceLoadType>
    {
        public bool Equals(ResourceLoadType x, ResourceLoadType y)
        {
            return x == y;
        }

        public int GetHashCode(ResourceLoadType x)
        {
            return (int)x;
        }
    }

    /// <summary>
    /// 资源加载任务状态
    /// </summary>
    public enum ResourceLoadState
    {
        None = 0,             // 空状态
        Waiting = 1,          // 等待加载状态
        Loading = 2,          // 加载中状态
        Complete = 3,         // 加载完成状态
        Cancel = 4,           // 取消状态
        Error = 5             // 出错状态
    }

}