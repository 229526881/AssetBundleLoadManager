using Data;
using SFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HotFix_Project
{
    class LoadingWindow : UIWindow
    {
        public LoadingPanel m_LoadingPanel;
        private AsyncOperation m_AsyncOpe;
        private float m_TimeDelta=0;
        public override void Awake()
        {
            m_LoadingPanel = PanelInfo as LoadingPanel;
        }
        public override void OnShow()
        {
            SceneData data = UIArgs as SceneData;
            m_AsyncOpe = data.operation;
            CreateBg(data.bgPath);

            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Reset();
            sw.Start();
            var languageList = GameDataManager.Singleton.Gett_AuthorInfoList();
            DIYLog.Log(languageList.Count + "----------------------------------------------");
            for (int i = 0; i < 500; i++)
            {
                var language = languageList[i];
                DIYLog.Log("----------------------------------------------");
                DIYLog.Log(string.Format("language Key : {0}", language.Id));
                DIYLog.Log(string.Format("language Value : {0}", language.age));
            }
            sw.Stop();
            Debug.LogFormat("刚刚的方法执行了:{0} ms", sw.ElapsedMilliseconds);
        }

        public override void OnUpdate()
        {
            //需要根据打开的界面处理
            if (null != m_AsyncOpe)
            {
                m_TimeDelta += Time.deltaTime;
                float progress = m_TimeDelta * 1f / m_LoadingPanel.m_IminTime;
                m_LoadingPanel.m_SliderExp.value = progress;
                if (m_TimeDelta >= m_LoadingPanel.m_IminTime)
                { 
                    if (m_AsyncOpe.progress >= 0.9f)
                    {
                        m_AsyncOpe.allowSceneActivation = true;
                    }
                    //必须只执行一次
                    m_LoadingPanel.m_SliderExp.value = 1;
                }
                m_LoadingPanel.m_TxtExp.text =  Mathf.Round(Math.Min(progress,1) * 100) + "%";
            }
        }

        public override void OnClose()
        {
 
        }

        private void CreateBg(string path)
        {
            AtlasManager.Singleton.setTImageSingleSprite(m_LoadingPanel.m_ImgBg, path);
        }
        //private void CreateBg1()
        //{
        //    //AtlasManager.Instance.setTImageSingleSprite(m_LoadingPanel.m_ImgBg, path);
        //    AtlasManager.Instance.setTImageSpriteAtlas(m_LoadingPanel.m_ImgBg, "Assets/_assets/uiassets/uitextures/common1/common1.spriteatlas", "zuobian");
        //}
    }
}
