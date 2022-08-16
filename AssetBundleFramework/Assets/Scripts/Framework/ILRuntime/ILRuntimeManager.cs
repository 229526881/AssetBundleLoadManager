using ILRuntime.CLR.Method;
using ILRuntime.CLR.TypeSystem;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.Runtime.Stack;
using ILRuntimeDemo;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using AppDomain = ILRuntime.Runtime.Enviorment.AppDomain;

public class ILRuntimeManager : SingletonTemplate<ILRuntimeManager>
{
    private const string DLLPATH = "Assets/_assets/hotfix/HotFix_Project.dll.bytes";
    private const string PDBPATH = "Assets/_assets/hotfix/HotFix_Project.pdb.bytes";

    private AppDomain m_AppDomain;

    private bool m_IsGetAB = false;

    public AppDomain ILRunAppDomain
    {
        get { return m_AppDomain; }
    }

    /// <summary>
    /// 初始ILRuntime的内容
    /// </summary>
    public void Init()
    {
        m_AppDomain = new ILRuntime.Runtime.Enviorment.AppDomain();
        TextAsset dllText, dllpdbText;
#if UNITY_EDITOR
        //直接读资源中的
        if (m_IsGetAB)
        {
            //读ab中的测试热更
            dllText = ResourceManager.Singleton.LoadAssetSync<TextAsset>(null, DLLPATH);
            dllpdbText = ResourceManager.Singleton.LoadAssetSync<TextAsset>(null, PDBPATH);
        }
        else
        {
            dllText = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>(DLLPATH);
            dllpdbText = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>(PDBPATH);
        }
#else
        dllText = ResourceManager.Singleton.LoadAssetSync<TextAsset>(null, DLLPATH);
        dllpdbText = ResourceManager.Singleton.LoadAssetSync<TextAsset>(null, PDBPATH);
#endif
        MemoryStream ms = new MemoryStream(dllText.bytes);
        MemoryStream pdb = new MemoryStream(dllpdbText.bytes);
        m_AppDomain.LoadAssembly(ms, pdb, new ILRuntime.Mono.Cecil.Pdb.PdbReaderProvider());
        InitializeIlRuntime();
    }

    void InitializeIlRuntime()
    {

#if DEBUG && (UNITY_EDITOR || UNITY_ANDROID || UNITY_IPHONE)
        //由于Unity的Profiler接口只允许在主线程使用，为了避免出异常，需要告诉ILRuntime主线程的线程ID才能正确将函数运行耗时报告给Profiler
        m_AppDomain.UnityMainThreadID = System.Threading.Thread.CurrentThread.ManagedThreadId;
#endif
        //开启调试功能,热更工程在这之后attach 即可进入调试
        m_AppDomain.DebugService.StartDebugService(56001);
        m_AppDomain.DelegateManager.RegisterMethodDelegate<byte[]>();
        m_AppDomain.DelegateManager.RegisterMethodDelegate<int>();
        m_AppDomain.DelegateManager.RegisterMethodDelegate<int,int>();
        m_AppDomain.DelegateManager.RegisterFunctionDelegate<byte[], ILRuntime.Runtime.Intepreter.ILTypeInstance>();

        m_AppDomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction>((act) =>
        {
            return new UnityEngine.Events.UnityAction(() =>
            {
                ((Action)act)();
            });
        });

  


        InitILRuntime(m_AppDomain);
      //  ILRuntime.Runtime.Generated.CLRBindings.Initialize(m_AppDomain);
    }

    void OnHotFixLoaded()
    {

    }

    /// <summary>
    /// 注册类适配器，减少GC
    /// </summary>
    public void InitILRuntime(ILRuntime.Runtime.Enviorment.AppDomain domain)
    {
        //domain.RegisterCrossBindingAdaptor(new MonoBehaviourAdapter());
        //domain.RegisterCrossBindingAdaptor(new CoroutineAdapter());
        domain.RegisterCrossBindingAdaptor(new UIWindowAdapter());
    }



