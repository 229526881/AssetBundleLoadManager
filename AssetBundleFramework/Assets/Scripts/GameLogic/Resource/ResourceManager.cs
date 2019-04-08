/*
 * Description:             ResourceManager.cs
 * Author:                  TONYTANG
 * Create Date:             2018//10/20
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ResourceManager.cs
/// �ϲ���Դ������������
/// </summary>
public class ResourceManager : SingletonTemplate<ResourceManager>, IModuleInterface {

    /// <summary>
    /// ģ����
    /// </summary>
    public string ModuleName
    {
        get
        {
            return this.GetType().ToString();
        }
    }

    /// <summary>
    /// ��ȡ����ʵ������
    /// </summary>
    /// <param name="wndname"></param>
    /// <returns></returns>
    public GameObject GetWindowInstance(string wndname)    
    {
        GameObject wndinstance = null;
        ModuleManager.Singleton.getModule<ResourceModuleManager>().requstResource(
        wndname,
        (abi) =>
        {
            wndinstance = abi.instantiateAsset(wndname);
        });
        return wndinstance;
    }
}