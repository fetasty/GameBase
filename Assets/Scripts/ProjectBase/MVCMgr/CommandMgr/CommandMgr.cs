using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandMgr : Singleton<CommandMgr>
{
    private Dictionary<string, System.Type> cmdDic = new Dictionary<string, System.Type>();
    public void RegistEvent<T>(string eventName) where T : ICommand
    {
        if (cmdDic.ContainsKey(eventName))
        {
            throw new System.Exception($"Try to register event with name{eventName} which already exist!");
        }
        cmdDic[eventName] = typeof(T);
    }
    public void UnregistEvent(string eventName)
    {
        cmdDic.Remove(eventName);
    }
    public void HandleEvent(Notification notification)
    {
        System.Type type;
        if (cmdDic.TryGetValue(notification.eventName, out type))
        {
            (Activator.CreateInstance(type) as ICommand).execute(notification);
        }
    }
}
