using System;
using System.ComponentModel.Design;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Notification
{
    public Notification(string eventName, object param = null)
    {
        this.eventName = eventName;
        this.param = param;
    }
    public string eventName;
    public object param = null;
}

public class MVCMgr : Singleton<MVCMgr>
{
    private Dictionary<string, UnityAction> handleDic;
    #region Cmd 相关
    public void RegistEvent<T, K>(string eventName) where T : ICommand where K : class
    {
        CommandMgr.Instance.RegistEvent<T>(eventName);
        UnityAction<K> handler = (param) => {
            HandleEvent<K>(eventName, param);
        };
        MessageCenter.Instance.AddHandle<K>(eventName, handler);
        handleDic[eventName] = () => {
            MessageCenter.Instance.RemoveHandle<K>(eventName, handler);
        };
    }
    public void RegistEvent<T>(string eventName) where T : ICommand
    {
        CommandMgr.Instance.RegistEvent<T>(eventName);
        UnityAction handler = () => {
            HandleEvent(eventName);
        };
        MessageCenter.Instance.AddHandle(eventName, handler);
        handleDic[eventName] = () => {
            MessageCenter.Instance.RemoveHandle(eventName, handler);
        };
    }
    public void UnregistEvent(string eventName)
    {
        CommandMgr.Instance.UnregistEvent(eventName);
        if (handleDic.ContainsKey(eventName))
        {
            handleDic[eventName].Invoke();
            handleDic.Remove(eventName);
        }
    }
    public void UnregistAllEvents()
    {
        foreach (var item in handleDic)
        {
            CommandMgr.Instance.UnregistEvent(item.Key);
            item.Value.Invoke();
        }
        handleDic.Clear();
    }
    public void HandleEvent<K>(string eventName, K param) where K : class
    {
        Notification notification = new Notification(eventName, param);
        // Command
        CommandMgr.Instance.HandleEvent(notification);
        // View
        ViewMgr.Instance.HandleEvent(notification);
    }
    public void HandleEvent(string eventName)
    {
        Notification notification = new Notification(eventName);
        // Command
        CommandMgr.Instance.HandleEvent(notification);
        // View
        ViewMgr.Instance.HandleEvent(notification);
    }
    #endregion

    #region Model 相关
    public void RegistModel(BaseModel model)
    {
        ModelMgr.Instance.RegistModel(model);
    }
    public void UnregistModel(string modelName)
    {
        ModelMgr.Instance.UnregistModel(modelName);
    }
    public T GetModel<T>(string modelName) where T : BaseModel
    {
        return ModelMgr.Instance.GetModel<T>(modelName);
    }
    #endregion

    #region View 相关
    /// <summary>
    /// 显示一个View, 不存在时自动异步加载显示
    /// </summary>
    /// <param name="viewName">View名称, Resources下的View路径</param>
    /// <param name="layer">显示层级</param>
    public void ShowView(string viewName, E_ViewLayer layer = E_ViewLayer.Middle)
    {
        ViewMgr.Instance.Show(viewName, layer);
    }
    /// <summary>
    /// 隐藏View, 不销毁; 若已经隐藏, 则无动作
    /// </summary>
    /// <param name="viewName">View名称</param>
    public void HideView(string viewName)
    {
        ViewMgr.Instance.Hide(viewName);
    }
    /// <summary>
    /// 隐藏并销毁View
    /// </summary>
    /// <param name="viewName">View名称</param>
    public void DestroyView(string viewName)
    {
        ViewMgr.Instance.Destroy(viewName);
    }
    /// <summary>
    /// 获取已经创建的View
    /// </summary>
    /// <param name="viewName">View名称</param>
    /// <typeparam name="T">View类型</typeparam>
    /// <returns>对应View对象, 若尚未创建, 则返回null</returns>
    public T GetView<T>(string viewName) where T : BaseView
    {
        return ViewMgr.Instance.GetView<T>(viewName);
    }
    #endregion
}
