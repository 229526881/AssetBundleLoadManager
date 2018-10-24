/*
 * Description:             WindowManager.cs
 * Author:                  TONYTANG
 * Create Date:             2018//10/25
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// WindowManager.cs
/// ���ڹ�������
/// </summary>
public class WindowManager : SingletonTemplate<WindowManager>, IModuleInterface
{

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