using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ResourceManager : SingletonBase<ResourceManager>
{
    /// <summary>
    /// 异步加载资源
    /// </summary>
    /// <param name="name">资源名称</param>
    /// <param name="callback">处理资源的回调函数</param>
    /// <typeparam name="T">资源类型</typeparam>
    public void LoadResourceAsync<T>(string name, UnityAction<T> callback)
        where T : Object
    {
        if (callback == null) { return; }
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
        where T : Object
    {
        var result = Resources.LoadAsync(name);
        // 这里要循环判断是否加载完成
        while (!result.isDone)
        {
            yield return result;
        }
        var res = result.asset;
        // 如果是GameObject则先实例化一下再回调
        if (res is GameObject)
        {
            var obj = GameObject.Instantiate(res) as GameObject;
            callback(obj as T);
        }
        else
        {
            callback(res as T);
        }
    }
}
