using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public enum E_UI_LAYER {
    Bottom,
    Middle,
    Top,
    System,
}

public class UIManager : Singleton<UIManager> {
    private const string BASE_PATH = "UI/";
    private const string CANVAS_PATH = "UI/Canvas";
    private const string EVENTSYSTEM_PATH = "UI/EventSystem";
    private const string BOTTOM = "Bottom";
    private const string MIDDLE = "Middle";
    private const string TOP = "Top";
    private const string SYSTEM = "System";
    private const float MAX_LOAD_SECONDS = 3;
    private const int MAX_CACHE = 10; // 缓存的panel个数, 同时显示panel个数最好不超过该值

    private Transform canvas;
    private Transform bottom;
    private Transform middle;
    private Transform top;
    private Transform system;

    private Dictionary<string, BasePanel> panelDic;
    private List<string> cacheList;

    public Transform Canvas { get { return canvas; } }
    public UIManager() {
        var obj = ResourceManager.Instance.LoadResource<GameObject>(EVENTSYSTEM_PATH);
        Object.DontDestroyOnLoad(obj);
        obj = ResourceManager.Instance.LoadResource<GameObject>(CANVAS_PATH);
        Object.DontDestroyOnLoad(obj);
        canvas = obj.transform;
        bottom = canvas.Find(BOTTOM);
        middle = canvas.Find(MIDDLE);
        top = canvas.Find(TOP);
        system = canvas.Find(SYSTEM);
        panelDic = new Dictionary<string, BasePanel>();
        cacheList = new List<string>();
    }
    /// <summary>
    /// 获取层级的Transform
    /// </summary>
    /// <param name="layer">层级</param>
    /// <returns>对应层级的Transform</returns>
    public Transform GetLayerTransform(E_UI_LAYER layer) {
        switch (layer) {
            case E_UI_LAYER.Bottom:
                return bottom;
            case E_UI_LAYER.Middle:
                return middle;
            case E_UI_LAYER.Top:
                return top;
            case E_UI_LAYER.System:
                return system;
            default:
                return middle;
        }
    }

    /// <summary>
    /// 显示一个面板
    /// </summary>
    /// <typeparam name="T">继承于BasePanel的类型</typeparam>
    /// <param name="name">panel资源路径, 基础路径 "UI/"</param>
    /// <param name="layer">显示层级, 默认为Middle</param>
    /// <param name="callback">回调函数, 可以用于panel加载后的数据初始化等</param>
    public void ShowPanel<T>(string name, E_UI_LAYER layer = E_UI_LAYER.Middle,
        UnityAction<T> callback = null) where T : BasePanel {
        name = BASE_PATH + name;
        if (panelDic.ContainsKey(name)) {
            panelDic[name].gameObject.SetActive(true);
            var panel = panelDic[name] as T;
            panel.transform.SetParent(GetLayerTransform(layer));
            panel.OnShow();
            UpdateCache(name);
            if (callback != null) { callback(panel); }
        } else {
            if (!CoolDownManager.Instance.CheckAndSetCoolDown(BaseCoolDown.PanelLoad + name,
                MAX_LOAD_SECONDS)) {
                return;
            }
            ResourceManager.Instance.LoadResourceAsync<GameObject>(name, (obj) => {
                var panel = obj.GetComponent<T>();
                panelDic.Add(name, panel);
                panel.transform.SetParent(GetLayerTransform(layer));
                RectTransform rect = panel.transform as RectTransform;
                rect.localScale = Vector3.one;
                rect.anchoredPosition = Vector2.zero;
                rect.offsetMin = Vector2.zero;
                rect.offsetMax = Vector2.zero;
                panel.OnShow();
                if (callback != null) { callback(panel); }
            });
        }
    }
    /// <summary>
    /// 隐藏一个panel
    /// </summary>
    /// <param name="name">panel资源路径, 基础路径 "UI/"</param>
    public void HidePanel(string name) {
        name = BASE_PATH + name;
        if (panelDic.ContainsKey(name)) {
            panelDic[name].OnHide();
            panelDic[name].gameObject.SetActive(false);
        }
    }
    /// <summary>
    /// 明确知道不再需要一个panel时, 销毁该panel
    /// </summary>
    /// <param name="name">panel资源路径, 基础路径 "UI/"</param>
    public void DestroyPanel(string name) {
        name = BASE_PATH + name;
        if (panelDic.ContainsKey(name)) {
            Object.Destroy(panelDic[name].gameObject);
            panelDic.Remove(name);
            DeleteCache(name);
        }
    }
    /// <summary>
    /// 获取已经创建的panel实例
    /// </summary>
    /// <typeparam name="T">继承于BasePanel的类型</typeparam>
    /// <param name="name">需要获取的panel名称</param>
    /// <returns>BasePanel的实例, 未找到则返回null</returns>
    public T GetPanel<T>(string name) where T : BasePanel {
        name = BASE_PATH + name;
        if (panelDic.ContainsKey(name)) {
            return panelDic[name] as T;
        }
        return null;
    }
    /// <summary>
    /// 给UI控件添加自定义事件
    /// </summary>
    /// <param name="control">需要添加事件的控件</param>
    /// <param name="type">事件类型</param>
    /// <param name="callback">回调函数</param>
    public void AddCustomEventListener(UIBehaviour control, EventTriggerType type, UnityAction<BaseEventData> callback) {
        var trigger = control.GetComponent<EventTrigger>();
        if (trigger == null) {
            trigger = control.gameObject.AddComponent<EventTrigger>();
        }
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = type;
        entry.callback.AddListener(callback);
        trigger.triggers.Add(entry);
    }

    private void UpdateCache(string name) {
        // 找到, 则更新位置
        for (int i = 0; i < cacheList.Count; ++i) {
            if (cacheList[i] == name) {
                cacheList.RemoveAt(i);
                cacheList.Add(name);
                return;
            }
        }
        // 未找到, 则尝试插入
        while (cacheList.Count >= MAX_CACHE) {
            // 缓存超过上限, 将隐藏的panel销毁
            bool find = false;
            for (int i = 0; i < cacheList.Count; ++i) {
                if (!panelDic[cacheList[i]].gameObject.activeInHierarchy) {
                    find = true;
                    Object.Destroy(panelDic[cacheList[i]].gameObject);
                    panelDic.Remove(cacheList[i]);
                    cacheList.RemoveAt(i);
                    break;
                }
            }
            // 允许缓存超过上限
            if (!find) { break; }
            //if (!find) {
            //    DestroyPanel(cacheList[0]);
            //    cacheList.RemoveAt(0);
            //}
        }
        cacheList.Add(name);
    }

    private void DeleteCache(string name) {
        for (int i = 0; i < cacheList.Count; ++i) {
            if (cacheList[i] == name) {
                cacheList.RemoveAt(i);
                break;
            }
        }
    }
}
