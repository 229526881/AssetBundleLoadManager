/*
 * Description:             HotUpdateConfig.cs
 * Author:                  TONYTANG
 * Create Date:             2019//08/04
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// HotUpdateConfig.cs
/// ��Ϸ�ȸ��µ�ַ������
/// </summary>
[Serializable]
public class HotUpdateConfig {

    /// <summary>
    /// �ȸ�APK����
    /// </summary>
    public string APKName;

    /// <summary>
    /// �ȸ��±��ز��Ե�ַ
    /// </summary>
    public string HotUpdateLocalURL;

    /// <summary>
    /// �ȸ�����ʽ��ַ
    /// </summary>
    public string HotUpdateURL;
}