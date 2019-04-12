/*
 * Description:             AssetDatabaseModule.cs
 * Author:                  TONYTANG
 * Create Date:             2019//04/07
 */

#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AssetDatabaseModule.cs
/// AssetDatabase��Դ����ģ��
/// </summary>
public class AssetDatabaseModule : AbstractResourceModule
{
    #region ��Դ���ع�����
    /// <summary>
    /// ��Դ��������ӳ��map
    /// KeyΪ��Դ����ValueΪ��Դ�����������
    /// </summary>
    public Dictionary<string, AssetDatabaseLoader> ResourceRequestTaskMap
    {
        get
        {
            return mResourceRequestTaskMap;
        }
    }
    private Dictionary<string, AssetDatabaseLoader> mResourceRequestTaskMap;

    /// <summary>
    /// �����Ѽ��ص���Դ����Ϣӳ��Map
    /// KeyΪAB�������ͣ�ValueΪ�ü��������Ѿ����ص���Դ��Ϣӳ��Map(KeyΪAB���֣�ValueΪ��Դ������Ϣ)
    /// </summary>
    private Dictionary<ResourceLoadType, Dictionary<string, AssetDatabaseInfo>> mAllLoadedResourceInfoMap;

    /// <summary>
    /// �Ѽ�����Դ�ﲻ������Ч���õ���Դ��Ϣ�б�
    /// </summary>
    private List<AssetDatabaseInfo> mUnsedResourceInfoList;

    /// <summary> ���δʹ����Դʱ����(���������Ϊ��ʱ�ż��δʹ����Դ) /// </summary>
    private float mCheckUnsedResourceTimeInterval;

    /// <summary> ���δʹ����Դ�ȴ���� /// </summary>
    private WaitForSeconds mWaitForCheckUnsedResourceInterval;

    /// <summary>
    /// ��֡ж�ص���Դ�������
    /// ���ⵥ֡ж�ع���AB���¿���
    /// </summary>
    private int mMaxUnloadResourceNumberPerFrame;

    /// <summary>
    /// ��Դ��̵���Ч����ʱ��
    /// ���ڱ����ʱ����Ƶ��ɾ��ж��ͬһ��AB�����(����ͬһ������AB��Դ�����ظ��򿪹ر�)
    /// </summary>
    private float mResourceMinimumLifeTime;

    /// <summary>
    /// ��Դ����֡���ż�(����֡�ʹ��͵�ʱ�����AB��ɹ���)
    /// </summary>
    private int mResourceRecycleFPSThreshold;

    /// <summary>
    /// ��Դ������ӳ��Map(��Dictionaryֻ��Ϊ�˿��ٷ���)
    /// KeyΪ��Դ����ValueҲΪ��Դ��
    /// �ڰ����������Դ��ֻҪ�ϲ���ظ���Դ��һ����PreLoad��ʽ������Դ
    /// ������Ҫ��Ϊ�˱�����Ϊ�ϲ��߼����뵼�´������Դ��Ƶ������ж��
    /// ����: Res.cs    public bool SetImageSprite(UnityEngine.UI.Image image, string spName)
    /// </summary>
    private Dictionary<string, string> mResourceWhileListMap;
    #endregion

    /// <summary>
    /// ��Դ����ģ���ʼ��
    /// </summary>
    public override void init()
    {
        ResLoadMode = ResourceLoadMode.AssetDatabase;
        EnableResourceRecyclingUnloadUnsed = true;

        mResourceRequestTaskMap = new Dictionary<string, AssetDatabaseLoader>();
        mAllLoadedResourceInfoMap = new Dictionary<ResourceLoadType, Dictionary<string, AssetDatabaseInfo>>();
        mAllLoadedResourceInfoMap.Add(ResourceLoadType.NormalLoad, new Dictionary<string, AssetDatabaseInfo>());
        mAllLoadedResourceInfoMap.Add(ResourceLoadType.Preload, new Dictionary<string, AssetDatabaseInfo>());
        mAllLoadedResourceInfoMap.Add(ResourceLoadType.PermanentLoad, new Dictionary<string, AssetDatabaseInfo>());
        mUnsedResourceInfoList = new List<AssetDatabaseInfo>();
        mCheckUnsedResourceTimeInterval = 5.0f;
        mWaitForCheckUnsedResourceInterval = new WaitForSeconds(mCheckUnsedResourceTimeInterval);
        mMaxUnloadResourceNumberPerFrame = 10;
        mResourceMinimumLifeTime = 20.0f;
        mResourceRecycleFPSThreshold = 20;
        mResourceWhileListMap = new Dictionary<string, string>(); ;

        AssetDatabaseLoaderFactory.initialize(20);             // ���ǵ��󲿷ֶ��ǲ���ͬ�����أ�����AssetDatabaseLoader������Ҫ��ʼ��̫��
        AssetDatabaseInfoFactory.initialize(200);
    }

