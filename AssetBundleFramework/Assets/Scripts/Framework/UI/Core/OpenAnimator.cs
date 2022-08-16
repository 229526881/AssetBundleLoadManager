using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class OpenAnimator : MonoBehaviour
{
    private Animator _anim;

    private Animator anim { get { return _anim ?? (_anim = transform.GetComponent<Animator>()); } }

    public void PlayAnim(string animName, Action endEvent = null)
    {
        //需要做个判断如果，没有该动画该怎么处理
        anim.Play(animName);
        StartCoroutine(DelayRunEffectCallback(anim, animName, endEvent));
    }

    public IEnumerator DelayRunEffectCallback(Animator animator, string stateName, Action callback)
    {
        // 状态机的切换发生在帧的结尾
        yield return new WaitForEndOfFrame();

        var info = animator.GetCurrentAnimatorStateInfo(0);
        if (!info.IsName(stateName)) yield return null;

        yield return new WaitForSeconds(info.length);
        callback?.Invoke();
    }

}
