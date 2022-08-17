using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HotFix_Project
{
    class LoginWindow:UIWindow
    {
        private LoginPanel m_LoginPanel; 
        public override void Awake()
        {
            m_LoginPanel = PanelInfo as LoginPanel;
            AddButtonClickListener(m_LoginPanel.m_BtnLogin, OnLoginClick);
        }

        public override void OnShow()
        {
            base.OnShow();
            Debug.Log("子类OnShow");
        }

        public override void OnOpenEnd()
        {
            Debug.Log("OnOpenEnd");
          
        }

        public void OnLoginClick()
        {
            CloseSelf();
            //GameSceneManager.Singleton.LoadScene(SceneType.Main, "Assets/_assets/uiassets/uitextures/loading/bgTest.png");
            GameSceneManager.Singleton.LoadScene(SceneType.Main, "Assets/_assets/uiassets/uitextures/common1/zuobian.png");
        }
    }
}
