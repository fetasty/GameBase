using System.Collections.Generic;
using UnityEngine;

public class CoolDownManager : Singleton<CoolDownManager>
{
    private Dictionary<string, float> cdDic;
    public CoolDownManager()
    {
        cdDic = new Dictionary<string, float>();
        MonoManager.Instance.AddUpdateListener(Update);
    }
    ~CoolDownManager()
    {
        MonoManager.Instance.RemoveUpdateListener(Update);
    }
    public void Update()
    {
        foreach (string key in cdDic.Keys)
        {
            if (cdDic[key] >= 0)
            {
                cdDic[key] -= Time.deltaTime;
                if (cdDic[key] < 0) { cdDic.Remove(key); }
            }
        }
    }
    public bool CheckAndSetCoolDown(string key, float seconds = -1)
    {
        if (cdDic.ContainsKey(key))
        {
            return false;
        }
        cdDic.Add(key, seconds);
        return true;
    }
    public void SetCoolDown(string key, float seconds = -1)
    {
        cdDic[key] = seconds;
    }
    public void UnsetCoolDown(string key)
    {
        cdDic.Remove(key);
    }
}
