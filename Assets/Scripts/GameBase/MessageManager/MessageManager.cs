using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

interface IMsgInfo {}

class MsgInfo : IMsgInfo
{
    public event UnityAction Listener = null;
    public void TriggerEvent()
    {
        if (Listener != null) { Listener(); }
    }
}

class MsgInfo<T> : IMsgInfo
{
    public event UnityAction<T> Listener = null;
    public void TriggerEvent(T param)
    {
        if (Listener != null) { Listener(param); }
    }
}

/// <summary>
/// 事件的增删清空, 事件分发
/// </summary>
public class MessageManager : SingletonBase<MessageManager>
{
    private Dictionary<string, IMsgInfo> eventDic = null;

    public MessageManager()
    {
        eventDic = new Dictionary<string, IMsgInfo>();
    }

    public void AddListener(string msg, UnityAction listener)
    {
        if (!eventDic.ContainsKey(msg))
        {
            eventDic.Add(msg, new MsgInfo());
        }
        (eventDic[msg] as MsgInfo).Listener += listener;
    }

    public void AddListener<T>(string msg, UnityAction<T> listener)
    {
        if (!eventDic.ContainsKey(msg))
        {
            eventDic.Add(msg, new MsgInfo<T>());
        }
        (eventDic[msg] as MsgInfo<T>).Listener += listener;
    }

    public void RemoveListener(string msg, UnityAction listener)
    {
        if (eventDic.ContainsKey(msg))
        {
            (eventDic[msg] as MsgInfo).Listener -= listener;
        }
    }

    public void RemoveListener<T>(string msg, UnityAction<T> listener)
    {
        if (eventDic.ContainsKey(msg))
        {
            (eventDic[msg] as MsgInfo<T>).Listener -= listener;
        }
    }

    public void TriggerEvent(string msg)
    {
        if (eventDic.ContainsKey(msg))
        {
            (eventDic[msg] as MsgInfo).TriggerEvent();
        }
    }

    public void TriggerEvent<T>(string strEvent, T param)
    {
        if (eventDic.ContainsKey(strEvent))
        {
            (eventDic[strEvent] as MsgInfo<T>).TriggerEvent(param);
        }
    }

    /// <summary>
    /// 清空监听, 一般过场景时
    /// </summary>
    public void Clear()
    {
        eventDic.Clear();
    }
}
