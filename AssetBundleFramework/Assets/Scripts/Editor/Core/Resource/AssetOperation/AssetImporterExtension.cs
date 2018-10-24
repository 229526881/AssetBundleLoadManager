/*
 * Description:             AssetImporterExtension.cs�ű�����������չ
 * Author:                  tanghuan
 * Create Date:             2018/04/10
 */

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// AssetImporterExtension.cs�ű�����������չ
/// </summary>
public class AssetImporterExtension : SingletonTemplate<AssetImporterExtension>{

    /// <summary>
    /// �޸�ָ��asset��ab��������
    /// </summary>
    /// <param name="assetpath"></param>
    /// <param name="abname"></param>
    public void changeAssetBundleName(string assetpath, string abname)
    {
        var assetimporter = AssetImporter.GetAtPath(assetpath);
        if(assetimporter != null)
        {
            assetimporter.assetBundleName = abname;
        }
        else
        {
            Debug.LogError(string.Format("�Ҳ���·��Ϊ:{0}��Assetimporter!", assetpath));
        }
    }
}
