using SFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum UIMsgID
{
    None = 0,
}

//热更版的UI框架
public class UIManager : SingletonTemplate<UIManager>
{

    //UI节点
    public RectTransform m_UiRoot;

    //窗口节点
    public RectTransform m_WndRoot;

    //UI摄像机
    private Camera m_UICamera;

    //EventSystem节点
    private EventSystem m_EventSystem;

    //屏幕的宽高比
    private float m_CanvasRate = 0;
    //界面关闭时调用
    private Action<UIType> m_UICloseDelegate = null;
    //在UIOnopen之前调用
    private Action<UIType,UIType> m_UIOpenBeforeDelegate = null;
    //界面打开成功时调用
    private Action<UIType, UIType> m_UIOpenDelegate = null;


    //所有打开的窗口
    private Dictionary<UIType, UIWindow> m_WindowDic = new Dictionary<UIType, UIWindow>();
    //循环的UI
    private Dictionary<UIType, UIWindow> m_CacheDic = new Dictionary<UIType, UIWindow>();
    //打开的窗口列表
    private List<UIWindow> m_WindowList = new List<UIWindow>();
    //需要打开的窗口信息
    private List<UIInfo> m_WindowInfoList=new List<UIInfo>();
    private Dictionary<int, Transform> m_LayerDic=new Dictionary<int, Transform>();


    private Queue<UIInfo> openQueue=new Queue<UIInfo>();
    private Queue<UIInfo> closeQueue=new Queue<UIInfo>();

    private GameObject uiAnimMask;

    private bool isOpening = false;
    private bool isClosing = false;

    /** 背景UI序号（有若干层UI是作为背景UI，而不受切换等影响）*/
    private int BackGroundUI = 0;

    public void Init(RectTransform uiRoot, RectTransform wndRoot, Camera uiCamera, EventSystem eventSystem)
    {
        m_UiRoot = uiRoot;
        m_WndRoot = wndRoot;
        m_UICamera = uiCamera;
        m_EventSystem = eventSystem;
        m_CanvasRate = Screen.height / (m_UICamera.orthographicSize * 2);
        LayerInit();
    }

