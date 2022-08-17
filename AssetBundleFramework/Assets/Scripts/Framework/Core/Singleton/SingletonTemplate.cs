using UnityEngine;
using System.Collections;

/// <summary>
/// 模板单例
/// </summary>
/// <typeparam name="T"></typeparam>
public class SingletonTemplate<T> where T : class, new()
{
    private static readonly object _lock = new object();
    private static T mInstance = null;
    //Singleton
    public static T Singleton
    {
        get
        {
            Initialize();
            return mInstance;
        }
    }

    protected SingletonTemplate()
    {

    }

    private static void Initialize()
    {
        if (mInstance == null)
        {
            //使用单向锁防止重复创建同一单例
            lock (_lock)
            {
                if (mInstance == null)
                {
                    //_instance = new T();
                    //使用反射构造类的实例
                    mInstance = System.Activator.CreateInstance<T>();
                }
            }
        }
    }


    /// <summary>
    /// 提供一个方法触发第一次构造函数调用
    /// </summary>
    public void startUp()
    {

    }
}
