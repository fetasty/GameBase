using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class BaseView : MonoBehaviour, IView
{
    public string Name { get; set; }
    public event UnityAction destroyEvent;
    public event UnityAction showEvent;
    public event UnityAction hideEvent;
    public event UnityAction<string> btnClickEvent;
    public event UnityAction<string, bool> toggleChangedEvent;
    public event UnityAction<string, float> sliderChangedEvent;
    public event UnityAction<string, string> inputChangedEvent;

    private Dictionary<string, List<UIBehaviour>> controlDic
        = new Dictionary<string, List<UIBehaviour>>();
    private HashSet<string> listeningEvents = new HashSet<string>();
    /// <summary>
    /// 子类重写Awake, 需要调用父类Awake
    /// </summary>
    protected virtual void Awake()
    {
        FindControlsInChildren<Button>();
        FindControlsInChildren<Toggle>();
        FindControlsInChildren<Slider>();
        FindControlsInChildren<InputField>();
        FindControlsInChildren<Text>();
        FindControlsInChildren<Image>();
        FindControlsInChildren<ScrollRect>();
        string[] events = LisnteningEvents();
        for (int i = 0; i < events.Length; ++i)
        {
            listeningEvents.Add(events[i]);
        }
    }
    /// <summary>
    /// 子类重写Destroy, 调用父类Destroy
    /// </summary>
    protected virtual void OnDestroy()
    {
        if (destroyEvent != null) { destroyEvent.Invoke(); }
    }
    public virtual void OnShow()
    {
        if (showEvent != null) { showEvent.Invoke(); }
    }
    public virtual void OnHide()
    {
        if (hideEvent != null) { hideEvent.Invoke(); }
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
    public bool IsListening(string eventName)
    {
        return listeningEvents.Contains(eventName);
    }
    public abstract void HandleEvent(Notification notification);
    protected abstract string[] LisnteningEvents();
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
