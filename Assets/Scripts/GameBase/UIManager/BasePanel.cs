using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class BasePanel : MonoBehaviour {
    // <类型名称, <控件物体名称, 控件>>
    protected Dictionary<string, Dictionary<string, UIBehaviour>> controlDic;
    protected virtual void Awake() {
        controlDic = new Dictionary<string, Dictionary<string, UIBehaviour>>();
        BindButtons();
        BindToggles();
    }
    /// <summary>
    /// 返回panel以及子节点中类型为T, 名字为name的控件
    /// </summary>
    /// <typeparam name="T">控件类型</typeparam>
    /// <param name="name">控件名称</param>
    /// <returns>找到的控件, 未找到则返回null</returns>
    public T GetControl<T>(string name) where T : UIBehaviour {
        System.Type type = typeof(T);
        if (!controlDic.ContainsKey(type.Name)) {
            FindControls<T>();
        }
        if (controlDic[type.Name].ContainsKey(name)) {
            return controlDic[type.Name][name] as T;
        }
        return null;
    }
    /// <summary>
    /// 在当前panel以及所有子节点中找到所有对应类型的控件并存放在字典中
    /// 重复调用则刷新字典
    /// </summary>
    /// <typeparam name="T">需要查找的控件类型</typeparam>
    protected void FindControls<T>() where T : UIBehaviour {
        System.Type type = typeof(T);
        if (!controlDic.ContainsKey(type.Name)) {
            controlDic[type.Name] = new Dictionary<string, UIBehaviour>();
        }
        controlDic[type.Name].Clear();
        var controls = GetComponentsInChildren<T>(true);
        for (int i = 0; i < controls.Length; ++i) {
            controlDic[type.Name].Add(controls[i].name, controls[i]);
        }
    }
    /// <summary>
    /// 为所有按钮绑定事件
    /// </summary>
    protected void BindButtons() {
        FindControls<Button>();
        var btnDic = controlDic[nameof(Button)];
        foreach (string name in btnDic.Keys) {
            (btnDic[name] as Button).onClick.AddListener(() => { OnButtonClick(name); });
        }
    }
    /// <summary>
    /// 为所有Toggle绑定事件
    /// </summary>
    protected void BindToggles() {
        FindControls<Toggle>();
        var toggleDic = controlDic[nameof(Toggle)];
        foreach (string name in toggleDic.Keys) {
            (toggleDic[name] as Toggle).onValueChanged.AddListener((value) => {
                OnToggleChanged(name, value);
            });
        }
    }
    /// <summary>
    /// 按钮点击回调
    /// </summary>
    /// <param name="btnName">被点击的按钮控件名字</param>
    protected virtual void OnButtonClick(string btnName) { }
    /// <summary>
    /// Toggle状态改变
    /// </summary>
    /// <param name="toggleName">Toggle名称</param>
    /// <param name="value">Toggle的选中状态</param>
    protected virtual void OnToggleChanged(string toggleName, bool value) { }
    /// <summary>
    /// panel被显示时调用
    /// </summary>
    public virtual void OnShow() { }
    /// <summary>
    /// panel被隐藏时调用
    /// </summary>
    public virtual void OnHide() { }
}
