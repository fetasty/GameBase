using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// һ������, ���һ�����͵�GameObject
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
            // ����ͬһ����Դ��Ӧ�ö�ʱ���ڶ�μ���
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
                // �ûص��ر�����, �ⲿ��ȷ�ϲ�����null
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
/// �����
/// ʹ��ע��: �ӻ���ػ�ȡ����Ϸ����, �ű���ʼ��Ӧ�÷���OnEnable��
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
    /// �ӻ����ȡһ����Ϸ����, ���������Զ�����, ȡ��ʱ�Զ�����
    /// </summary>
    /// <typeparam name="T">��������</typeparam>
    /// <param name="resName">������Դ��, �����ResourcesĿ¼��·����</param>
    /// <param name="callback">������ص�</param>
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
    /// �黹��Ϸ���󵽻����
    /// </summary>
    /// <param name="obj">�黹�Ķ���</param>
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
    /// ���һ��'����'����Ϸ����
    /// </summary>
    /// <param name="resName">��Ϸ��������/��������</param>
    public void Clear(string resName)
    {
        if (boxDic.ContainsKey(resName))
        {
            boxDic.Remove(resName);
        }
    }
    /// <summary>
    /// ������������
    /// </summary>
    public void ClearAll()
    {
        boxDic.Clear();
    }
}
