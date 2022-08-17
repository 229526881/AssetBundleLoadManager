using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SFramework;

//放置脚本的参数
public class UIPanelInfo : MonoBehaviour
{
    [Tooltip("界面的层级")]
    public UILayerType layerType;

    [Tooltip("是否缓存")]
    public bool isCache = false;

    [Tooltip("遮罩透明度")]
    public int maskOpacity = 255;

    [Tooltip("是否能快速关闭")]
    public bool quickClose = false;

    [Tooltip("屏幕上显示的方式")]
    public UIShowTypes showType = UIShowTypes.UISingle;

}
