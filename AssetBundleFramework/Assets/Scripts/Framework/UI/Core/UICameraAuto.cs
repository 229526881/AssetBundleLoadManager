using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SFramework;

//需要保证UI相机永远是最上层的，所以需要处理
public class UICameraAuto : MonoBehaviour,IMsgRegister
{
    public bool isDontDestory=false;

    private void Awake()
    {
        if (isDontDestory)
        {
            this.RegisterLogicMsg(EventId.Change_SceneEnd, AddCameraStack);
        }
    }

    private void OnDestroy()
    {
        this.ClearSingleLogicMsg(EventId.Change_SceneEnd);
    }
    void Start()
    {
        AddCameraStack();
    }

    void AddCameraStack(params object[] param)
    {
        //以后再完善加入的堆栈
        //var m_uiCamera = GetComponent<Camera>();
        //if (!gameObject.name.Equals("UICamera")&& !gameObject.name.Equals("ViewCamera"))
        //{
        //    int count = Camera.main.GetUniversalAdditionalCameraData().cameraStack.Count;
        //    Camera.main.GetUniversalAdditionalCameraData().cameraStack.Insert(count-1, m_uiCamera);
          
        //}
        //else
        //{
        //    Camera.main.GetUniversalAdditionalCameraData().cameraStack.Add(m_uiCamera);
        //}
    }
}
