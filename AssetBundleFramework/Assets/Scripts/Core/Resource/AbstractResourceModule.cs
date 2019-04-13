/*
 * Description:             AbstractResourceModule.cs
 * Author:                  TONYTANG
 * Create Date:             2019//04/07
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AbstractResourceModule.cs
/// ��Դ����ģʽģ�����(�������ֲ�ͬ����Դ����ģʽ e.g. AssetBundle || AssetDatabase)
/// </summary>
public abstract class AbstractResourceModule {

    /// <summary> �߼�����Դ�������ί�� /// </summary>
    /// <param name="abinfo"></param>
    public delegate void LoadResourceCompleteHandler(AbstractResourceInfo abinfo);

    /// <summary>
    /// ��Դ����ģʽ
    /// </summary>
    public ResourceLoadMode ResLoadMode
    {
        get;
        protected set;
    }

    /// <summary>
    /// �Ƿ�����Դ���ռ��(��Щ����²��ʺ�Ƶ�����մ���������ս������)
    /// </summary>
    public bool EnableResourceRecyclingUnloadUnsed
    {
        get;
        set;
    }

    /// <summary>
    /// ��Դ������ӳ��Map(��Dictionaryֻ��Ϊ�˿��ٷ���)
    /// KeyΪ��Դ����ValueҲΪ��Դ��
    /// �ڰ����������Դ��ֻҪ�ϲ���ظ���Դ��һ����PreLoad��ʽ������Դ
    /// ������Ҫ��Ϊ�˱�����Ϊ�ϲ��߼����뵼�´������Դ��Ƶ������ж��
    /// ����: Res.cs    public bool SetImageSprite(UnityEngine.UI.Image image, string spName)
    /// </summary>
    protected Dictionary<string, string> mResourceWhileListMap;

    /// <summary>
    /// �����Ѽ��ص���Դ����Ϣӳ��Map
    /// KeyΪAB�������ͣ�ValueΪ�ü��������Ѿ����ص���Դ��Ϣӳ��Map(KeyΪAB���֣�ValueΪ��Դ������Ϣ)
    /// </summary>
    protected Dictionary<ResourceLoadType, Dictionary<string, AbstractResourceInfo>> mAllLoadedResourceInfoMap;

    #region FSP���㲿��
    /// <summary>
    /// ��ǰFPS
    /// </summary>
    public int CurrentFPS
    {
        get
        {
            return mCurrentFPS;
        }
    }
    protected int mCurrentFPS;

    /// <summary>
    /// ������ʱ��
    /// </summary>
    private float mTotalDeltaTime;

    /// <summary>
    /// ������֡��
    /// </summary>
    private int mFrameCount;

    /// <summary>
    /// FPS���¼��Ƶ��
    /// </summary>
    private float mFPSUpdateInterval;
    #endregion

    /// <summary>
    /// ��Դ����ģ���ʼ��
    /// </summary>
    public virtual void init()
    {
        ResLoadMode = ResourceLoadMode.Invalide;
        EnableResourceRecyclingUnloadUnsed = true;
        mResourceWhileListMap = new Dictionary<string, string>();
        mAllLoadedResourceInfoMap = new Dictionary<ResourceLoadType, Dictionary<string, AbstractResourceInfo>>();
        mAllLoadedResourceInfoMap.Add(ResourceLoadType.NormalLoad, new Dictionary<string, AbstractResourceInfo>());
        mAllLoadedResourceInfoMap.Add(ResourceLoadType.Preload, new Dictionary<string, AbstractResourceInfo>());
        mAllLoadedResourceInfoMap.Add(ResourceLoadType.PermanentLoad, new Dictionary<string, AbstractResourceInfo>());
        mFPSUpdateInterval = 1.0f;
    }

    /// <summary>
    /// ������Դ����ʹ�û��ռ��
    /// </summary>
    public void startResourceRecyclingTask()
    {
        CoroutineManager.Singleton.startCoroutine(checkUnsedResource());
    }

    /// <summary>
    /// �����ﲻ������Դ��Ҫ����ʱ��鲻��ʹ�õ���Դ
    /// </summary>
    protected abstract IEnumerator checkUnsedResource();

    /// <summary>
    /// ���ָ����Դ��������
    /// Note:
    /// Ĭ�ϰ����������Դ����ResourceLoadType.Preload��ʽ����
    /// </summary>
    /// <param name="resname">��Դ��(��AB��)</param>
    public void addToWhiteList(string resname)
    {
        if (!mResourceWhileListMap.ContainsKey(resname))
        {
            mResourceWhileListMap.Add(resname, resname);
        }
        else
        {
            ResourceLogger.logErr(string.Format("��Դ : {0}�ظ���Ӱ�����!", resname));
        }
    }

    /// <summary>
    /// ������Դ
    /// �ϲ���Դ����ͳһ���
    /// </summary>
    /// <param name="resname">��ԴAB��</param>
    /// <param name="completehandler">��������ϲ�ص�</param>
    /// <param name="loadtype">��Դ��������</param>
    /// <param name="loadmethod">��Դ���ط�ʽ</param>
    public void requstResource(string resname, LoadResourceCompleteHandler completehandler, ResourceLoadType loadtype = ResourceLoadType.NormalLoad, ResourceLoadMethod loadmethod = ResourceLoadMethod.Sync)
    {
        // �ڰ����������Դһ����Ԥ������ʽ���أ�
        // ������Ϊ�ϲ��߼�������غ�Ƶ������ж��
        if (mResourceWhileListMap.ContainsKey(resname))
        {
            loadtype = ResourceLoadType.Preload;
        }

        realRequestResource(resname, completehandler, loadtype, loadmethod);
    }

    /// <summary>
    /// ������������Դ(�ɲ�ͬ����Դģ��ȥʵ��)
    /// </summary>
    /// <param name="resname">��ԴAB��</param>
    /// <param name="completehandler">��������ϲ�ص�</param>
    /// <param name="loadtype">��Դ��������</param>
    /// <param name="loadmethod">��Դ���ط�ʽ</param>
    protected abstract void realRequestResource(string resname, LoadResourceCompleteHandler completehandler, ResourceLoadType loadtype = ResourceLoadType.NormalLoad, ResourceLoadMethod loadmethod = ResourceLoadMethod.Sync);

    /// <summary>
    /// �������
    /// </summary>
    public virtual void Update()
    {
        mTotalDeltaTime += Time.deltaTime;
        mFrameCount++;
        if (mTotalDeltaTime >= mFPSUpdateInterval)
        {
            mCurrentFPS = (int)(mFrameCount / mTotalDeltaTime);
            mTotalDeltaTime = 0f;
            mFrameCount = 0;
        }
    }

    /// <summary>
    /// �ͷſ��ͷŵ�Ԥ������Դ(�ݹ��ж��������ƻ�������)
    /// Note:
    /// �г���ǰ���ã�ȷ������Ԥ������Դ��ȷ�ͷ�
    /// </summary>
    public void unloadAllUnsedPreloadLoadedResources()
    {
        unloadSpecificLoadTypeUnsedResource(ResourceLoadType.Preload);
    }

    /// <summary>
    /// �ṩ���ⲿ�Ĵ���ж�������������ز���ʹ�õ���Դ(�ݹ��ж��������ƻ�������)
    /// Note:
    /// ͬ���ӿڣ�����������Ƚϴ�ֻ�����г���ʱ����ж�غ����һ��
    /// </summary>
    public void unloadAllUnsedNormalLoadedResources()
    {
        unloadSpecificLoadTypeUnsedResource(ResourceLoadType.NormalLoad);
    }

    /// <summary>
    /// ж��ָ�����Ͳ���ʹ�õ���Դ(Note:��֧��ж�س�פ��Դ����)
    /// </summary>
    /// <param name="resourceloadtype">��Դ��������</param>
    protected void unloadSpecificLoadTypeUnsedResource(ResourceLoadType resourceloadtype)
    {
        if (resourceloadtype == ResourceLoadType.PermanentLoad)
        {
            ResourceLogger.logErr("������ж�س�פAB��Դ!");
            return;
        }
        doUnloadSpecificLoadTypeUnsedResource(resourceloadtype);
    }

    /// <summary>
    /// ����ִ����Դж��ָ�����Ͳ���ʹ�õ���Դ�ӿ�
    /// </summary>
    /// <param name="resourceloadtype">��Դ��������</param>
    protected abstract void doUnloadSpecificLoadTypeUnsedResource(ResourceLoadType resourceloadtype);

    /// <summary>
    /// �����Ѽ���AB�ļ�������
    /// </summary>
    /// <param name="resname">��Դ��</param>
    /// <param name="oldloadtype">�ɵļ�������</param>
    /// <param name="newloadtype">�µļ�������</param>
    protected void updateLoadedResourceInfoLoadType(string resname, ResourceLoadType oldloadtype, ResourceLoadType newloadtype)
    {
        if (mAllLoadedResourceInfoMap[oldloadtype].ContainsKey(resname))
        {
            var abi = mAllLoadedResourceInfoMap[oldloadtype][resname];
            mAllLoadedResourceInfoMap[newloadtype].Add(resname, abi);
            mAllLoadedResourceInfoMap[oldloadtype].Remove(resname);
            ResourceLogger.log(string.Format("�Ѽ��ص���Դ : {0}����Դ���� : {1}���µ���Դ���� : {2}��", resname, oldloadtype, newloadtype));
        }
        else
        {
            ResourceLogger.logErr(string.Format("��Դ���� : {0}���Ҳ����Ѽ��ص���Դ : {1}���޷����¸���Դ�ļ������ͣ�", oldloadtype, resname));
        }
    }

    /// <summary>
    /// ��ȡָ���������͵��Ѽ�����Դ��Ϣӳ��Map
    /// </summary>
    /// <param name="loadtype">��Դ��������</param>
    /// <returns></returns>
    public Dictionary<string, AbstractResourceInfo> getSpecificLoadTypeARIMap(ResourceLoadType loadtype)
    {
        if (mAllLoadedResourceInfoMap.ContainsKey(loadtype))
        {
            return mAllLoadedResourceInfoMap[loadtype];
        }
        else
        {
            ResourceLogger.logErr(string.Format("�Ҳ�����Դ���� : {0}���Ѽ���AB��Ϣ!", loadtype));
            return null;
        }
    }

    #region ���Կ�������
    /// <summary>
    /// ��ȡ�����Ѽ��ز����õ�AB����(��������פAB)
    /// </summary>
    public int getNormalUnsedABNumber()
    {
        var unsednumber = 0;
        // �����ղ���ʹ�õ�AB
        foreach (var loadedab in mAllLoadedResourceInfoMap[ResourceLoadType.NormalLoad])
        {
            if (loadedab.Value.IsUnsed)
            {
                unsednumber++;
            }
        }
        return unsednumber;
    }

    /// <summary>
    /// ��ȡԤ�����Ѽ��ز����õ�AB����(��������פAB)
    /// </summary>
    public int getPreloadUnsedABNumber()
    {
        var unsednumber = 0;
        // �����ղ���ʹ�õ�AB
        foreach (var loadedab in mAllLoadedResourceInfoMap[ResourceLoadType.Preload])
        {
            if (loadedab.Value.IsUnsed)
            {
                unsednumber++;
            }
        }
        return unsednumber;
    }

    /// <summary>
    /// ��ӡ��ǰ��Դ����ʹ������Ϣ�Լ���������(������)
    /// </summary>
    public void printAllLoadedResourceOwnersAndRefCount()
    {
        ResourceLogger.log("Normal Loaded AssetDatabase Info:");
        foreach (var adi in mAllLoadedResourceInfoMap[ResourceLoadType.NormalLoad])
        {
            adi.Value.printAllOwnersNameAndRefCount();
        }

        ResourceLogger.log("Preload Loaded AssetDatabase Info:");
        foreach (var adi in mAllLoadedResourceInfoMap[ResourceLoadType.Preload])
        {
            adi.Value.printAllOwnersNameAndRefCount();
        }

        ResourceLogger.log("Permanent Loaded AssetDatabase Info:");
        foreach (var adi in mAllLoadedResourceInfoMap[ResourceLoadType.PermanentLoad])
        {
            adi.Value.printAllOwnersNameAndRefCount();
        }
    }
    #endregion
}