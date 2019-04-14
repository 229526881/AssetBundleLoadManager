/*
 * Description:             HotUpdateModuleManager.cs
 * Author:                  TONYTANG
 * Create Date:             2019//04/14
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// HotUpdateModuleManager.cs
/// ��Դ�ȸ�ģ��
/// </summary>
public class HotUpdateModuleManager : SingletonMonoBehaviourTemplate<HotUpdateModuleManager>, IModuleInterface
{
    /// <summary>
    /// ��ʵģ����
    /// </summary>
    public string ModuleName
    {
        get
        {
            return this.GetType().ToString();
        }
    }


}