using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public interface IView
{
    #region View名称
    string Name { get; }
    #endregion

    #region 生命周期事件
    event UnityAction showEvent;
    event UnityAction hideEvent;
    event UnityAction destroyEvent;
    #endregion

    #region 基本控件事件
    event UnityAction<string> btnClickEvent;
    event UnityAction<string, bool> toggleChangedEvent;
    event UnityAction<string, float> sliderChangedEvent;
    event UnityAction<string, string> inputChangedEvent;
    #endregion

    #region View接口
    T Get<T>(string controlName) where T : UIBehaviour;
    bool IsListening(string eventName);
    void HandleEvent(Notification notification);
    #endregion
}
