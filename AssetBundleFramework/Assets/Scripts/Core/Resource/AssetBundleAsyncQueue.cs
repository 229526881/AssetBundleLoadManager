/*
 * Description:             AssetBundleAsyncQueue.cs
 * Author:                  TONYTANG
 * Create Date:             2019//04/02
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AssetBundleAsyncQueue.cs
/// AB�첽���ض���
/// Ŀ�ģ�
/// �Ż�ԭ����ÿһ���첽AB���ض���һ��Я�̵�����
/// �ĳ��޶�AB����Я��������ģ����е���ʽ����AB�첽����
/// </summary>
public class AssetBundleAsyncQueue {

    /// <summary>
    /// �첽AB���ض���(ȫ��Ψһ)
    /// </summary>
    public static Queue<AssetBundleLoader> ABAsyncQueue = new Queue<AssetBundleLoader>();

    /// <summary>
    /// �Ƿ�����AB��������Я��
    /// </summary>
    public bool IsLoadStart
    {
        get;
        private set;
    }

    /// <summary>
    /// ��ǰ���ڼ��ص�AB������
    /// </summary>
    public AssetBundleLoader CurrentLoadingAssetBundleLoader
    {
        get;
        private set;
    }
    
    public AssetBundleAsyncQueue()
    {
        IsLoadStart = false;
    }

    /// <summary>
    /// ����AB�첽��������Я��
    /// </summary>
    public void startABAsyncLoad()
    {
        if(IsLoadStart == false)
        {
            ModuleManager.Singleton.getModule<ResourceModuleManager>().StartCoroutine(assetBundleLoadAsync());
            IsLoadStart = true;
        }
        else
        {
            ResourceLogger.logErr("AB�첽��������Я���Ѿ������������ظ�������");
        }
    }

    /// <summary>
    /// �첽�������������
    /// </summary>
    /// <param name="abl"></param>
    public static void enqueue(AssetBundleLoader abl)
    {
        if(abl.LoadMethod == ABLoadMethod.Async)
        {
            ABAsyncQueue.Enqueue(abl);
        }
        else
        {
            ResourceLogger.logErr(string.Format("���ش���ͬ��������Դ : {0} ��Ӧ����ӵ��첽���ض����", abl.ABName));
        }
    }

    /// <summary>
    /// AB����Я��
    /// </summary>
    /// <returns></returns>
    private IEnumerator assetBundleLoadAsync()
    {
        while (true)
        {
            if (ABAsyncQueue.Count > 0)
            {
                CurrentLoadingAssetBundleLoader = ABAsyncQueue.Dequeue();
                CurrentLoadingAssetBundleLoader.LoadState = ABLoadState.Loading;
                var abpath = AssetBundlePath.GetABPath() + CurrentLoadingAssetBundleLoader.ABName;
                AssetBundleCreateRequest abrequest = null;
#if UNITY_EDITOR
                //��Ϊ��Դ��ȫ���ܶ���Դ��ʧ������ֱ�ӱ���
                //������ʱ����Editorģʽ���ж����ļ��Ƿ���ڣ�����AssetBundle.LoadFromFile()ֱ�ӱ���
                if (System.IO.File.Exists(abpath))
                {
                    abrequest = AssetBundle.LoadFromFileAsync(abpath);
                }
                else
                {
                    Debug.LogError(string.Format("AB : {0}�ļ������ڣ�", CurrentLoadingAssetBundleLoader.ABName));
                }
#else
                abrequest = AssetBundle.LoadFromFileAsync(abpath);
#endif
                yield return abrequest;
                var assetbundle = abrequest.assetBundle;
                if (assetbundle == null)
                {
                    ResourceLogger.logErr(string.Format("Failed to load AssetBundle : {0}!", CurrentLoadingAssetBundleLoader.ABName));
                }

                CurrentLoadingAssetBundleLoader.onSelfABLoadComplete(assetbundle);
                CurrentLoadingAssetBundleLoader = null;
            }
            else
            {
                yield return null;
            }
        }
    }
}