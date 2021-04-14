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
    /// �Ѽ�����Դ�ﲻ������Ч���õ���Դ��Ϣ�б�
    /// </summary>
    private List<AbstractResourceInfo> mUnsedResourceInfoList;

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
    #endregion

    /// <summary>
    /// ��Դ����ģ���ʼ��
    /// </summary>
    public override void init()
    {
        base.init();
        ResLoadMode = ResourceLoadMode.AssetDatabase;

        mResourceRequestTaskMap = new Dictionary<string, AssetDatabaseLoader>();
        mUnsedResourceInfoList = new List<AbstractResourceInfo>();
        mCheckUnsedResourceTimeInterval = 5.0f;
        mWaitForCheckUnsedResourceInterval = new WaitForSeconds(mCheckUnsedResourceTimeInterval);
        mMaxUnloadResourceNumberPerFrame = 10;
        mResourceMinimumLifeTime = 20.0f;
        mResourceRecycleFPSThreshold = 20;

        AssetDatabaseLoaderFactory.initialize(20);             // ���ǵ��󲿷ֶ��ǲ���ͬ�����أ�����AssetDatabaseLoader������Ҫ��ʼ��̫��
        AssetDatabaseInfoFactory.initialize(200);
    }

    /// <summary>
    /// ������������Դ
    /// </summary>
    /// <param name="respath">��ԴAB·��</param>
    /// <param name="completehandler">��������ϲ�ص�</param>
    /// <param name="loadtype">��Դ��������</param>
    /// <param name="loadmethod">��Դ���ط�ʽ</param>
    protected override void realRequestResource(string respath, LoadResourceCompleteHandler completehandler, ResourceLoadType loadtype = ResourceLoadType.NormalLoad, ResourceLoadMethod loadmethod = ResourceLoadMethod.Sync)
    {
        // �����Դ�Ѿ�������ɣ�ֱ�ӷ���
        if (mAllLoadedResourceInfoMap[ResourceLoadType.NormalLoad].ContainsKey(respath))
        {
            completehandler(mAllLoadedResourceInfoMap[ResourceLoadType.NormalLoad][respath]);
            if (loadtype > ResourceLoadType.NormalLoad)
            {
                updateLoadedResourceInfoLoadType(respath, ResourceLoadType.NormalLoad, loadtype);
            }
        }
        else if (mAllLoadedResourceInfoMap[ResourceLoadType.Preload].ContainsKey(respath))
        {
            completehandler(mAllLoadedResourceInfoMap[ResourceLoadType.Preload][respath]);
            if (loadtype > ResourceLoadType.Preload)
            {
                updateLoadedResourceInfoLoadType(respath, ResourceLoadType.Preload, loadtype);
            }
        }
        else if (mAllLoadedResourceInfoMap[ResourceLoadType.PermanentLoad].ContainsKey(respath))
        {
            completehandler(mAllLoadedResourceInfoMap[ResourceLoadType.PermanentLoad][respath]);
        }
        else
        {
            AssetDatabaseLoader adloader = createADLoader(respath);
            //��ʱĬ�϶���ͬ�����أ���֧���첽ģ��
            adloader.LoadMethod = loadmethod;
            adloader.LoadType = loadtype;
            adloader.LoadResourceCompleteCallBack = completehandler;
            adloader.LoadSelfResourceCompleteNotifier = onResourceLoadCompleteNotifier;
            mResourceRequestTaskMap.Add(respath, adloader);
            adloader.startLoad();
        }
    }

    /// <summary>
    /// �������
    /// </summary>
    public override void Update()
    {
        base.Update();
    }

    /// <summary>
    /// ����ִ�еݹ�ж��ָ�����Ͳ���ʹ�õ���Դ�ӿ�
    /// </summary>
    /// <param name="resourceloadtype">��Դ��������</param>
    protected override void doUnloadSpecificLoadTypeUnsedResource(ResourceLoadType resourceloadtype)
    {
        // �ݹ��ж�ж�����в��ٿ��õ�����������Դ
        bool iscomplete = false;
        bool hasunsedres = false;
        while (!iscomplete)
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
                iscomplete = true;
            }
            else
            {
                hasunsedres = true;
                // �п�ж�ص�������
                for (int i = 0; i < mUnsedResourceInfoList.Count; i++)
                {
                    mAllLoadedResourceInfoMap[resourceloadtype].Remove(mUnsedResourceInfoList[i].AssetBundlePath);
                    mUnsedResourceInfoList[i].dispose();
                }
                mUnsedResourceInfoList.Clear();
            }
        }

        if (iscomplete && hasunsedres)
        {
            //Resources.UnloadAsset()ֻ�Ǳ�Ǹ���Դ�ܱ�ж��,����������ж����Դ
            //ͬʱAssetDatabaseģʽ�£�û��������Դ�ĸ��
            //Ϊ��ȷ������ʹ�õ���Դ����ȷж��(ģ��AssetBundleģʽ�ļ��ع�����)
            //ͨ������Resources.UnloadUnusedAssets()����������ж����Դ
            Resources.UnloadUnusedAssets();
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
        adl.AssetBundlePath = resname;
        return adl;
    }

    /// <summary>
    /// �����ﲻ������Դ��Ҫ����ʱ��鲻��ʹ�õ���Դ
    /// </summary>
    protected override IEnumerator checkUnsedResource()
    {
        while (true)
        {
            if (EnableResourceRecyclingUnloadUnsed && mCurrentFPS >= mResourceRecycleFPSThreshold && mResourceRequestTaskMap.Count == 0)
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
                            mAllLoadedResourceInfoMap[ResourceLoadType.NormalLoad].Remove(mUnsedResourceInfoList[i].AssetBundlePath);
                            mUnsedResourceInfoList[i].dispose();
                        }
                        else
                        {
                            break;
                        }
                    }
                    mUnsedResourceInfoList.Clear();
                    //Resources.UnloadAsset()ֻ�Ǳ�Ǹ���Դ�ܱ�ж��,����������ж����Դ
                    //ͬʱAssetDatabaseģʽ�£�û��������Դ�ĸ��
                    //Ϊ��ȷ������ʹ�õ���Դ����ȷж��(ģ��AssetBundleģʽ�ļ��ع�����)
                    //ͨ������Resources.UnloadUnusedAssets()����������ж����Դ
                    Resources.UnloadUnusedAssets();
                }
            }
            yield return mWaitForCheckUnsedResourceInterval;
        }
    }

    /// <summary> ��Դ�������֪ͨ(���ڸ�����Դ���ع���) /// </summary>
    /// <param name="adl">��Դ����������Ϣ</param>
    private void onResourceLoadCompleteNotifier(AssetDatabaseLoader adl)
    {
        var abname = adl.AssetBundlePath;
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
    private int ADILastUsedTimeSort(AbstractResourceInfo a, AbstractResourceInfo b)
    {
        return a.LastUsedTime.CompareTo(b.LastUsedTime);
    }
}
#endif