    public bool SendMessageToWnd(UIType type, UIMsgID msgID = 0, params object[] paralist)
    {
        UIWindow wnd = FindWndByName<UIWindow>(type);
        if (wnd != null)
        {
            return wnd.OnMessage(msgID, paralist);
        }

        return false;
    }
    public void OpenWindow(UIType type, bool resources = false, object uiArgs = null, Action<UIWindow> endEvent = null)
    {
        //这个逻辑主要是怕界面没生成好，组成开启队列
        UIInfo uiInfo = new UIInfo()
        {
            type = type,
            uiArgs = uiArgs,
            isResources = resources,
            window = null,
            endEvent = endEvent,
        };
        if (isOpening || isClosing)
        {
            DIYLog.LogError(string.Format("open {0} ui action push into queue", Enum.GetValues(typeof(UIType))));
            openQueue.Enqueue(uiInfo);
            return;
        }
        UIDataInfo dataInfo = UIDataInfo.GetUIData(type);
        if (dataInfo == null)
        {
            DIYLog.LogError(string.Format(" {0} 未注册", type));
            return;
        }

        string wndName = dataInfo.Name;
        uiInfo.Name = wndName;
        int index =m_WindowList.FindIndex((item) => { return item.UIType == type; });
        if (index >= 0)
        {
            CloseToUI(uiInfo, index,true);
            return;
        }

        m_WindowInfoList.Add(uiInfo);
        isOpening = true;
        ShowWaitAnim();

        UIWindow wnd = FindWndByName<UIWindow>(type);
        if (wnd == null)
        {
            System.Type tp = null;
            if (resources)
            {
                wnd = System.Activator.CreateInstance(tp) as UIWindow;
            }
            else
            {
                string hotName = "HotFix_Project." + wndName.Replace("Panel", "") + "Window";
                wnd = ILRuntimeManager.Singleton.ILRunAppDomain.Instantiate<UIWindow>(hotName);
                wnd.IsHotFix = true;
                wnd.HotFixClassName = hotName;
            }

            if (wnd == null)
            {
                DIYLog.Log("创建窗口代码失败");
                return;
            }
            wnd.Init(wndName, type, resources, uiArgs);
            if (resources)
            {
                GameObject wndObj = GameObject.Instantiate(Resources.Load<GameObject>(wndName));
                EndCreateWindow(wndObj, wnd, uiInfo, endEvent);
            }
            else
            {
                ResourceManager.Singleton.getPrefabInstance(
                  dataInfo.FullPrefabPath,
                  (prefabInstance, requestUid) =>
                  {
                      EndCreateWindow(prefabInstance, wnd, uiInfo, endEvent);
                  });
            }
        }
        else
        {
            DIYLog.Log("界面已经打开了："+ wnd.Name);
        }
    }
    public void EndCreateWindow(GameObject wndObj, UIWindow wnd, UIInfo uiInfo,Action<UIWindow> endEvent = null)
    {
        HideWaitAnim();
        if (wndObj == null)
        {
            DIYLog.Log("创建窗口Prefab失败：" + wnd.Name);
            return;
        }
        if (!m_WindowDic.ContainsKey(wnd.UIType))
        {
            m_WindowList.Add(wnd);
            m_WindowDic.Add(wnd.UIType, wnd);
        }

        wnd.GameObject = wndObj;
        wnd.Transform = wndObj.transform;
        wnd.PanelInfo = wndObj.GetComponent<UIPanelInfo>();
        uiInfo.window = wnd;
        SetLayer(wndObj.transform, wnd.PanelInfo.layerType);
        wnd.Awake();
        wnd.OnShow();
        OnUIOpenEnd(wnd,  uiInfo, endEvent);
    }


    private void OnUIOpenEnd(UIWindow wnd,UIInfo uiInfo, Action<UIWindow> endEvent = null)
    {
        wnd.GameObject.SetActive(true);
        int zIndex = uiInfo.zIndex >= 0 ? uiInfo.zIndex : m_WindowInfoList.Count;
        wnd.Transform.SetSiblingIndex(zIndex);
        if (wnd.PanelInfo.quickClose)
        {
            //在界面后做一个黑遮罩关闭
            OpenQuickCloseMask(wnd.Transform,wnd.UIType);
        }
        SetLayer(wnd.Transform, wnd.PanelInfo.layerType);
        UpdateUI();

        UIType fromUIID = 0;
        if (m_WindowInfoList.Count > 1)
        {
            fromUIID = m_WindowInfoList[m_WindowInfoList.Count - 2].type;
        }
        m_UIOpenBeforeDelegate?.Invoke(wnd.UIType, fromUIID);

        Action onAniOverFunc = () =>
        {
            wnd.IsPlayOpenAni = false;
            isOpening = false;
            m_UIOpenDelegate?.Invoke(wnd.UIType, fromUIID);
            //遮挡界面逻辑处理
            uiAnimMask?.SetActive(false);
            endEvent?.Invoke(wnd);
            wnd.OnOpenEnd();
            AutoExecNextUI();
        };
        OpenAnimator openAnim = wnd.GameObject.GetComponent<OpenAnimator>();
        Animator anim = wnd.GameObject.GetComponent<Animator>();
        if (anim!=null&& openAnim==null)
        {
            openAnim= wnd.GameObject.AddComponent<OpenAnimator>();
        }
        openAnim?.PlayAnim("In", onAniOverFunc);
        if (openAnim == null)
        {
            onAniOverFunc();
        }
        else
        {
            //防止动画打开过程中被触发点击事件
            OpenAnimMask(uiInfo.zIndex);
        }
    }

