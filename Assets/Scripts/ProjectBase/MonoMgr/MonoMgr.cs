using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MonoMgr : Singleton<MonoMgr>
{
    private MonoController controller;
    public MonoMgr()
    {
        GameObject obj = new GameObject(nameof(MonoMgr));
        Object.DontDestroyOnLoad(obj);
        controller = obj.AddComponent<MonoController>();
    }
    // 添加外部Update函数
    public void AddUpdateAction(UnityAction action)
    {
        controller.updateEvent += action;
    }
    // 移除外部的Update函数
    public void RemoveUpdateAction(UnityAction action)
    {
        controller.updateEvent -= action;
    }
    // 开启协程
    public Coroutine StartCoroutine(IEnumerator ie)
    {
        return controller.StartCoroutine(ie);
    }
    // 停止所有协程
    public void StopAllCoroutines()
    {
        controller.StopAllCoroutines();
    }
    // 根据迭代器停止一个协程
    public void StopCoroutine(IEnumerator ie)
    {
        controller.StopCoroutine(ie);
    }
    // 根据Coroutine对象停止一个协程
    public void StopCoroutine(Coroutine co)
    {
        controller.StopCoroutine(co);
    }
    // 基于协程的延时函数
    public void Invoke(UnityAction action, float seconds)
    {
        if (action == null)
        {
            throw new System.ArgumentNullException("Params can not be null!");
        }
        if (seconds <= 0)
        {
            action.Invoke();
        }
        else
        {
            StartCoroutine(InvokeCoroutine(action, seconds));
        }
    }

    private IEnumerator InvokeCoroutine(UnityAction action, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        action.Invoke();
    }
}
