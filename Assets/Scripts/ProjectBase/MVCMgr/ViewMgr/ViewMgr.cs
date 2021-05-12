using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_ViewLayer
{
    Bottom,
    Middle,
    Top,
    System,
}

public class ViewMgr : Singleton<ViewMgr>
{
    private const string ResCanvas = "UI/Canvas";
    private const string ResEventSystem = "UI/EventSystem";
    private const string LayerBottom = "Bottom";
    private const string LayerMiddle = "Middle";
    private const string LayerTop = "Top";
    private const string LayerSystem = "System";

    private RectTransform canvas;
    private RectTransform bottom;
    private RectTransform middle;
    private RectTransform top;
    private RectTransform system;
    private Dictionary<string, BaseView> viewDic;
    private HashSet<string> loadingSet;

    public ViewMgr()
    {
        viewDic = new Dictionary<string, BaseView>();
        loadingSet = new HashSet<string>();
        GameObject obj = ResMgr.Instance.Load<GameObject>(ResCanvas);
        Object.DontDestroyOnLoad(obj);
        canvas = obj.transform as RectTransform;
        bottom = canvas.Find(LayerBottom) as RectTransform;
        middle = canvas.Find(LayerMiddle) as RectTransform;
        top = canvas.Find(LayerTop) as RectTransform;
        system = canvas.Find(LayerSystem) as RectTransform;
        obj = ResMgr.Instance.Load<GameObject>(ResEventSystem);
        Object.DontDestroyOnLoad(obj);
    }

    /// <summary>
    /// 显示一个View面板
    /// </summary>
    /// <param name="viewName">View名称, 也是Resources下View预制体资源相对路径</param>
    /// <param name="layer">显示在哪一层</param>
    public void Show(string viewName, E_ViewLayer layer = E_ViewLayer.Middle)
    {
        if (viewName == null)
        {
            throw new System.ArgumentNullException();
        }
        if (viewDic.ContainsKey(viewName))
        {
            BaseView view = viewDic[viewName];
            ShowView(view, layer);
        }
        else
        {
            CreateAsync(viewName);
        }
    }
    public void CreateAsync(string viewName, bool isShow = true, E_ViewLayer layer = E_ViewLayer.Middle)
    {
        if (loadingSet.Contains(viewName))
        {
            Debug.LogWarning($"Try to show view {viewName} whitch is loading!");
            return;
        }
        loadingSet.Add(viewName);
        ResMgr.Instance.LoadAsync<GameObject>(viewName, (viewObj) =>
        {
            loadingSet.Remove(viewName);
            if (viewObj == null)
            {
                Debug.LogWarning($"Load view {viewName} failed!");
                return;
            }
            BaseView view = viewObj.GetComponent<BaseView>();
            if (view == null)
            {
                Debug.LogWarning($"View {viewName} do not have BaseView component!");
                return;
            }
            view.Name = viewName;
            viewDic[viewName] = view;
            // view销毁时从字典中移除 (重要)
            view.destroyEvent += () =>
            {
                if (viewDic.ContainsKey(viewName)) { viewDic.Remove(viewName); }
            };
            if (isShow)
            {
                ShowView(view, layer);
            }
        });
    }
    public void Hide(string viewName)
    {
        BaseView view;
        if (viewDic.TryGetValue(viewName, out view))
        {
            if (view.gameObject.activeSelf)
            {
                view.OnHide();
                view.gameObject.SetActive(false);
            }
        }
    }
    /// <summary>
    /// 获取已创建, 但尚未销毁的View对象, 未找到则返回null
    /// </summary>
    /// <param name="viewName">View名称</param>
    /// <typeparam name="T">类型</typeparam>
    /// <returns>View对象或者null</returns>
    public T GetView<T>(string viewName) where T : BaseView
    {
        BaseView view = null;
        viewDic.TryGetValue(viewName, out view);
        return view as T;
    }
    public void Destroy(string viewName)
    {
        BaseView view;
        if (viewDic.TryGetValue(viewName, out view))
        {
            Hide(viewName);
            Object.Destroy(view.gameObject);
        }
    }
    public void Clear()
    {
        foreach (string key in viewDic.Keys)
        {
            if (viewDic[key].gameObject.activeSelf)
            {
                Hide(key);
                GameObject.Destroy(viewDic[key].gameObject);
            }
        }
        viewDic.Clear();
    }
    public void HandleEvent(Notification notification)
    {
        foreach (var item in viewDic)
        {
            if (item.Value.IsListening(notification.eventName))
            {
                item.Value.HandleEvent(notification);
            }
        }
    }
    private void ShowView(BaseView view, E_ViewLayer layer)
    {
        RectTransform layerRect = GetLayerTransform(layer);
        view.transform.SetParent(layerRect);
        RectTransform rect = view.transform as RectTransform;
        rect.pivot = Vector2.one / 2f;
        rect.localPosition = Vector3.zero;
        rect.localRotation = Quaternion.identity;
        rect.localScale = Vector3.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        view.gameObject.SetActive(true);
        view.OnShow();
    }
    private RectTransform GetLayerTransform(E_ViewLayer layer)
    {
        switch (layer)
        {
            case E_ViewLayer.Bottom:
                return bottom;
            case E_ViewLayer.Middle:
                return middle;
            case E_ViewLayer.Top:
                return top;
            case E_ViewLayer.System:
                return system;
        }
        return middle;
    }
}
