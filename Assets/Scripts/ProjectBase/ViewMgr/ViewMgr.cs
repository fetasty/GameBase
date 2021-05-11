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
    private bool isLoading;

    public ViewMgr()
    {
        viewDic = new Dictionary<string, BaseView>();
        GameObject obj = ResMgr.Instance.Load<GameObject>(ResCanvas);
        Object.DontDestroyOnLoad(obj);
        canvas = obj.transform as RectTransform;
        bottom = canvas.Find(LayerBottom) as RectTransform;
        middle = canvas.Find(LayerMiddle) as RectTransform;
        top = canvas.Find(LayerTop) as RectTransform;
        system = canvas.Find(LayerSystem) as RectTransform;
        obj = ResMgr.Instance.Load<GameObject>(ResEventSystem);
        Object.DontDestroyOnLoad(obj);
        isLoading = false;
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
            throw new System.ArgumentNullException("");
        }
        if (viewDic.ContainsKey(viewName))
        {
            BaseView view = viewDic[viewName];
            ShowView(view, layer);
        }
        else
        {
            lock(this)
            {
                if (isLoading)
                {
                    Debug.LogWarning($"Try to show view {viewName} whitch is loading!");
                    return;
                }
                isLoading = true;
            }
            ResMgr.Instance.LoadAsync<GameObject>(viewName, (viewObj) =>
            {
                lock(this) { isLoading = false; }
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
                viewDic[viewName] = view;
                ShowView(view, layer);
            });
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
        view.gameObject.SetActive(true);
        view.OnShow();
    }
    public void Hide(string viewName)
    {
        if (viewDic.ContainsKey(viewName))
        {
            BaseView view = viewDic[viewName];
            if (view.gameObject.activeSelf)
            {
                view.OnHide();
                view.gameObject.SetActive(false);
            }
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
