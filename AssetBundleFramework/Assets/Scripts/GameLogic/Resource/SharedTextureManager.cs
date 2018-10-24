/*
 * Description:             SharedTextureManager.cs
 * Author:                  TONYTANG
 * Create Date:             2018//10/20
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// SharedTextureManager.cs
/// ���������������
/// </summary>
public class SharedTextureManager : SingletonTemplate<SharedTextureManager>, IModuleInterface {

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