    /// <summary>
    /// ������Դ����ʹ�û��ռ��
    /// </summary>
    public override void startResourceRecyclingTask()
    {
        CoroutineManager.Singleton.startCoroutine(checkUnsedResource());
    }

    /// <summary>
    /// ���ָ����Դ��������
    /// </summary>
    /// <param name="resname">��Դ��(��AB��)</param>
    public override void addToWhiteList(string resname)
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
    /// ��Դ����ͳһ���
    /// </summary>
    /// <param name="resname">��ԴAB��</param>
    /// <param name="completehandler">��������ϲ�ص�</param>
    /// <param name="loadtype">��Դ��������</param>
    /// <param name="loadmethod">��Դ���ط�ʽ</param>
    public override void requstResource(string resname, LoadResourceCompleteHandler completehandler, ResourceLoadType loadtype = ResourceLoadType.NormalLoad, ResourceLoadMethod loadmethod = ResourceLoadMethod.Sync)
    {
        // �ڰ����������Դһ����Ԥ������ʽ���أ�
        // ������Ϊ�ϲ��߼�������غ�Ƶ������ж��
        if (mResourceWhileListMap.ContainsKey(resname))
        {
            loadtype = ResourceLoadType.Preload;
        }

        // �����Դ�Ѿ�������ɣ�ֱ�ӷ���
        if (mAllLoadedResourceInfoMap[ResourceLoadType.NormalLoad].ContainsKey(resname))
        {
            completehandler(mAllLoadedResourceInfoMap[ResourceLoadType.NormalLoad][resname]);
            if (loadtype > ResourceLoadType.NormalLoad)
            {
                updateLoadedResourceILoadType(resname, ResourceLoadType.NormalLoad, loadtype);
            }
        }
        else if (mAllLoadedResourceInfoMap[ResourceLoadType.Preload].ContainsKey(resname))
        {
            completehandler(mAllLoadedResourceInfoMap[ResourceLoadType.Preload][resname]);
            if (loadtype > ResourceLoadType.Preload)
            {
                updateLoadedResourceILoadType(resname, ResourceLoadType.Preload, loadtype);
            }
        }
        else if (mAllLoadedResourceInfoMap[ResourceLoadType.PermanentLoad].ContainsKey(resname))
        {
            completehandler(mAllLoadedResourceInfoMap[ResourceLoadType.PermanentLoad][resname]);
        }
        else
        {
            AssetDatabaseLoader adloader = createADLoader(resname);
            //��ʱĬ�϶���ͬ�����أ���֧���첽ģ��
            adloader.LoadMethod = loadmethod;
            adloader.LoadType = loadtype;
            adloader.LoadResourceCompleteCallBack = completehandler;
            adloader.LoadSelfResourceCompleteNotifier = onResourceLoadCompleteNotifier;
            mResourceRequestTaskMap.Add(resname, adloader);
            adloader.startLoad();
            /*
            // ȷ��ͬһ����Դ���ص�Loader��ͬһ��
            // ��֤ͬһ����Դ�������ʱ�ϲ����м��ظ���Դ�Ļص���ȷ
            AssetBundleLoader abloader = null;
            if (mABRequestTaskMap.ContainsKey(resname))
            {
                abloader = mABRequestTaskMap[resname];
                // ֮ǰ������resname��Դ������δ���
                // �������첽����resname�����첽���ǰ����һ��ͬ������resname
                // �޸ļ��ط�ʽ����ӻص�������ͬ�����ط�ʽ���첽���ػ���ͬ���������ʱһ��ص�
                abloader.LoadMethod = loadmethod;
                abloader.LoadType = loadtype;
                abloader.LoadABCompleteCallBack += completehandler;
                abloader.LoadSelfABCompleteNotifier = onABLoadCompleteNotifier;
                if (loadmethod == ResourceLoadMethod.Sync)
                {
                    ResourceLogger.log(string.Format("����ͬ������һ�������첽���ص���Դ : {0}", abloader.AssetBundleName));
                    //����AB����״̬����ͬ������ģʽ
                    abloader.LoadState = ResourceLoadState.None;
                    abloader.startLoad();
                }
            }
            else
            {
                abloader = createABLoader(resname);
                abloader.LoadMethod = loadmethod;
                abloader.LoadType = loadtype;
                abloader.LoadABCompleteCallBack = completehandler;
                abloader.LoadSelfABCompleteNotifier = onABLoadCompleteNotifier;
                mABRequestTaskMap.Add(resname, abloader);
                abloader.startLoad();
            }
            */
        }

    }

