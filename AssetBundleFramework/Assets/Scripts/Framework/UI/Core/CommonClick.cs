using SFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 通用鼠标点击效果
/// </summary>
public class CommonClick : MonoBehaviour
{

    //public Transform clickPrefab;
    //private PrefabPool _prefabPool;
    //private PoolGroup _clickPool;
    //private Canvas _canvas;

    //void Start()
    //{
    //    _prefabPool = PoolManager.Instance.CreatePrefabPool("CommonClick", clickPrefab);
    //    _prefabPool.limitCount = 5;
    //    _prefabPool.limitInstances = true;
    //    _canvas = UIManager.Instance.GetRootCanvas();
    //}

    //public void ShowCommonClick()
    //{
    //    _clickPool = PoolManager.Instance["CommonClick"];
    //    Transform obj = _clickPool.Spawn("click", transform);

    //    Vector2 _pos = Vector2.one;
    //    RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvas.transform as RectTransform,
    //                Input.mousePosition, _canvas.worldCamera, out _pos);


    //    Vector2 tranPos = _pos;
    //    obj.localPosition = tranPos;
    //    obj.transform.Find("click").GetComponent<ParticleSystem>().Play();
    //    this.StartCoroutine(RecycleObj(obj));
    //}


    //private IEnumerator RecycleObj(Transform obj)
    //{
    //    yield return new WaitForSeconds(1.0f);
    //    PoolManager.Instance["CommonClick"].Despawn(obj);
    //}
}
