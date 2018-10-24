/*
 * Description:             AudioManager.cs
 * Author:                  TONYTANG
 * Create Date:             2018//10/20
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AudioManager.cs
/// ��Ч����������
/// </summary>
public class AudioManager : SingletonTemplate<AudioManager>, IModuleInterface {

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