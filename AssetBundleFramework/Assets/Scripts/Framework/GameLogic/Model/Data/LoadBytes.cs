using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LoadBytes 
{
    public static void loadDataFromBin<T>(ref List<T>list, ref Dictionary<int,T> map)
    {
        Stream fs = ConfLoader.Singleton.getStreamByteName(typeof(T).Name);
        if (fs != null)
        {
            BinaryReader br = new BinaryReader(fs);
            uint offset = 0;
            bool frist = true;
            try
            {
                while (fs.Length - fs.Position > 0)
                {
                    if (frist)
                    {
                        frist = false;
                        var count = br.ReadInt32();
                        list = new List<T>(count);
                        map = new Dictionary<int, T>(count);
                    }

                    var length = br.ReadInt32();
                    var data = br.ReadBytes(length);
                    //var obj = t_AuthorInfoBuffer.deserialize(data, ref offset);
                    //offset = 0;
                    //list.Add(obj);
                    //map.Add(obj.Id, obj);
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
