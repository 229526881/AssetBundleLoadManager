using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SFramework
{

    public enum UILayerType
    {

        /// <summary>
        /// GUI层
        /// </summary>
        GUI,

        /// <summary>
        /// 弹出
        /// </summary>
        Pop,

        /// <summary>
        /// 提示
        /// </summary>
        Tip,

        /// <summary>
        /// 顶层
        /// </summary>
        Top
    }
    public enum UIType
    {
        _Start = 0,

        Login = 1,     //登录

        Loading = 2,   //加载界面

        Main = 3,      //主界面

        _End,
    }

    public enum UIShowTypes
    {
        UIFullScreen,       // 全屏显示，全屏界面使用该选项可获得更高性能
        UIAddition,         // 叠加显示，性能较差
        UISingle,           // 单界面显示，只显示当前界面和背景界面，性能较好
    };

    public class UIDataInfo
    {

        public UIType uiType;

        public string path;

        public System.Type type;

        /// <summary>
        /// 遮罩是否会阻挡点击
        /// </summary>
        public bool preventTouch = true;

        public UIDataInfo(UIType uiType, string path, System.Type type, bool preventTouch = false)
        {
            this.uiType = uiType;
            this.type = type;
            this.path = path;
            this.preventTouch = preventTouch;
        }

        public static UIDataInfo GetUIData(UIType type)
        {
            for (int i = 0; i < _uiDataList.Count; i++)
            {
                var item = _uiDataList[i];
                if (item.uiType == type)
                {
                    return item;
                }
            }
            return null;
        }

        public string FullPath
        {
            get
            {
                return path;
            }
        }

        public string FullPrefabPath
        {
            get
            {
                return "Assets/_assets/uiassets/uiprefabs/"+ path+".prefab";
            }
        }

        public string Name
        {
            get
            {
                //对路径进行处理
                string fileName = path;
                if (path.IndexOf("/") >= 0)
                {
                    fileName = path.Substring(path.LastIndexOf("/") + 1);
                }
                return fileName;
            }
        }

        private static List<UIDataInfo> _uiDataList = new List<UIDataInfo>()
        {
            new UIDataInfo(UIType.Loading,"common/LoadingPanel",typeof(UIWindow)),
            new UIDataInfo(UIType.Login,"common/LoginPanel",typeof(UIWindow)),
            new UIDataInfo(UIType.Main,"MainPanel",typeof(UIWindow)),
        };

    }

}