    /// <summary>
    /// 关闭界面
    /// </summary>
    /// <param name="uiType"></param>
    /// <param name="playCloseAni">是否播放关闭动画</param>
    public void CloseWindow(UIType uiType, bool playCloseAni = false)
    {
        int uiCount = m_WindowInfoList.Count;
        UIInfo uiInfo;

        if (uiType > UIType._End || uiType < UIType._Start||uiCount<1)
        {
            DIYLog.LogError("unknown ui type to close");
            return;
        }

        uiInfo = m_WindowInfoList.Find((item) => { return item.type == uiType; });
        if ( isClosing || isOpening )
        {
            uiInfo.window.GameObject.SetActive(false);
            closeQueue.Enqueue(uiInfo);
            return;
        }
        DIYLog.Log("close UI:" + uiInfo.Name);

        //删除要关闭的界面信息
        m_WindowInfoList.Remove(uiInfo);
        isClosing = true;

        UIWindow window = uiInfo.window;
        uiInfo.isClose = true;

        if (null == window)
        {
            isClosing = false;
            return;
        }

        UIInfo preUIInfo = null;
        if (uiCount >= 2)
            preUIInfo = m_WindowInfoList[uiCount - 2];

        UpdateUI();

        Action closeCallBack = () =>
        {
            if (null != preUIInfo && null != preUIInfo.window && isTopUI(preUIInfo.type))
            {
                // 如果之前的界面弹到了最上方（中间有可能打开了其他界面）
                preUIInfo.window.GameObject.SetActive(true);
                preUIInfo.window.OnTop(window.UIType);
                window.OnClose();  
            }
            else
            {
                window.OnClose();
            }
            m_UICloseDelegate?.Invoke(window.UIType);

            if (window.IsCache)
            {
                m_CacheDic[uiType] = window;
                window.GameObject.SetActive(false);
                window.Transform.parent = null;
                DIYLog.Log(string.Format("uiView removeFromParent {0}", window.Name));
            }
            else
            {
                window.GameObject.SetActive(false);
                GameObject.Destroy(window.GameObject);
            }

            if (m_WindowDic.ContainsKey(window.UIType))
            {
                m_WindowDic.Remove(window.UIType);
                m_WindowList.Remove(window);
            }
            window.GameObject = null;
            window.Transform = null;
            window = null;
            isClosing = false;
            AutoExecNextUI();
        };

        OpenAnimator closeAnim = window.GameObject.GetComponent<OpenAnimator>();

        if (!playCloseAni || closeAnim == null)
        {
            closeCallBack.Invoke();
        }
        else
        {
            closeAnim?.PlayAnim("Out", closeCallBack);
        }
    }

    private void AutoExecNextUI()
    {
        if (this.openQueue.Count > 0)
        {
            UIInfo uiInfo = openQueue.Dequeue();
            OpenWindow(uiInfo.type, uiInfo.isResources, uiInfo.uiArgs, uiInfo.endEvent);
            return;
        }

        //如果没有要打开的则处理关闭的
        if (closeQueue.Count > 0)
        {
            UIInfo uiInfo = closeQueue.Dequeue();
            CloseWindow(uiInfo.type, false);
        }
    }

    /// <summary>
    /// 窗口的Update逻辑，热更界面和普通界面不同
    /// </summary>
    public void OnUpdate()
    {
        for (int i = 0; i < m_WindowList.Count; i++)
        {
            UIWindow window = m_WindowList[i];
            if (window != null)
            {
                window.OnUpdate();
            }
        }
    }
    
    /// <summary>
    /// 关闭所有窗口
    /// </summary>
    public void CloseAllWindow()
    {
        for (int i = m_WindowList.Count - 1; i >= 0; i--)
        {
            CloseWindow(m_WindowList[i].UIType);
        }
    }

    /// <summary>
    /// 根据窗口名查找窗口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <returns></returns>
    public UIWindow FindWndByName<T>(UIType type) where T:UIWindow
    {
        UIWindow window = null;
        if (m_WindowDic.TryGetValue(type, out window))
        {
            return window;
        }
        return null;
    }

