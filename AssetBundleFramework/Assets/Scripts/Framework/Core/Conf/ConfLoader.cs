/*
 * Description:             配置表加载辅助单例类
 * Author:                  tanghuan
 * Create Date:             2018/09/05
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// 配置表加载辅助单例类
/// </summary>
public class ConfLoader : SingletonTemplate<ConfLoader>
{

    /// <summary>
    /// Excel表格数据存储目录
    /// </summary>
    public const string ExcelDataFolderPath = "Assets/_assets/table/databytes/";

    public ConfLoader()
    {

    }

    /// <summary>
    /// 获取表格配置数据的二进制流数据,读取逻辑需要调整TODO
    /// </summary>
    /// <param name="bytefilename"></param>
    /// <returns></returns>
    public Stream getStreamByteName(string bytefilename)
    {
        string path = ExcelDataFolderPath + bytefilename+".bytes";
        //传递null 会使text不在内存中存在
        var textasset = ResourceManager.Singleton.LoadAssetSync<TextAsset>(null,path);
        //var textasset = Resources.Load(ExcelDataFolderPath + bytefilename) as TextAsset;
        var memorystream = new MemoryStream(textasset.bytes);
        return memorystream;
    }

    public  void loadDataFromBin<T>(string name,Func<byte[],T>endEvent)
    {
        Stream fs = getStreamByteName(name);
        if (fs != null)
        {
            BinaryReader br = new BinaryReader(fs);
            bool frist = true;
            try
            {
                while (fs.Length - fs.Position > 0)
                {
                    if (frist)
                    {
                        frist = false;
                        var count = br.ReadInt32();
                        //list = new List<T>(count);
                        //map = new Dictionary<int, T>(count);
                    }

                    var length = br.ReadInt32();
                    var data = br.ReadBytes(length);
                    //t_AuthorInfoBuffer.deserialize(data, ref offset);
                    T item =    endEvent(data);
                 //   list.Add(item);
                    //map.Add(item.Id, item);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("import data error: " + ex.ToString());
            }
            br.Close();
            fs.Close();
        }
    }
}
