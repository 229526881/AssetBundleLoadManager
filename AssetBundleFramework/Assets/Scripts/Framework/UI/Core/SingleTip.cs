using SFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SingleTip : MonoBehaviour
{
    //public Text text;
    //public Image imgBg;
    //public Animator anim;
    //private AnimatorStateInfo stateinfo;
    ////private string animName = "tipShow";
    //private float deltaWidth = 100;
    //private float deltaHeight = 34;
    //private string idleAnim = "tipIdle";
    //private string animName;
    //private float deltaTime;

    //public void ShowTip(string msg, string animName, float delta = 1f, float waitTime = 1)
    //{
    //    gameObject.SetActive(true);
    //    transform.localPosition = Vector3.zero;
    //    text.text = msg;
    //    //Debug.Log("动画速度:" + anim.speed+"   "+ time);
    //    //同时需要修改文本背景长度
    //    StartCoroutine(SetWidth());
    //    this.animName = animName;
    //    this.deltaTime = delta;
    //    Invoke("PlayeAnim", waitTime);
    //}

    //private void PlayeAnim()
    //{
    //    anim.Play(animName);
    //    AnimationClip clip = GetClip(animName);
    //    float time = clip.length;
    //    anim.speed = deltaTime / time;
    //}

    ///// <summary>
    ///// 设置文本尺寸
    ///// </summary>
    ///// <returns></returns>
    //private  IEnumerator SetWidth()
    //{
    //    yield return new  WaitForEndOfFrame();
    //    float width = text.GetComponent<RectTransform>().rect.width+deltaWidth;
    //    float height = text.GetComponent<RectTransform>().rect.height + deltaHeight;
    //    imgBg.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
    //    imgBg.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);

    //}

    ////动画结束后通知
    //private void Update()
    //{
    //    stateinfo = anim.GetCurrentAnimatorStateInfo(0);
    //    //这里要多次调用个才行 todo
    //    if (stateinfo.normalizedTime >= 1)
    //    {
    //        PoolManager.Instance["GameTip"].Despawn(transform);
    //    }
    //}

    //private AnimationClip GetClip(string clipName)
    //{
    //    AnimationClip [] clips = anim.runtimeAnimatorController.animationClips;
        
    //    AnimationClip clip= Array.Find(clips, (item) =>
    //    {
    //        return item.name == clipName;
    //    });

    //    return clip;
    //}
}
