using System.Collections.Generic;
using UnityEngine;

public class InputManager : Singleton<InputManager> {
    private HashSet<KeyCode> listenKeySet = new HashSet<KeyCode>();
    public InputManager() {
        MonoManager.Instance.AddUpdateListener(Update);
    }

    ~InputManager() {
        MonoManager.Instance.RemoveUpdateListener(Update);
    }
    /// <summary>
    /// 添加需要监听的按键, 对应的按键按下/弹起时触发 BaseMessage.KeyDown/KeyUp事件
    /// </summary>
    /// <param name="key">需要监听的keycode</param>
    public void AddListenKey(KeyCode key) {
        listenKeySet.Add(key);
    }
    /// <summary>
    /// 移除对应按键的监听
    /// </summary>
    /// <param name="key">键的keycode</param>
    public void RemoveListenKey(KeyCode key) {
        listenKeySet.Remove(key);
    }
    /// <summary>
    /// 每帧检测按键事件
    /// </summary>
    public void Update() {
        CheckKey();
    }
    /// <summary>
    /// 按键事件检测
    /// </summary>
    private void CheckKey() {
        foreach (var key in listenKeySet) {
            if (Input.GetKeyDown(key)) {
                MessageCenter.Instance.SendMessage<KeyCode>(BaseMessage.KeyDown, key);
            } else if (Input.GetKeyUp(key)) {
                MessageCenter.Instance.SendMessage<KeyCode>(BaseMessage.KeyUp, key);
            }
        }
    }
}
