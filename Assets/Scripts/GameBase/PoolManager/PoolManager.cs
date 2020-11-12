using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class PoolData
{
    private Transform parent = null;
    private Queue<GameObject> objQueue = new Queue<GameObject>();

    /// <summary>
    /// 通过名称和缓存池根节点构造一个PoolData, 自动创建一个以name命令的子节点挂在在root下
    /// </summary>
    /// <param name="name">对象名称 (Resources中的资源路径)</param>
    /// <param name="root">缓存池节点</param>
    public PoolData(string name, Transform root)
    {
        var parentObj = new GameObject(name);
        this.parent = parentObj.transform;
        this.parent.SetParent(root);
    }

    /// <summary>
    /// 从这个类型的缓存池对象中取得一个对应类型对象
    /// </summary>
    /// <returns>对应类型对象</returns>
    public GameObject GetObject()
    {
        GameObject obj = null;
        if (objQueue.Count > 0)
        {
            obj = this.objQueue.Dequeue();
            obj.transform.SetParent(null);
            obj.SetActive(true);
        }
        else
        {
            var template = Resources.Load<GameObject>(this.parent.name);
            obj = GameObject.Instantiate<GameObject>(template);
            obj.name = this.parent.name;
        }
        return obj;
    }

    /// <summary>
    /// 将对象压入PoolData中
    /// </summary>
    /// <param name="obj">需要缓存的对象</param>
    public void PushObject(GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.SetParent(this.parent);
        this.objQueue.Enqueue(obj);
    }
}

public class PoolManager: SingletonBase<PoolManager>
{
    public static string PoolName = "Pool";
    private Dictionary<string, PoolData> poolDic = new Dictionary<string, PoolData>();
    private Transform root = null;

    public PoolManager()
    {
        var rootObj = new GameObject(PoolName);
        root = rootObj.transform;
    }

    /// <summary>
    /// 从缓存池中获取一个对象
    /// </summary>
    /// <param name="name">Resources中的资源名称, 名称包含相对目录, 用于动态创建对象, 也作为key</param>
    /// <returns>对象池中获取的对象</returns>
    public GameObject GetObject(string name)
    {
        if (!poolDic.ContainsKey(name))
        {
            var poolData = new PoolData(name, this.root);
            poolDic.Add(name, poolData);
        }
        return poolDic[name].GetObject();
    }

    /// <summary>
    /// 将一个对象压入缓存池
    /// </summary>
    /// <param name="name">对象名称, 作为key</param>
    /// <param name="obj">压入的对象</param>
    public void PushObject(string name, GameObject obj)
    {
        if (!poolDic.ContainsKey(name))
        {
            var poolData = new PoolData(name, this.root);
            poolDic.Add(name, poolData);
        }
        poolDic[name].PushObject(obj);
    }

    /// <summary>
    /// 清空缓存池
    /// </summary>
    public void Clear()
    {
        this.poolDic.Clear();
    }
}
