/*
 * Description:             TWebRequest.cs
 * Author:                  TONYTANG
 * Create Date:             2019//04/21
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// TWebRequest.cs
/// Web������ʷ�װ
/// </summary>
public class TWebRequest {

    /// <summary>
    /// Web����״̬
    /// </summary>
    public enum WebRequestStatus
    {
        WRP_None = 1,           // ��״̬
        WRP_Faield,             // ʧ��
        WRP_Complete            // ���
    }

    /// <summary>
    /// Web����������Ϣ����
    /// </summary>
    private class WebRequestTaskInfo
    {
        /// <summary>
        /// ����URL
        /// </summary>
        public string URL
        {
            get;
            private set;
        }

        /// <summary>
        /// ������ɻص�
        /// </summary>
        public Action<string, WebRequestStatus> CompleteCallback
        {
            get;
            private set;
        }

        /// <summary>
        /// ����ʱʱ��
        /// </summary>
        public int TimeOut
        {
            get;
            private set;
        }

        /// <summary>
        /// Web����������Ϣ���캯��
        /// </summary>
        /// <param name="url"></param>
        /// <param name="callback"></param>
        /// <param name="timeout"></param>
        public WebRequestTaskInfo(string url, Action<string, WebRequestStatus> callback, int timeout)
        {
            URL = url;
            CompleteCallback = callback;
            TimeOut = timeout;
        }
    }

    /// <summary>
    /// Web�����������
    /// </summary>
    private Queue<WebRequestTaskInfo> mWebRequestTaskQueue;

    /// <summary>
    /// �Ƿ���������
    /// </summary>
    private bool mIsInProgress;

    public TWebRequest()
    {
        mWebRequestTaskQueue = new Queue<WebRequestTaskInfo>();
        mIsInProgress = false;
    }

    /// <summary>
    /// �������������
    /// </summary>
    /// <param name="url">url</param>
    /// <param name="completecallback">��ɻص�</param>
    /// <param name="timeout">��ʱʱ��</param>
    public void enqueue(string url, Action<string, WebRequestStatus> completecallback, int timeout = 5)
    {
        if(mIsInProgress == false)
        {
            if(!url.IsNullOrEmpty() && completecallback != null)
            {
                var newtask = new WebRequestTaskInfo(url, completecallback, timeout);
                mWebRequestTaskQueue.Equals(newtask);
            }
            else
            {
                DIYLog.LogError("URL��completecallback������Ϊ�գ��������ʧ�ܣ�");
            }
        }
        else
        {
            DIYLog.LogError("�Ѿ��������У��޷��������");
        }
    }

    /// <summary>
    /// ��ʼ������Դ����
    /// </summary>
    public void startRequest()
    {
        if(!mIsInProgress)
        {
            if (mWebRequestTaskQueue.Count > 0)
            {
                CoroutineManager.Singleton.startCoroutine(requestCoroutine());
            }
            else
            {
                DIYLog.LogWarning("û��������Ϣ���޷���ʼ����");
            }
        }
        else
        {
            DIYLog.LogWarning("�Ѿ��������У��޷���ʼ����");
        }
    }

    /// <summary>
    /// ��������Я��
    /// </summary>
    /// <returns></returns>
    private IEnumerator requestCoroutine()
    {
        mIsInProgress = true;

        while(mWebRequestTaskQueue.Count > 0)
        {
            var task = mWebRequestTaskQueue.Dequeue();
            DIYLog.Log(string.Format("������Դ : {0}", task.URL));
            var webrequest = UnityWebRequest.Get(task.URL);
            webrequest.timeout = task.TimeOut;
            yield return webrequest.SendWebRequest();
            if (webrequest.isNetworkError)
            {
                DIYLog.LogError(string.Format("{0}��Դ���س���!", task.URL));
                DIYLog.LogError(webrequest.error);
                if(webrequest.isHttpError)
                {
                    DIYLog.LogError(string.Format("responseCode : ", webrequest.responseCode));
                }
                task.CompleteCallback(task.URL, WebRequestStatus.WRP_Faield);
            }
            else
            {
                DIYLog.Log(string.Format("{0} webrequest.isDone:{1}!", task.URL, webrequest.isDone));
                DIYLog.Log(string.Format("{0}��Դ�������!", task.URL));
                task.CompleteCallback(task.URL, WebRequestStatus.WRP_Faield);
            }
        }
    }
}