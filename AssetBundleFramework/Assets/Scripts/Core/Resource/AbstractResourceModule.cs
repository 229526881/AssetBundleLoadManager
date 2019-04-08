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
    /// ��Դ����ģ���ʼ��
    /// </summary>
    public abstract void init();

    /// <summary>
    /// ������Դ����ʹ�û��ռ��
    /// </summary>
    public abstract void startResourceRecyclingTask();

    /// <summary>
    /// ���ָ����Դ��������
    /// </summary>
    /// <param name="resname">��Դ��(��AB��)</param>
    public abstract void addToWhiteList(string resname);

    /// <summary>
    /// ������Դ
    /// ��Դ����ͳһ���
    /// </summary>
    /// <param name="resname">��ԴAB��</param>
    /// <param name="completehandler">��������ϲ�ص�</param>
    /// <param name="loadtype">��Դ��������</param>
    /// <param name="loadmethod">��Դ���ط�ʽ</param>
    public abstract void requstResource(string resname, LoadResourceCompleteHandler completehandler, ResourceLoadType loadtype = ResourceLoadType.NormalLoad, ResourceLoadMethod loadmethod = ResourceLoadMethod.Sync);

    /// <summary>
    /// �������
    /// </summary>
    public abstract void Update();

    /// <summary>
    /// �ͷſ��ͷŵ�Ԥ������Դ(�ݹ��ж��������ƻ�������)
    /// Note:
    /// �г���ǰ���ã�ȷ������Ԥ������Դ��ȷ�ͷ�
    /// </summary>
    public abstract void unloadAllUnsedPreloadLoadedResources();

    /// <summary>
    /// �ṩ���ⲿ�Ĵ���ж�������������ز���ʹ�õ���Դ��Դ(�ݹ��ж��������ƻ�������)
    /// Note:
    /// ͬ���ӿڣ�����������Ƚϴ�ֻ�����г���ʱ����ж�غ����һ��
    /// </summary>
    public abstract void unloadAllUnsedNormalLoadedResources();
}