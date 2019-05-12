/*
 * Description:             NativeMessageHandler.cs
 * Author:                  TONYTANG
 * Create Date:             2018/08/10
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// NativeMessageHandler.cs
/// ԭ����Ϣ��Ӧ������
/// </summary>
public class NativeMessageHandler : MonoBehaviour {

    public Text TxtOutput;

    /// <summary>
    /// ����ԭ����Ϣ
    /// </summary>
    /// <param name="msg"></param>
    public void resUnityMsg(string msg)
    {
        Debug.Log(string.Format("resUnityMsg : {0}", msg));
        TxtOutput.text = msg;
    }

    /// <summary>
    /// ����ԭ��Log
    /// </summary>
    /// <param name="msg"></param>
    public void resJavaLog(string log)
    {
        Debug.Log(string.Format("Java Log : {0}", log));
    }
}