    /// <summary>
    /// �������
    /// </summary>
    public override void Update()
    {

    }


    /// <summary>
    /// �ͷſ��ͷŵ�Ԥ������Դ(�ݹ��ж��������ƻ�������)
    /// Note:
    /// �г���ǰ���ã�ȷ������Ԥ������Դ��ȷ�ͷ�
    /// </summary>
    public override void unloadAllUnsedPreloadLoadedResources()
    {
        unloadSpecificLoadTypeUnsedResourceRecursively(ResourceLoadType.Preload);
    }

    /// <summary>
    /// �ṩ���ⲿ�Ĵ���ж�������������ز���ʹ�õ���Դ(�ݹ��ж��������ƻ�������)
    /// Note:
    /// ͬ���ӿڣ�����������Ƚϴ�ֻ�����г���ʱ����ж�غ����һ��
    /// </summary>
    public override void unloadAllUnsedNormalLoadedResources()
    {
        unloadSpecificLoadTypeUnsedResourceRecursively(ResourceLoadType.NormalLoad);
    }

    /// <summary>
    /// �ݹ�ж��ָ�����Ͳ���ʹ�õ���Դ(Note:��֧��ж�س�פ��Դ����)
    /// </summary>
    /// <param name="resourceloadtype">AB��Դ��������</param>
    private void unloadSpecificLoadTypeUnsedResourceRecursively(ResourceLoadType resourceloadtype)
    {
        if (resourceloadtype == ResourceLoadType.PermanentLoad)
        {
            ResourceLogger.logErr("������ж�س�פAB��Դ!");
            return;
        }

        // �ݹ��ж�ж�����в��ٿ��õ�����������Դ
        bool hasunsedres = true;
        while (hasunsedres)
        {
            // �����ղ���ʹ�������Ѽ��ص�AB
            foreach (var loadedab in mAllLoadedResourceInfoMap[resourceloadtype])
            {
                if (loadedab.Value.IsUnsed)
                {
                    mUnsedResourceInfoList.Add(loadedab.Value);
                }
            }

            if (mUnsedResourceInfoList.Count == 0)
            {
                //�����п�ж�ص���Դ
                hasunsedres = false;
            }
            else
            {
                // �п�ж�ص�������
                for (int i = 0; i < mUnsedResourceInfoList.Count; i++)
                {
                    mAllLoadedResourceInfoMap[resourceloadtype].Remove(mUnsedResourceInfoList[i].AssetBundleName);
                    mUnsedResourceInfoList[i].dispose();
                }
                mUnsedResourceInfoList.Clear();
            }
        }
    }

