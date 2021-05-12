using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MessageCenter : Singleton<MessageCenter>
{
    private Dictionary<string, Delegate> handleDic;
    public MessageCenter()
    {
        handleDic = new Dictionary<string, Delegate>();
    }
    // 添加无参消息监听
    public void AddHandle(string msgName, UnityAction handle)
    {
        OnAddHandle(msgName, handle);
        if (handleDic.ContainsKey(msgName))
        {
            handleDic[msgName] = handleDic[msgName] as UnityAction + handle;
        }
        else
        {
            handleDic[msgName] = handle;
        }
    }
    // 添加带参消息监听
    public void AddHandle<T>(string msgName, UnityAction<T> handle)
    {
        OnAddHandle(msgName, handle);
        if (handleDic.ContainsKey(msgName))
        {
            handleDic[msgName] = handleDic[msgName] as UnityAction<T> + handle;
        }
        else
        {
            handleDic[msgName] = handle;
        }
    }
    // 移除消息监听
    public void RemoveHandle(string msgName, UnityAction handle)
    {
        OnRemoveHandle(msgName, handle);
        handleDic[msgName] = (handleDic[msgName] as UnityAction) - handle;
        OnRemovedHandle(msgName);
    }
    // 移除带参消息监听
    public void RemoveHandle<T>(string msgName, UnityAction<T> handle)
    {
        OnRemoveHandle(msgName, handle);
        handleDic[msgName] = (handleDic[msgName] as UnityAction<T>) - handle;
        OnRemovedHandle(msgName);
    }
    // 发送无参消息
    public void Send(string msgName)
    {
        OnSend(msgName);
        if (handleDic.ContainsKey(msgName))
        {
            Delegate d = handleDic[msgName];
            if (!(d is UnityAction))
            {
                throw new Exception($"Send msg {msgName} with out args. " +
                    $"But type of handles is {d.GetType().Name}");
            }
            (d as UnityAction).Invoke();
        }
    }
    // 发送带参消息
    public void Send<T>(string msgName, T arg)
    {
        OnSend(msgName);
        if (handleDic.ContainsKey(msgName))
        {
            Delegate d = handleDic[msgName];
            if (!(d is UnityAction<T>))
            {
                throw new Exception($"Send msg {msgName} with arg {arg.GetType().Name}. " +
                    $"But type of handles is {d.GetType().Name}");
            }
            (d as UnityAction<T>).Invoke(arg);
        }
    }
    // 清空某个消息的所有监听
    public void Clear(string msgName)
    {
        if (handleDic.ContainsKey(msgName))
        {
            handleDic.Remove(msgName);
        }
    }
    // 清空所有监听
    public void ClearAll()
    {
        handleDic.Clear();
    }

    private void OnAddHandle(string msgName, Delegate handle)
    {
        if (msgName == null || handle == null)
        {
            throw new ArgumentNullException("Params not allowed null!");
        }
        if (handleDic.ContainsKey(msgName))
        {
            Delegate d = handleDic[msgName];
            if (d != null && d.GetType() != handle.GetType())
            {
                throw new Exception($"Attempting to add handle to {msgName} " +
                    $"with type {handle.GetType().Name}, but " +
                    $"the type of existing handles is {d.GetType().Name}");
            }
        }
    }

    private void OnRemoveHandle(string msgName, Delegate handle)
    {
        if (msgName == null || handle == null)
        {
            throw new ArgumentNullException("Params not allowed null!");
        }
        if (handleDic.ContainsKey(msgName))
        {
            Delegate d = handleDic[msgName];
            if (d == null)
            {
                throw new Exception($"Attempting to remove handle of {msgName}, " +
                    $"but handles at the key is null!");
            }
            if (d.GetType() != handle.GetType())
            {
                throw new Exception($"Attempting to remove handle of {msgName} " +
                    $"with type {handle.GetType().Name}, but " +
                    $"the type of existing handles is {d.GetType().Name}");
            }
        }
        else
        {
            throw new Exception($"Attempting to remove handle of {msgName}, " +
                $"but handle map do not contains this key!");
        }
    }

    private void OnRemovedHandle(string msgName)
    {
        if (handleDic.ContainsKey(msgName) && handleDic[msgName] == null)
        {
            handleDic.Remove(msgName);
        }
    }

    private void OnSend(string msgName)
    {
        if (msgName == null)
        {
            throw new ArgumentNullException("MsgName can not be null!");
        }
    }
}
