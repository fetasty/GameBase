using PureMVC.Patterns.Mediator;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class BaseView : MonoBehaviour
{
    public event UnityAction awakeEvent;
    public event UnityAction destroyEvent;
    public event UnityAction onShowEvent;
    public event UnityAction onHideEvent;
    public event UnityAction<string> btnClickEvent;
    public event UnityAction<string, bool> toggleChangedEvent;
    public event UnityAction<string, float> sliderChangedEvent;
    public event UnityAction<string, string> inputChangedEvent;

    private Dictionary<string, List<UIBehaviour>> controlDic;
    /// <summary>
    /// 子类重写Awake, 需要调用父类Awake, 之后可以注册绑定Mediator
    /// </summary>
    protected virtual void Awake()
    {
        controlDic = new Dictionary<string, List<UIBehaviour>>();
        FindControlsInChildren<Button>();
        FindControlsInChildren<Toggle>();
        FindControlsInChildren<Slider>();
        FindControlsInChildren<InputField>();
        FindControlsInChildren<Text>();
        FindControlsInChildren<Image>();
        if (awakeEvent != null) { awakeEvent.Invoke(); }
    }
    /// <summary>
    /// 子类重写Destroy, 调用父类Destroy, 之后可以删除Mediator
    /// </summary>
    protected virtual void OnDestroy()
    {
        if (destroyEvent != null) { destroyEvent.Invoke(); }
    }
    public T Get<T>(string name) where T : UIBehaviour
    {
        if (controlDic.ContainsKey(name))
        {
            foreach (var control in controlDic[name])
            {
                if (control is T) { return control as T; }
            }
        }
        return null;
    }
    public virtual void OnShow()
    {
        if (onShowEvent != null) { onShowEvent.Invoke(); }
    }
    public virtual void OnHide()
    {
        if (onHideEvent != null) { onHideEvent.Invoke(); }
    }
    protected virtual void FindControlsInChildren<T>()
        where T : UIBehaviour
    {
        T[] controls = GetComponentsInChildren<T>();
        foreach (var control in controls)
        {
            string name = control.name;
            if (controlDic.ContainsKey(name))
            {
                controlDic[name].Add(control);
            }
            else
            {
                controlDic[name] = new List<UIBehaviour>() { control };
            }
            BindControlEvent(control);
        }
    }
    protected virtual void BindControlEvent(UIBehaviour control)
    {
        string name = control.name;
        if (control is Button)
        {
            (control as Button).onClick.AddListener(() =>
            {
                if (btnClickEvent != null) { btnClickEvent.Invoke(name); }
            });
        }
        else if (control is Toggle)
        {
            (control as Toggle).onValueChanged.AddListener((value) =>
            {
                if (toggleChangedEvent != null)
                {
                    toggleChangedEvent.Invoke(name, value);
                }
            });
        }
        else if (control is Slider)
        {
            (control as Slider).onValueChanged.AddListener((value) =>
            {
                if (sliderChangedEvent != null)
                {
                    sliderChangedEvent.Invoke(name, value);
                }
            });
        }
        else if (control is InputField)
        {
            (control as InputField).onValueChanged.AddListener((value) =>
            {
                if (inputChangedEvent != null)
                {
                    inputChangedEvent.Invoke(name, value);
                }
            });
        }
    }
}