    /// <summary>
    /// ��ӡ��ǰ��Դ����ʹ������Ϣ�Լ���������(������)
    /// </summary>
    public override void printAllLoadedResourceOwnersAndRefCount()
    {
        ResourceLogger.log("Normal Loaded AssetDatabase Info:");
        foreach (var abi in mAllLoadedResourceInfoMap[ResourceLoadType.NormalLoad])
        {
            abi.Value.printAllOwnersNameAndRefCount();
        }

        ResourceLogger.log("Preload Loaded AssetDatabase Info:");
        foreach (var abi in mAllLoadedResourceInfoMap[ResourceLoadType.Preload])
        {
            abi.Value.printAllOwnersNameAndRefCount();
        }

        ResourceLogger.log("Permanent Loaded AssetDatabase Info:");
        foreach (var abi in mAllLoadedResourceInfoMap[ResourceLoadType.PermanentLoad])
        {
            abi.Value.printAllOwnersNameAndRefCount();
        }
    }

    /// <summary>
    /// ����AssetDatabase��Դ���ض���
    /// </summary>
    /// <param name="resname">��Դ��</param>
    /// <returns></returns>
    private AssetDatabaseLoader createADLoader(string resname)
    {
        var adl = AssetDatabaseLoaderFactory.create();
        adl.AssetBundleName = resname;
        return adl;
    }

    /// <summary>
    /// �����ﲻ������Դ��Ҫ����ʱ��鲻��ʹ�õ���Դ
    /// </summary>
    private IEnumerator checkUnsedResource()
    {
        while (true)
        {
            if (EnableResourceRecyclingUnloadUnsed && mResourceRequestTaskMap.Count == 0)
            {
                float time = Time.time;
                // ����������ص���Դ�����ղ���ʹ�õ���Դ
                foreach (var loadedres in mAllLoadedResourceInfoMap[ResourceLoadType.NormalLoad])
                {
                    if (loadedres.Value.IsUnsed)
                    {
                        if ((time - loadedres.Value.LastUsedTime) > mResourceMinimumLifeTime)
                        {
                            mUnsedResourceInfoList.Add(loadedres.Value);
                        }
                    }
                }

                if (mUnsedResourceInfoList.Count > 0)
                {
                    // �������ʹ��ʱ����������
                    mUnsedResourceInfoList.Sort(ADILastUsedTimeSort);

                    for (int i = 0; i < mUnsedResourceInfoList.Count; i++)
                    {
                        if (i < mMaxUnloadResourceNumberPerFrame)
                        {
                            mAllLoadedResourceInfoMap[ResourceLoadType.NormalLoad].Remove(mUnsedResourceInfoList[i].AssetBundleName);
                            mUnsedResourceInfoList[i].dispose();
                        }
                        else
                        {
                            break;
                        }
                    }
                    mUnsedResourceInfoList.Clear();
                }
            }
            yield return mWaitForCheckUnsedResourceInterval;
        }
    }

    /// <summary> ��Դ�������֪ͨ(���ڸ�����Դ���ع���) /// </summary>
    /// <param name="adl">��Դ����������Ϣ</param>
    private void onResourceLoadCompleteNotifier(AssetDatabaseLoader adl)
    {
        var abname = adl.AssetBundleName;
        if (mResourceRequestTaskMap.ContainsKey(abname))
        {
            mResourceRequestTaskMap.Remove(abname);
            mAllLoadedResourceInfoMap[adl.LoadType].Add(abname, adl.ResourceInfo);
            //��Դ��������ͳ��
            if (ResourceLoadAnalyse.Singleton.ResourceLoadAnalyseSwitch)
            {
                ResourceLoadAnalyse.Singleton.addResourceLoadedTime(abname);
            }
        }
        else
        {
            ResourceLogger.logErr(string.Format("�յ����ڼ�����������������AB:{0}������ɻص�!", abname));
        }
    }

    /// <summary>
    /// AassetDatabase��Ϣ�������ʹ��ʱ������
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    private int ADILastUsedTimeSort(AssetDatabaseInfo a, AssetDatabaseInfo b)
    {
        return a.LastUsedTime.CompareTo(b.LastUsedTime);
    }

    /// <summary>
    /// �����Ѽ�����Դ�ļ�������
    /// </summary>
    /// <param name="resname">��Դ��</param>
    /// <param name="oldloadtype">�ɵļ�������</param>
    /// <param name="newloadtype">�µļ�������</param>
    private void updateLoadedResourceILoadType(string resname, ResourceLoadType oldloadtype, ResourceLoadType newloadtype)
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
}
#endif