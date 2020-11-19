using System.Collections.Generic;
using UnityEngine.Events;

interface IMsgInfo { }

class MsgInfo : IMsgInfo
{
    public event UnityAction Action = null;
    public void HandleMessage()
    {
        if (Action != null) { Action(); }
    }
}

class MsgInfo<T> : IMsgInfo
{
    public event UnityAction<T> Action = null;
    public void HandleMessage(T param)
    {
        if (Action != null) { Action(param); }
    }
}

/// <summary>
/// 事件的增删清空, 事件分发
/// </summary>
public class MessageCenter : Singleton<MessageCenter>
{
    private Dictionary<string, IMsgInfo> eventDic = null;

    public MessageCenter()
    {
        eventDic = new Dictionary<string, IMsgInfo>();
    }

    public void AddListener(string msg, UnityAction listener)
    {
        if (!eventDic.ContainsKey(msg))
        {
            eventDic.Add(msg, new MsgInfo());
        }
        (eventDic[msg] as MsgInfo).Action += listener;
    }

    public void AddListener<T>(string msg, UnityAction<T> listener)
    {
        if (!eventDic.ContainsKey(msg))
        {
            eventDic.Add(msg, new MsgInfo<T>());
        }
        (eventDic[msg] as MsgInfo<T>).Action += listener;
    }

    public void RemoveListener(string msg, UnityAction listener)
    {
        if (eventDic.ContainsKey(msg))
        {
            (eventDic[msg] as MsgInfo).Action -= listener;
        }
    }

    public void RemoveListener<T>(string msg, UnityAction<T> listener)
    {
        if (eventDic.ContainsKey(msg))
        {
            (eventDic[msg] as MsgInfo<T>).Action -= listener;
        }
    }

    public void SendMessage(string msg)
    {
        if (eventDic.ContainsKey(msg))
        {
            (eventDic[msg] as MsgInfo).HandleMessage();
        }
    }

    public void SendMessage<T>(string msg, T param)
    {
        if (eventDic.ContainsKey(msg))
        {
            (eventDic[msg] as MsgInfo<T>).HandleMessage(param);
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
