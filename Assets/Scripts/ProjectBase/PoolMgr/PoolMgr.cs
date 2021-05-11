using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// 一个盒子, 存放一种类型的GameObject
public class PoolBox
{
    private Transform box;
    private string resName;
    private Queue<GameObject> queue;
    private bool isResLoading;

    public PoolBox(string resName, Transform root)
    {
        this.resName = resName;
        box = new GameObject("Box-" + resName).transform;
        box.SetParent(root);
        queue = new Queue<GameObject>();
        isResLoading = false;
    }
    public void Get(UnityAction<GameObject> callback)
    {
        if (queue.Count > 0)
        {
            GameObject obj = queue.Dequeue();
            obj.transform.SetParent(null);
            obj.SetActive(true);
            callback.Invoke(obj);
        }
        else
        {
            // 对于同一个资源不应该短时间内多次加载
            lock(this)
            {
                if (isResLoading)
                {
                    Debug.LogWarning($"Requre object {resName}. But res {resName} is loading.");
                    callback.Invoke(null);
                    return;
                }
                isResLoading = true;
            }
            ResMgr.Instance.LoadAsync<GameObject>(resName, (res) =>
            {
                // 该回调必被调用, 外部已确认参数非null
                lock(this)
                {
                    isResLoading = false;
                }
                res.name = resName;
                callback.Invoke(res);
            });
        }
    }
    public void Return(GameObject obj)
    {
        obj.transform.SetParent(box);
        obj.SetActive(false);
        queue.Enqueue(obj);
    }
}

/// <summary>
/// 缓存池
/// 使用注意: 从缓存池获取的游戏对象, 脚本初始化应该放在OnEnable中
/// </summary>
public class PoolMgr : Singleton<PoolMgr>
{
    private Transform pool;
    private Dictionary<string, PoolBox> boxDic;
    public PoolMgr()
    {
        pool = new GameObject(nameof(PoolMgr)).transform;
        GameObject.DontDestroyOnLoad(pool.gameObject);
        boxDic = new Dictionary<string, PoolBox>();
    }
    /// <summary>
    /// 从缓存池取一个游戏对象, 不存在则自动创建, 取出时自动激活
    /// </summary>
    /// <typeparam name="T">对象类型</typeparam>
    /// <param name="resName">对象资源名, 相对于Resources目录的路径名</param>
    /// <param name="callback">对象处理回调</param>
    public void Get(string resName, UnityAction<GameObject> callback)
    {
        if (resName == null || callback == null)
        {
            throw new System.ArgumentNullException("Params cannot be null!");
        }
        if (!boxDic.ContainsKey(resName))
        {
            boxDic[resName] = new PoolBox(resName, pool);
        }
        boxDic[resName].Get(callback);
    }
    /// <summary>
    /// 归还游戏对象到缓存池
    /// </summary>
    /// <param name="obj">归还的对象</param>
    public void Return(GameObject obj)
    {
        if (obj == null)
        {
            throw new System.ArgumentNullException("Try to return null GameObject");
        }
        if (!boxDic.ContainsKey(obj.name))
        {
            boxDic[obj.name] = new PoolBox(obj.name, pool);
        }
        boxDic[obj.name].Return(obj);
    }
    /// <summary>
    /// 清空一个'抽屉'的游戏对象
    /// </summary>
    /// <param name="resName">游戏对象名称/抽屉名称</param>
    public void Clear(string resName)
    {
        if (boxDic.ContainsKey(resName))
        {
            boxDic.Remove(resName);
        }
    }
    /// <summary>
    /// 清空整个缓存池
    /// </summary>
    public void ClearAll()
    {
        boxDic.Clear();
    }
}
