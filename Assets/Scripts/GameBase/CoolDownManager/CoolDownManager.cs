using System.Collections.Generic;
using UnityEngine;

public class CoolDownManager : Singleton<CoolDownManager> {
    private Dictionary<string, float> cdDic;
    public CoolDownManager() {
        cdDic = new Dictionary<string, float>();
    }
    /// <summary>
    /// 检查对应key是否可用, 如果可用, 则返回true, 并且key进入CD
    /// </summary>
    /// <param name="key">字符串的key</param>
    /// <param name="seconds">key的CD时间(秒), 小于0表示永久CD, 只能手动解除</param>
    /// <returns>若key可用(不在CD中)则返回true; 若key在CD中, 则返回false</returns>
    public bool CheckAndSetCoolDown(string key, float seconds = -1) {
        if (!cdDic.ContainsKey(key)) {
            SetCoolDown(key, seconds);
            return true;
        }
        float overTime = cdDic[key];
        if (overTime < 0) { return false; }
        if (Time.time > overTime) {
            SetCoolDown(key, seconds);
            return true;
        }
        return false;
    }
    /// <summary>
    /// 在对应key上设置CD, 若存在对应key, 则使用新的seconds重新计算时间
    /// </summary>
    /// <param name="key">需要设置的CD</param>
    /// <param name="seconds">CD时间(秒), 小于0表示永久CD, 只能手动解除</param>
    public void SetCoolDown(string key, float seconds = -1) {
        if (seconds < 0) {
            cdDic[key] = seconds;
        } else {
            cdDic[key] = seconds + Time.time;
        }
    }
    /// <summary>
    /// 取消key的CD
    /// </summary>
    /// <param name="key">需要解除CD的key</param>
    public void UnsetCoolDown(string key) {
        cdDic.Remove(key);
    }
}