    /// <summary>
    /// 生成界面遮罩，防止穿透到下层界面，只需要给部分界面使用即可
    /// </summary>
    //private void PreventTouch(UIInfo uiInfo)
    //{
    //    int slbIndex = uiInfo.zIndex;
    //    ResourceManager.Instance.getPrefabInstance(
    //      "Assets/_assets/uiassets/uiprefabs/common/UIBack.prefab",
    //      (prefabInstance, requestUid) =>
    //      {
    //          Utility.SetParent(prefabInstance.transform, GetLayer(UILayerType.Top));
    //          prefabInstance.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
    //          SortAnimMaskIndex(slbIndex);
    //          uiInfo.preventNode = prefabInstance;
    //      });
    //}

    private void OpenQuickCloseMask(Transform trans,UIType type)
    {
        Transform back = trans.Find("background");
        if (null == back)
        {
            GameObject go = new GameObject("background");
            go.transform.SetParent(trans);
            back = go.AddComponent<RectTransform>();
        }
        RectTransform rect = (RectTransform)back;
        RectTransformExtensions.SetAnchor(rect, AnchorPresets.StretchAll);
        back.transform.SetAsFirstSibling();
        Image img = back.gameObject.AddComponent<Image>();
        img.color = new Color(1, 1, 1, 0);
        Button btn = back.gameObject.AddComponent<Button>();
        btn.transition = Button.Transition.None;
        btn.onClick.AddListener(() => { CloseWindow(type); });
    }

    /// <summary>
    /// 打开一个遮挡动画事件的遮罩
    /// </summary>
    /// <param name="zIndex"></param>
    private void OpenAnimMask(int slbIndex)
    {
        if (uiAnimMask == null)
        {
            uiAnimMask= ResourceManager.Singleton.GetPrefabInstance("Assets/_assets/uiassets/uiprefabs/common/UIAnimMask.prefab");
            Utility.SetParent( GetLayer(UILayerType.Top), uiAnimMask.transform);
            uiAnimMask.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
        }
        SortAnimMaskIndex(slbIndex);
    }
     
    private void SortAnimMaskIndex(int slbIndex)
    {
        uiAnimMask.transform.SetSiblingIndex(slbIndex + 1); //关于层级的问题
        uiAnimMask.SetActive(true);
    }

    #region 层级相关
    private void LayerInit()
    {
        if (m_LayerDic!=null&& m_LayerDic.Count >= Enum.GetNames(typeof(UILayerType)).Length) return;
        //根据层级的种类初始化字典
        int nums = Enum.GetNames(typeof(UILayerType)).Length;
        for (int i = 0; i < nums; i++)
        {
            object obj = Enum.GetValues(typeof(UILayerType)).GetValue(i);
            int key = System.Convert.ToInt32(obj);
            if (m_LayerDic.ContainsKey(key))
            {
                if (m_LayerDic[key] == null)
                    m_LayerDic[key] = CreateLayerGameObject(obj.ToString(), (UILayerType)obj);
            }
            else
                m_LayerDic.Add(key, CreateLayerGameObject(obj.ToString(), (UILayerType)obj));
        }
    }

    /// <summary>
    /// 创建各个UI层的节点                                                                                                                                                           
    /// </summary>
    /// <param name="name"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    private Transform CreateLayerGameObject(string name, UILayerType type)
    {
        //首先判断是否有该节点,找根节点
        Transform layer = m_WndRoot.Find(name);
        if (!layer)
        {
            layer = new GameObject(name).transform;
            Utility.SetParent(m_WndRoot, layer); ;
            //可以通过改变Z来更好的控制层级，层级越大，负值越大
            RectTransform back = layer.gameObject.AddComponent<RectTransform>();
            RectTransform rect = (RectTransform)back;
            RectTransformExtensions.SetAnchor(rect, AnchorPresets.StretchAll,0,0);
            rect.sizeDelta = Vector4.zero;
            layer = rect;
            //layer.transform.position = new Vector3(0, 0, ((int)type) * -1);
            //layer.transform.localScale = Vector3.one;
        }
        return layer;
    }

