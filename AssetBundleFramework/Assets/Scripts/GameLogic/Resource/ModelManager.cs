/*
 * Description:             ModelManager.cs
 * Author:                  TONYTANG
 * Create Date:             2018//10/20
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ModelManager.cs
/// ģ�͹�������
/// </summary>
public class ModelManager : SingletonTemplate<ModelManager>, IModuleInterface {

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
}