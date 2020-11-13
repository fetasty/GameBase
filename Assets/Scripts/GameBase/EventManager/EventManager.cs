using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

interface IEventInfo {}

class EventInfo : IEventInfo
{
    public event UnityAction Listener = null;
    public void TriggerEvent()
    {
        if (Listener != null) { Listener(); }
    }
}

class EventInfo<T> : IEventInfo
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
public class EventManager : SingletonBase<EventManager>
{
    private Dictionary<string, IEventInfo> eventDic = null;

    public EventManager()
    {
        eventDic = new Dictionary<string, IEventInfo>();
    }

    public void AddListener(string msg, UnityAction listener)
    {
        if (!eventDic.ContainsKey(msg))
        {
            eventDic.Add(msg, new EventInfo());
        }
        (eventDic[msg] as EventInfo).Listener += listener;
    }

    public void AddListener<T>(string msg, UnityAction<T> listener)
    {
        if (!eventDic.ContainsKey(msg))
        {
            eventDic.Add(msg, new EventInfo<T>());
        }
        (eventDic[msg] as EventInfo<T>).Listener += listener;
    }

    public void RemoveListener(string msg, UnityAction listener)
    {
        if (eventDic.ContainsKey(msg))
        {
            (eventDic[msg] as EventInfo).Listener -= listener;
        }
    }

    public void RemoveListener<T>(string msg, UnityAction<T> listener)
    {
        if (eventDic.ContainsKey(msg))
        {
            (eventDic[msg] as EventInfo<T>).Listener -= listener;
        }
    }

    public void TriggerEvent(string msg)
    {
        if (eventDic.ContainsKey(msg))
        {
            (eventDic[msg] as EventInfo).TriggerEvent();
        }
    }

    public void TriggerEvent<T>(string msg, T param)
    {
        if (eventDic.ContainsKey(msg))
        {
            (eventDic[msg] as EventInfo<T>).TriggerEvent(param);
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
