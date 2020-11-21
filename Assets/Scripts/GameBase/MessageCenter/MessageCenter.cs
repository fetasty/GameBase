using System.Collections.Generic;
using UnityEngine.Events;

interface IMsgInfo { }

/// <summary>
/// 不带参数的消息
/// </summary>
class MsgInfo : IMsgInfo {
    public event UnityAction Action = null;
    public void HandleMessage() {
        if (Action != null) { Action(); }
    }
}
/// <summary>
/// 带参数的消息
/// </summary>
/// <typeparam name="T">参数类型</typeparam>
class MsgInfo<T> : IMsgInfo {
    public event UnityAction<T> Action = null;
    public void HandleMessage(T param) {
        if (Action != null) { Action(param); }
    }
}

/// <summary>
/// 消息中心, 可以分发 & 订阅 对应的消息
/// </summary>
public class MessageCenter : Singleton<MessageCenter> {
    private Dictionary<string, IMsgInfo> eventDic = null;

    public MessageCenter() {
        eventDic = new Dictionary<string, IMsgInfo>();
    }
    /// <summary>
    /// 添加无参消息监听
    /// </summary>
    /// <param name="msg">消息类型</param>
    /// <param name="listener">消息处理函数</param>
    public void AddListener(string msg, UnityAction listener) {
        if (!eventDic.ContainsKey(msg)) {
            eventDic.Add(msg, new MsgInfo());
        }
        (eventDic[msg] as MsgInfo).Action += listener;
    }
    /// <summary>
    /// 添加有参消息监听
    /// </summary>
    /// <typeparam name="T">参数类型</typeparam>
    /// <param name="msg">消息类型</param>
    /// <param name="listener">消息处理函数</param>
    public void AddListener<T>(string msg, UnityAction<T> listener) {
        if (!eventDic.ContainsKey(msg)) {
            eventDic.Add(msg, new MsgInfo<T>());
        }
        (eventDic[msg] as MsgInfo<T>).Action += listener;
    }
    /// <summary>
    /// 移除无参消息监听
    /// </summary>
    /// <param name="msg">消息类型</param>
    /// <param name="listener">消息处理函数</param>
    public void RemoveListener(string msg, UnityAction listener) {
        if (eventDic.ContainsKey(msg)) {
            (eventDic[msg] as MsgInfo).Action -= listener;
        }
    }
    /// <summary>
    /// 移除有参消息监听
    /// </summary>
    /// <typeparam name="T">参数类型</typeparam>
    /// <param name="msg">消息类型</param>
    /// <param name="listener">消息处理函数</param>
    public void RemoveListener<T>(string msg, UnityAction<T> listener) {
        if (eventDic.ContainsKey(msg)) {
            (eventDic[msg] as MsgInfo<T>).Action -= listener;
        }
    }
    /// <summary>
    /// 分发消息, 所有订阅该消息的实例将收到该消息
    /// </summary>
    /// <param name="msg">消息类型</param>
    public void SendMessage(string msg) {
        if (eventDic.ContainsKey(msg)) {
            (eventDic[msg] as MsgInfo).HandleMessage();
        }
    }
    /// <summary>
    /// 分发带参消息
    /// </summary>
    /// <typeparam name="T">参数类型</typeparam>
    /// <param name="msg">消息类型</param>
    /// <param name="param">参数</param>
    public void SendMessage<T>(string msg, T param) {
        if (eventDic.ContainsKey(msg)) {
            (eventDic[msg] as MsgInfo<T>).HandleMessage(param);
        }
    }

    /// <summary>
    /// 清空监听, 一般过场景时
    /// </summary>
    public void Clear() {
        eventDic.Clear();
    }
}
