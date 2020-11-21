using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ResourceManager : Singleton<ResourceManager> {
    /// <summary>
    /// 同步资源加载
    /// </summary>
    /// <typeparam name="T">资源类型</typeparam>
    /// <param name="name">资源名称(路径)</param>
    /// <returns>资源实例</returns>
    public T LoadResource<T>(string name) where T : Object {
        var asset = Resources.Load(name);
        if (asset is GameObject) {
            var obj = Object.Instantiate(asset);
            return obj as T;
        } else {
            return asset as T;
        }
    }
    /// <summary>
    /// 异步加载资源
    /// </summary>
    /// <param name="name">资源名称</param>
    /// <param name="callback">处理资源的回调函数</param>
    /// <typeparam name="T">资源类型</typeparam>
    public void LoadResourceAsync<T>(string name, UnityAction<T> callback = null)
        where T : Object {
        MonoManager.Instance.StartCoroutine(
            LoadResourceCoroutine<T>(name, callback)
        );
    }

    /// <summary>
    /// 异步加载资源的协程函数
    /// </summary>
    /// <param name="name">资源名称</param>
    /// <param name="callback">处理资源的回调函数</param>
    /// <typeparam name="T">资源类型</typeparam>
    /// <returns>协程的IEnumerator</returns>
    private IEnumerator LoadResourceCoroutine<T>(string name, UnityAction<T> callback)
        where T : Object {
        var result = Resources.LoadAsync(name);
        yield return result;
        var asset = result.asset;
        // 如果是GameObject则先实例化一下再回调
        if (asset is GameObject) {
            var obj = Object.Instantiate(asset) as T;
            if (callback != null) { callback(obj); }
        } else {
            if (callback != null) { callback(asset as T); }
        }
    }
}
