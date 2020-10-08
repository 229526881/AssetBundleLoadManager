/*
 * Description:             TRawImage.cs
 * Author:                  TONYTANG
 * Create Date:             2020//10/08
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TUI
{
    /// <summary>
    /// TRawImage.cs
    /// ��дRawImage�����������Դ������Զ���һЩ����
    /// Note:
    /// Ĭ���ڱ༭����ֵ��ͨ���������صĲ��ṩ��Դ��Ϣ�ӿ�(ABI),
    /// ����������ǰTRawImage���ж��ʱ������Դ����ǰж��
    /// </summary>
    public class TRawImage : RawImage
    {
        /// <summary>
        /// ��Դ������Ϣ
        /// </summary>
        public AbstractResourceInfo ABI
        {
            get;
            set;
        }

        /// <summary>
        /// ��ǰͼƬ��
        /// </summary>
        [HideInInspector]
        public string TextureName;

        /// <summary>
        /// ��ӡ��ǰTImageͼ��ʹ����Ϣ
        /// </summary>
        public void printTRawImageInfo()
        {
            DIYLog.Log($"TextureName = {TextureName}");
            var refcount = ABI != null ? ABI.RefCount.ToString() : "��";
            DIYLog.Log($"TextureName���ü��� = {refcount}");
        }
    }
}