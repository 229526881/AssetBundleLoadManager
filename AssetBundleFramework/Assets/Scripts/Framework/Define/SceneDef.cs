using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum SceneType
{
    //初始界面，无绑定
    _Start = -1,

    None = 0,

    Bootstarp,

    //主场景
    Main,

    //结束界面，无绑定
    _End,

}

public class SceneDataInfo
{
    public SceneType sceneType;
    public string path;

    public string FullPath
    {
        get { return "assets/_assets/scenes/" + path; }
    }
    public SceneDataInfo(SceneType type, string path)
    {
        sceneType = type;
        this.path = path;
    }
    public static SceneDataInfo GetSceneData(SceneType type)
    {
        for (int i = 0; i < _sceneDataList.Count; i++)
        {
            var item = _sceneDataList[i];
            if (item.sceneType == type)
            {
                return item;
            }
        }
        return null;
    }

    private static List<SceneDataInfo> _sceneDataList = new List<SceneDataInfo>()
    {
        new SceneDataInfo(SceneType.Bootstarp,"Bootstarp"),
        new SceneDataInfo(SceneType.Main,"Main"),
    };
}
