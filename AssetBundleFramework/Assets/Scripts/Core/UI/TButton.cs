/*
 * Description:             TButton.cs
 * Author:                  TONYTANG
 * Create Date:             2020//10/08
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TUI
{
    /// <summary>
    /// TButton.cs
    /// ��дButton����������ṩһЩ���⹦��(���簴ťͳһ�����Ч����ť���ű��ֵ�)
    /// </summary>
    [RequireComponent(typeof(TImage))]
    public class TButton : Button, IPointerDownHandler, IPointerUpHandler
    {
        /// <summary>
        /// �����������
        /// </summary>
        [Header("�����������")]
        public bool EnableLongtimePress = false;

        /// <summary>
        /// ��������Ƿ�ֻ��һ��(��֮������)
        /// </summary>
        [Header("��������Ƿ�ֻ��һ��(��֮������)")]
        public bool IsLongtimePressOnlyOnce = true;

        /// <summary>
        /// ��Ч����ʱ�����
        /// </summary>
        [Header("��Ч����ʱ�����")]
        public float LongtimePressTimeInterval = 1.0f;

        /// <summary>
        /// ��������ص�
        /// </summary>
        public Action LongTimePressedClick
        {
            get;
            set;
        }

        /// <summary>
        /// �������ʱ��
        /// </summary>
        private float mLongTimeClickPressedTimePassed;

        /// <summary>
        /// ���������Ӧ����
        /// </summary>
        private int mLongTimePressedCalledTimes;

        /// <summary>
        /// �Ƿ���
        /// </summary>
        private bool mIsPressed;

        public override void OnPointerDown(PointerEventData pointerEventData)
        {
            base.OnPointerDown(pointerEventData);
            mIsPressed = true;
            if (EnableLongtimePress)
            {
                mLongTimePressedCalledTimes = 0;
                mLongTimeClickPressedTimePassed = 0f;
            }
            //TODO: ͳһ������Ч������������
        }

        private void Update()
        {
            if(mIsPressed && EnableLongtimePress)
            {
                mLongTimeClickPressedTimePassed += Time.deltaTime;
                if (mLongTimeClickPressedTimePassed >= LongtimePressTimeInterval)
                {
                    if (mLongTimePressedCalledTimes == 0 && IsLongtimePressOnlyOnce)
                    {
                        LongTimePressedClick?.Invoke();
                    }
                    else
                    {
                        LongTimePressedClick?.Invoke();
                    }
                    mLongTimePressedCalledTimes++;
                    mLongTimeClickPressedTimePassed = 0f;
                    Debug.Log($"�����������:{mLongTimePressedCalledTimes}");
                }
                Debug.Log($"����ʱ��:{mLongTimeClickPressedTimePassed}");
            }
        }

        public override void OnPointerUp(PointerEventData pointerEventData)
        {
            base.OnPointerUp(pointerEventData);
            if (EnableLongtimePress)
            {
                mLongTimeClickPressedTimePassed = 0f;
                mLongTimePressedCalledTimes = 0;
            }
            mIsPressed = false;
            //TODO: ͳһ����������
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            LongTimePressedClick = null;
        }
    }
}