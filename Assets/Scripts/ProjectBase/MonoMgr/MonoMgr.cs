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
    // ����ⲿUpdate����
    public void AddUpdateAction(UnityAction action)
    {
        controller.updateEvent += action;
    }
    // �Ƴ��ⲿ��Update����
    public void RemoveUpdateAction(UnityAction action)
    {
        controller.updateEvent -= action;
    }
    // ����Э��
    public Coroutine StartCoroutine(IEnumerator ie)
    {
        return controller.StartCoroutine(ie);
    }
    // ֹͣ����Э��
    public void StopAllCoroutines()
    {
        controller.StopAllCoroutines();
    }
    // ���ݵ�����ֹͣһ��Э��
    public void StopCoroutine(IEnumerator ie)
    {
        controller.StopCoroutine(ie);
    }
    // ����Coroutine����ֹͣһ��Э��
    public void StopCoroutine(Coroutine co)
    {
        controller.StopCoroutine(co);
    }
    // ����Э�̵���ʱ����
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