    //unsafe void SetUpCLRBinding()
    //{
    //    var arr = typeof(GameObject).GetMethods();
    //    foreach (var i in arr)
    //    {
    //        if (i.Name == "GetCompontent" && i.GetGenericArguments().Length == 1)
    //        {
    //            m_AppDomain.RegisterCLRMethodRedirection(i, GetCompontent);
    //        }

    //        if (i.Name == "AddComponent" && i.GetGenericArguments().Length == 1)
    //        {
    //            m_AppDomain.RegisterCLRMethodRedirection(i, AddCompontent);
    //        }
    //    }
    //}

    //private unsafe StackObject* AddCompontent(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack,
    //   CLRMethod __method, bool isNewObj)
    //{
    //    ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;

    //    var ptr = __esp - 1;
    //    GameObject instance = StackObject.ToObject(ptr, __domain, __mStack) as GameObject;
    //    if (instance == null)
    //    {
    //        throw new System.NullReferenceException();
    //    }

    //    __intp.Free(ptr);

    //    var genericArgument = __method.GenericArguments;
    //    if (genericArgument != null && genericArgument.Length == 1)
    //    {
    //        var type = genericArgument[0];
    //        object res;
    //        if (type is CLRType) //CLRType表示这个类型是Unity工程里的类型   //ILType表示是热更dll里面的类型
    //        {
    //            //Unity主工程的类，不需要做处理
    //            res = instance.AddComponent(type.TypeForCLR);
    //        }
    //        else
    //        {
    //            //创建出来MonoTest
    //            var ilInstance = new ILTypeInstance(type as ILType, false);
    //            var clrInstance = instance.AddComponent<MonoBehaviourAdapter.Adaptor>();
    //            clrInstance.ILInstance = ilInstance;
    //            clrInstance.AppDomain = __domain;
    //            //这个实例默认创建的CLRInstance不是通过AddCompontent出来的有效实例，所以要替换
    //            ilInstance.CLRInstance = clrInstance;

    //            res = clrInstance.ILInstance;

    //            //补掉Awake
    //            clrInstance.Awake();
    //        }

    //        return ILIntepreter.PushObject(ptr, __mStack, res);
    //    }

    //    return __esp;
    //}

    //private unsafe StackObject* GetCompontent(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack,
    //  CLRMethod __method, bool isNewObj)
    //{
    //    ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;

    //    var ptr = __esp - 1;
    //    GameObject instance = StackObject.ToObject(ptr, __domain, __mStack) as GameObject;
    //    if (instance == null)
    //        throw new System.NullReferenceException();

    //    __intp.Free(ptr);

    //    var genericArgument = __method.GenericArguments;
    //    if (genericArgument != null && genericArgument.Length == 1)
    //    {
    //        var type = genericArgument[0];
    //        object res = null;
    //        if (type is CLRType)
    //        {
    //            res = instance.GetComponent(type.TypeForCLR);
    //        }
    //        else
    //        {
    //            var clrInstances = instance.GetComponents<MonoBehaviourAdapter.Adaptor>();
    //            foreach (var clrInstance in clrInstances)
    //            {
    //                if (clrInstance.ILInstance != null)
    //                {
    //                    if (clrInstance.ILInstance.Type == type)
    //                    {
    //                        res = clrInstance.ILInstance;
    //                        break;
    //                    }
    //                }
    //            }
    //        }

    //        return ILIntepreter.PushObject(ptr, __mStack, res);
    //    }

    //    return __esp;
    //}

    /// <summary>
    /// 处理无返回值且无GC的方法,且无参数
    /// </summary>
    /// <param name="className"></param>
    /// <param name="methodName"></param>
    /// <param name="paramNum"></param>
    /// <param name="param"></param>
    public void InvokeMethodNoGC(string className, string methodName, object obj)
    {
        IType type = m_AppDomain.LoadedTypes[className];
        if(obj==null) obj = ((ILType)type).Instantiate(); ;
        IMethod method = type.GetMethod(methodName, 0);
        using (var ctx = m_AppDomain.BeginInvoke(method))
        {
            ctx.PushObject(obj);
            ctx.Invoke();
        }
    }

}
