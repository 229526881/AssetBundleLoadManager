/*
 * Description:             ResourceHelper.cs
 * Author:                  TONYTANG
 * Create Date:             2021/10/12
 */

using System;
using System.Collections.Generic;

/// <summary>
/// ResourceHelper.cs
/// ��Դ��������
/// </summary>
public static class ResourceHelper
{
    /// <summary>
    /// ��Ч��Asset��Դ��׺Map<��׺��,�Ƿ���Ч>
    /// </summary>
    private static Dictionary<string, bool> mValideAssetPostFixMap = new Dictionary<string, bool>()
    {
        { ".prefab", true },
        { ".fbx", true },
        { ".mat", true },
        { ".png", true },
        { ".PNG", true },
        { ".jpg", true },
        { ".JPG", true },
        { ".mp3", true },
        { ".wav", true },
        { ".ogg", true },
        { ".shader", true },
        { ".anim", true },
        { ".spriteatlas", true },
        { ".playable", true },
        { ".asset", true },
    }

    /// <summary>
    /// ָ��Asset·���Ƿ���Ч��׺��
    /// </summary>
    /// <param name="assetPath"></param>
    /// <returns></returns>
    public static bool IsAssetPathHasValideAssetPostfix(string assetPath)
    {
        string ext = Path.GetExtension(assetPath);
        bool result = false;
        mValideAssetPostFixMap.TryGetValue(ext, out result);
        return result;
    }
}