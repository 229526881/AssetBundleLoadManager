/*
 * Description:             PathUtilities.cs
 * Author:                  TONYTANG
 * Create Date:             2021//04/11
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// PathUtilities.cs
/// ·����̬������
/// </summary>
public static class PathUtilities
{
    /// <summary>
    /// ��ȡ��ԴAsset���·��
    /// </summary>
    /// <param name="folderfullpath"></param>
    /// <returns></returns>
    public static string GetAssetsRelativeFolderPath(string folderfullpath)
    {
        var projectpathprefix = Application.dataPath.Replace("Assets", string.Empty);
        if (folderfullpath.StartsWith(projectpathprefix))
        {
            var relativefolderpath = folderfullpath.Replace(projectpathprefix, string.Empty);
            return relativefolderpath;
        }
        else
        {
            Debug.LogError($"Ŀ¼:{folderfullpath}������Ŀ��Ч·��,��ȡ���·��ʧ��!");
            return string.Empty;
        }
    }
}