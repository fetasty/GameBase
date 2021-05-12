using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ResMgr : Singleton<ResMgr>
{
    /// <summary>
    /// 同步加载资源
    /// </summary>
    /// <typeparam name="T">资源类型</typeparam>
    /// <param name="path">Resources下的相对路径</param>
    /// <returns>如果是GameObject自动实例化返回, 如果是其它资源则直接返回</returns>
    public T Load<T>(string path) where T : Object
    {
        if (path == null) { throw new System.ArgumentNullException("Path cannot be null!"); }
        T res = Resources.Load<T>(path);
        if (res is GameObject)
        {
            GameObject obj = GameObject.Instantiate(res) as GameObject;
            obj.name = path;
            return obj as T;
        }
        return res;
    }

    /// <summary>
    /// 异步加载资源
    /// </summary>
    /// <typeparam name="T">资源类型</typeparam>
    /// <param name="path">Resources下的相对路径</param>
    /// <param name="callback">处理资源的回调函数</param>
    public void LoadAsync<T>(string path, UnityAction<T> callback) where T : Object
    {
        if (path == null || callback == null)
        {
            throw new System.ArgumentNullException("Params cannot be null!");
        }
        MonoMgr.Instance.StartCoroutine(LoadAsyncCoroutine<T>(path, callback));
    }
    // 异步加载资源的协程函数
    private IEnumerator LoadAsyncCoroutine<T>(string path, UnityAction<T> callback)
        where T : Object
    {
        ResourceRequest req = Resources.LoadAsync<T>(path);
        yield return req;
        Object asset = req.asset;
        if (asset is GameObject)
        {
            GameObject obj = GameObject.Instantiate(asset) as GameObject;
            obj.name = path;
            callback.Invoke(obj as T);
        }
        else
        {
            callback.Invoke(asset as T);
        }
    }
}
