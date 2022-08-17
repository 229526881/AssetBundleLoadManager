using Data;
using SFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameLogic : MonoBehaviour
{
    /// <summary>
    /// 资源管理单例对象(快速访问)
    /// </summary>
    private TResource.ResourceModuleManager mRMM;

    private AudioSource mBGMAudioSource;

    //原生消息展示
    public Text m_TxtNative;

    private void Awake()
    {
        DontDestroyOnLoad(this);

        // DontDestroyOnLoad(UIRoot);

        initSingletons();

        addMonoComponents();

        nativeInitilization();
         
        initilization();
    }

    /// <summary>
    /// 原生初始化
    /// </summary>
    private void nativeInitilization()
    {
        NativeManager.Singleton.init();
    }

    /// <summary>
    /// 初始化单例
    /// </summary>
    private void initSingletons()
    {
        // 因为SingletonTemplate采用的是惰性初始化(即第一次调用的时候初始化)
        // 会造成单例构造函数无法一开始被触发的问题
        AtlasManager.Singleton.startUp();
    }

    /// <summary>
    /// 初始化
    /// </summary>
    private void initilization()
    {
        mRMM = TResource.ResourceModuleManager.Singleton;

        // 资源模块初始化
        mRMM.init();

        //热更模块初始化
        HotUpdateModuleManager.Singleton.init();

        // 预加载Shader
        //ResourceManager.Singleton.loadAllShader("shaderlist", () =>
        //{

        //},
        //ResourceLoadType.PermanentLoad);

        //初始化版本信息
        // VersionConfigModuleManager.Singleton.initVerisonConfigData();

        //初始化表格数据读取，表格也是需要更新结束后才会读取，除非是不会更新的部分
        GameDataManager.Singleton.loadAll();


        // 初始化逻辑层Manager
        GameSceneManager.Singleton.init();

        mBGMAudioSource = GetComponent<AudioSource>();

        //获取服务器信息
        onObtainServerVersionConfig();
    }

    /// <summary>
    /// 增加Mono模块
    /// </summary>
    private void addMonoComponents()
    {
        gameObject.AddComponent<CoroutineManager>();
        //接收原生层的消息处理
        gameObject.AddComponent<NativeMessageHandler>();
        NativeMessageHandler.Singleton.TxtNativeOutput = m_TxtNative;
        UIManager.Singleton.Init(transform.Find("UIRoot") as RectTransform,
            transform.Find("UIRoot/WndRoot") as RectTransform, transform.Find("UIRoot/UICamera").GetComponent<Camera>(),
            transform.Find("UIRoot/EventSystem").GetComponent<EventSystem>());
    }

    /// <summary>
    /// 获取服务器版本信息
    /// </summary>
    public void onObtainServerVersionConfig()
    {
        DIYLog.Log("onObtainServerVersionConfig()");
        HotUpdateModuleManager.Singleton.doObtainServerVersionConfig(serverVersionConfigHotUpdateCompleteCallBack);
    }

    /// <summary>
    /// 获取服务器版本信息回调
    /// </summary>
    /// <param name="result"></param>
    private void serverVersionConfigHotUpdateCompleteCallBack(bool result)
    {
        DIYLog.Log(string.Format("获取服务器版本结果 result : {0}", result));
        if (!result) return;
        HotUpdateFullWorkFlow();
    }

    /// <summary>
    /// 测试热更新完整流程,强更目前只支持安卓，IOS只能使用热更逻辑
    /// </summary>
    public void HotUpdateFullWorkFlow()
    {
        DIYLog.Log("onTestHotUpdateFullWorkFlow()");
        VersionConfigModuleManager.Singleton.initVerisonConfigData();
        //检测是否强更过版本
        HotUpdateModuleManager.Singleton.checkHasVersionHotUpdate();
        //TODO:
        //拉去服务器列表信息(网络那一套待开发,暂时用本地默认数值测试)
        if (HotUpdateModuleManager.Singleton.checkVersionHotUpdate(HotUpdateModuleManager.Singleton.ServerVersionConfig.VersionCode))
        {
            HotUpdateModuleManager.Singleton.doNewVersionHotUpdate(
                HotUpdateModuleManager.Singleton.ServerVersionConfig.VersionCode,
                (versionhotupdateresult) =>

                {
                    if (versionhotupdateresult)
                    {
                        DIYLog.Log("版本强更完成!触发自动安装！");
#if UNITY_ANDROID
                        (NativeManager.Singleton as AndroidNativeManager).InstallApk(HotUpdateModuleManager.Singleton.VersionHotUpdateCacheFilePath);
#endif
                        return;
                    }
                    else
                    {
                        resourceHotUpdate();
                    }
                }
            );
        }
        else
        {
            resourceHotUpdate();
        }
    }

    /// <summary>
    /// 热更逻辑
    /// </summary>
    private void resourceHotUpdate()
    {
        //不需要强更走后判定资源热更流程
        if (HotUpdateModuleManager.Singleton.checkResourceHotUpdate(HotUpdateModuleManager.Singleton.ServerVersionConfig.ResourceVersionCode))
        {
            //单独开启一个携程打印热更进度
            StartCoroutine(printVersionHotUpdateProgressCoroutine());
            HotUpdateModuleManager.Singleton.doResourceHotUpdate(
                HotUpdateModuleManager.Singleton.ServerVersionConfig.ResourceVersionCode,
                (resourcehotupdateresult) =>
                {
                    if (resourcehotupdateresult)
                    {
                        DIYLog.Log("资源热更完成!请重进或重新触发热更流程！");
                        //热更游戏完成需要重进游戏吗，这个逻辑思考一下
                        OnHotUpdateEnd();
                        return;
                    }
                    else
                    {
                        DIYLog.Log("资源热更出错!");
                        return;
                    }
                }
            );
        }
        else
        {
            OnHotUpdateEnd();
            DIYLog.Log("无需资源热更，可以直接进入游戏！");
        }
    }

    /// <summary>
    /// 打印热更进度，优化为一个特殊的界面处理
    /// </summary>
    /// <returns></returns>
    private IEnumerator printVersionHotUpdateProgressCoroutine()
    {
        Debug.Log("printVersionHotUpdateProgressCoroutine()");
        while (HotUpdateModuleManager.Singleton.HotVersionUpdateRequest.TWRequestStatus == TWebRequest.TWebRequestStatus.TW_In_Progress)
        {
            yield return new WaitForSeconds(1.0f);
            Debug.Log(string.Format("当前版本热更进度 : {0}", HotUpdateModuleManager.Singleton.HotResourceUpdateProgress));
            //可以在面板上显示出来
        }
    }

    public void OnHotUpdateEnd()
    {
        ILRuntimeManager.Singleton.Init();
        //动态加载热更表格，如果是确定不热更的表格则不更新逻辑
        //ILRuntimeManager.Singleton.InvokeMethodNoGC("Data.GameDataManager", "loadAll", null);
        //这里使用更新使用表格数据得是热更端传递而来，如果是不进入更新的表格则在游戏本体使用
        UIManager.Singleton.OpenWindow(UIType.Login, false, null);//逻辑走到这里，可以接入 热更的相应逻辑思考下如何做
        //卡在表格里
    }

    private void Update()
    {
        TResource.ResourceModuleManager.Singleton.Update();

        UIManager.Singleton.OnUpdate();
    }
}
