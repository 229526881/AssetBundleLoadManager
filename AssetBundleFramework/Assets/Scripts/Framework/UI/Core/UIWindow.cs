using SFramework;
using System.Collections;
using System.Collections.Generic;
using TUI;
using UnityEngine;
using UnityEngine.UI;

public class UIWindow 
{
    //引用GameObject
    public GameObject GameObject { get; set; }

    //引用Transform
    public Transform Transform { get; set; }
    
    //挂载的界面组件信息
    public UIPanelInfo PanelInfo { get; set; }
    //界面类型
    public UIType UIType { get; set; }
    //界面传递的数据
    public object UIArgs { get; set; }
    //名字
    public string Name { get; set; }

    //是否从resources加载
    public bool Resource { get; set; } = false;
    //是否热更
    public bool IsHotFix { get; set; } = false;
    //热更类名 
    public string HotFixClassName { get; set; }
    //是否循环使用
    public bool IsCache { get; set; }
    //是否有打开动画
    public bool IsPlayOpenAni { get; set; }
    //所有的Button
    protected List<TButton> m_AllButton = new List<TButton>();

    //所有Toggle
    protected List<Toggle> m_AllToggle = new List<Toggle>();


    public virtual bool OnMessage(UIMsgID msgID, params object[] paralist)
    {
        return true;
    }

    public virtual void Init(string name,UIType type,bool resources,object uiArgs)
    {
        Name = name;
        UIType = type;
        Resource = resources;
        UIArgs = uiArgs;
    }

    public virtual void Awake()
    {
    }

    public virtual void OnShow()
    {
        Debug.Log("base Onshow");
    }


    /// <summary>
    /// 关闭其他界面而导致成为顶部界面
    /// </summary>
    /// <param name="type">关闭的界面</param>
    public virtual void OnTop(UIType type)
    {

    }

    public virtual void OnOpenEnd()
    {
    }

    public virtual void OnDisable()
    {
    }

    public virtual void OnUpdate()
    {
    }

    public virtual void OnClose()
    {
        RemoveAllButtonListener();
        RemoveAllToggleListener();
        m_AllButton.Clear();
        m_AllToggle.Clear();
    }

    public void CloseSelf()
    {
        UIManager.Singleton.CloseWindow(UIType);
    }

    /// <summary>
    /// 同步替换图片
    /// </summary>
    /// <param name="path"></param>
    /// <param name="image"></param>
    /// <param name="setNativeSize"></param>
    /// <returns></returns>
    public void ChangeImageSprite(string path, Image image, bool setNativeSize = false)
    {
        if (image == null)
            return;

        AtlasManager.Singleton.setImageSingleSprite(image, path,(sprite, id) => {
            if (setNativeSize)
            {
                image.SetNativeSize();
            }
        });
    }

    /// <summary>
    /// 异步替换图片
    /// </summary>
    /// <param name="path"></param>
    /// <param name="image"></param>
    /// <param name="setNativeSize"></param>
    public void ChangImageSpriteAsync(string path, Image image, bool setNativeSize = false)
    {
        if (image == null)
            return;
        TResource.AssetLoader assetLoader;
        AtlasManager.Singleton.setImageSingleSpriteAsync(image, path,out assetLoader, (sprite, id) =>
        {
            if (setNativeSize)
            {
                image.SetNativeSize();
            }
        });
    }

    /// <summary>
    /// 移除所有的button事件
    /// </summary>
    public void RemoveAllButtonListener()
    {
        foreach (Button btn in m_AllButton)
        {
            btn.onClick.RemoveAllListeners();
        }
    }

    /// <summary>
    /// 移除所有的toggle事件
    /// </summary>
    public void RemoveAllToggleListener()
    {
        foreach (Toggle toggle in m_AllToggle)
        {
            toggle.onValueChanged.RemoveAllListeners();
        }
    }

    /// <summary>
    /// 添加button事件监听
    /// </summary>
    /// <param name="btn"></param>
    /// <param name="action"></param>
    public void AddButtonClickListener(TButton btn, UnityEngine.Events.UnityAction action)
    {
        if (btn != null)
        {
            if (!m_AllButton.Contains(btn))
            {
                m_AllButton.Add(btn);
            }

            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(action);
        }
    }

    /// <summary>
    /// Toggle事件监听
    /// </summary>
    /// <param name="toggle"></param>
    /// <param name="action"></param>
    public void AddToggleClickListener(Toggle toggle, UnityEngine.Events.UnityAction<bool> action)
    {
        if (toggle != null)
        {
            if (!m_AllToggle.Contains(toggle))
            {
                m_AllToggle.Add(toggle);
            }

            toggle.onValueChanged.RemoveAllListeners();
            toggle.onValueChanged.AddListener(action);
            toggle.onValueChanged.AddListener(TogglePlaySound);
        }
    }


    /// <summary>
    /// 播放toggle声音
    /// </summary>
    /// <param name="isOn"></param>
    void TogglePlaySound(bool isOn)
    {

    }
}
