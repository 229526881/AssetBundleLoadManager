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
    /// <summary>
    /// ��Դ����ģ���ʼ��
    /// </summary>
    public override void init()
    {
        AssetDatabaseLoaderFactory.initialize(20);             // ���ǵ��󲿷ֶ��ǲ���ͬ�����أ�����AssetDatabaseLoader������Ҫ��ʼ��̫��
        AssetDatabaseInfoFactory.initialize(200);
    }

    /// <summary>
    /// ������Դ����ʹ�û��ռ��
    /// </summary>
    public override void startResourceRecyclingTask()
    {

    }

    /// <summary>
    /// ���ָ����Դ��������
    /// </summary>
    /// <param name="resname">��Դ��(��AB��)</param>
    public override void addToWhiteList(string resname)
    {

    }

    /// <summary>
    /// ������Դ
    /// ��Դ����ͳһ���
    /// </summary>
    /// <param name="resname">��ԴAB��</param>
    /// <param name="assetname">asset��</param>
    /// <param name="completehandler">��������ϲ�ص�</param>
    /// <param name="loadtype">��Դ��������</param>
    /// <param name="loadmethod">��Դ���ط�ʽ</param>
    public override void requstResource(string resname, string assetname, LoadResourceCompleteHandler completehandler, ResourceLoadType loadtype = ResourceLoadType.NormalLoad, ResourceLoadMethod loadmethod = ResourceLoadMethod.Sync)
    {
        AssetDatabaseLoader adloader = createADLoader(resname, assetname);
        //��ʱĬ�϶���ͬ�����أ���֧���첽ģ��
        adloader.LoadMethod = loadmethod;
        adloader.LoadType = loadtype;
        adloader.LoadResourceCompleteCallBack = completehandler;
        adloader.startLoad();
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

    }

    /// <summary>
    /// �ṩ���ⲿ�Ĵ���ж�������������ز���ʹ�õ���Դ(�ݹ��ж��������ƻ�������)
    /// Note:
    /// ͬ���ӿڣ�����������Ƚϴ�ֻ�����г���ʱ����ж�غ����һ��
    /// </summary>
    public override void unloadAllUnsedNormalLoadedResources()
    {

    }

    /// <summary>
    /// ����AssetDatabase��Դ���ض���
    /// </summary>
    /// <param name="resname">��Դ��</param>
    /// <param name="assetname">asset��</param>
    /// <returns></returns>
    private AssetDatabaseLoader createADLoader(string resname, string assetname)
    {
        var loader = AssetDatabaseLoaderFactory.create();
        loader.AssetBundleName = resname;
        loader.AssetName = assetname;
        return loader;
    }
}
#endif