    private void SetLayer(Transform current, UILayerType layer)
    {
        if (m_LayerDic==null||m_LayerDic.Count < Enum.GetNames(typeof(UILayerType)).Length)
        {
            LayerInit();
        }
        Transform  layerobj = null;
        int depth = (int)layer;
        m_LayerDic.TryGetValue(depth, out layerobj);
        Utility.SetParent(layerobj, current);
        //NGUI需要设置层级比所有的高，UGUI不需要
        current.GetComponent<RectTransform>().sizeDelta = Vector4.zero;
        Utility.GetMaxDepth(layerobj, out depth);
        current.transform.SetSiblingIndex(depth);
    }

    private Transform GetLayer(UILayerType layerType)
    {
        Transform res = null;
        m_LayerDic.TryGetValue((int)layerType, out res);
        return res;
    }
    #endregion

    private void CloseToUI(UIInfo info,int index , bool bOpenSelf = true)
    {
        index = bOpenSelf ? index : index++;
        int idx = 0;
        while (idx++ < index)
        {
            UIInfo tempInfo = m_WindowInfoList[m_WindowInfoList.Count - 1];
            m_WindowInfoList.RemoveAt(m_WindowInfoList.Count - 1);

            UIType uiType = tempInfo.type;
            UIWindow window = tempInfo.window;
            tempInfo.isClose = true;

            m_UICloseDelegate?.Invoke(window.UIType);
            if (null != window)
            {
                if (window.IsCache)
                {
                    m_CacheDic[window.UIType] = window;
                    window.Transform.parent = null;
                    window.OnClose();
                }
                else
                {
                    //否则要回收窗口做处理,思考一下ab回收的情况
                    GameObject.Destroy(window.GameObject);
                }
                m_WindowList.Remove(window);
            }
        }

        UpdateUI();
        openQueue.Clear();
        if (bOpenSelf)
        {
            OpenWindow(info.type, info.isResources, info.uiArgs, info.endEvent);
        }
    }

    /// <summary>
    /// 根据界面显示类型刷新显示
    /// </summary>
    private void UpdateUI()
    {
        int hideIndex = 0, showIndex = m_WindowInfoList.Count - 1;
        for (; showIndex >= 0; showIndex--)
        {
            UIInfo uiInfo = m_WindowInfoList[showIndex];
            var showType = uiInfo.window.PanelInfo.showType;
            uiInfo.window.GameObject.SetActive(true);
            //显示全屏，则不需要打开前面的界面
            if (showType == UIShowTypes.UIFullScreen)
            {
                break;
            }
            else if (showType == UIShowTypes.UISingle)
            {
                //如果是单层界面，只需要展示固定的背景界面和该单层界面
                for (int i = 0; i < BackGroundUI; ++i)
                {
                    if (null != m_WindowInfoList[i])
                    {
                        m_WindowInfoList[i].window.GameObject.SetActive(true);
                    }
                }
                hideIndex = BackGroundUI;
                break;
            }
        }
        for (int i = hideIndex; i < showIndex; i++)
        {
            m_WindowInfoList[i].window.GameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 播放界面加载动画
    /// </summary>
    private void ShowWaitAnim()
    {
        
    }

    /// <summary>
    /// 隐藏界面加载动画
    /// </summary>
    private void HideWaitAnim()
    {
        
    }

    private bool isTopUI(UIType type)
    {
        int count = m_WindowInfoList.Count;
        if (count == 0) return false;
        return m_WindowInfoList[count - 1].type == type;
    }

}

public class UIInfo
{
    public UIType type;
    public object uiArgs;
    public bool isResources = false;
    public UIWindow window;
    public Action<UIWindow> endEvent;
    public string Name;
    //public GameObject preventNode;
    public bool isClose;
    public int zIndex = -1;
}