using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using SFramework;

public class SceneMgr :SingletonTemplate<SceneMgr>,IMsgSender
{
    private SceneType _lastType=SceneType.Bootstarp;

    private SceneType _curType=SceneType.Bootstarp;

    private bool isLoading = false;

    public  AsyncOperation asyncOpe = null;

    private GameObject loadBg;

    public void LoadScene(SceneType type,string bgPath="",object param=null)
    {
        if (isLoading) return;
        isLoading = true;
        asyncOpe = null;
        //if(!string.IsNullOrEmpty(bgPath)) UIManager.Instance.CreateBg(bgPath);
        //UIManager.Instance.OpenPanel(UIType.Loading, type);
        _lastType = _curType;
        _curType = type;
    }

    public void Update(bool state)
    {
        if (isLoading && asyncOpe != null)
        {
            if (asyncOpe.progress >=1f && state)
            {
                //直接关闭界面可能会有问题，因为新的场景可能会打开新的界面，因此需要注意当前状态
               // UIManager.Instance.ClosePanel(UIType.Loading);
               // SoundManager.Instance.Stop("bgm");
                isLoading = false;
                asyncOpe = null;
                this.SendLogicMsg(EventId.Change_SceneEnd);
                DIYLog.Log("关闭加载页面");
                Resources.UnloadUnusedAssets();
            }
        }
    }

    public SceneType GetLastType()
    {
        return this._lastType;
    }
}
