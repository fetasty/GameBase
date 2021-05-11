using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InputMgr : Singleton<InputMgr>
{
    private HashSet<KeyCode> listenKeySet;
    private bool listenKey;
    private bool listenMouse;
    public bool ListenKey
    {
        get => listenKey;
        set => listenKey = value;
    }
    public bool ListenMouse
    {
        get => listenMouse;
        set => listenMouse = value;
    }
    public InputMgr()
    {
        listenKeySet = new HashSet<KeyCode>();
        MonoMgr.Instance.AddUpdateAction(Update);
        listenKey = false;
        listenMouse = false;
    }
    public void AddListenKey(KeyCode key)
    {
        lock(this)
        {
            listenKeySet.Add(key);
        }
    }
    public void RemoveListenKey(KeyCode key)
    {
        lock(this)
        {
            listenKeySet.Remove(key);
        }
    }
    public void RemoveAllListenKey()
    {
        lock(this)
        {
            listenKeySet.Clear();
        }
    }
    private void Update()
    {
        if (listenKey)
        {
            lock (this)
            {
                foreach (var key in listenKeySet)
                {
                    if (Input.GetKeyDown(key))
                    {
                        MessageCenter.Instance.Send<KeyCode>(BaseConst.Msg_KeyDown, key);
                    }
                    if (Input.GetKeyUp(key))
                    {
                        MessageCenter.Instance.Send<KeyCode>(BaseConst.Msg_KeyUp, key);
                    }
                }
            }
        }
        if (listenMouse)
        {
            for (int i = 0; i < 3; ++i)
            {
                if (Input.GetMouseButtonDown(i))
                {
                    MessageCenter.Instance.Send<int>(BaseConst.Msg_MouseDown, i);
                }
                if (Input.GetMouseButtonUp(i))
                {
                    MessageCenter.Instance.Send<int>(BaseConst.Msg_MouseUp, i);
                }
            }
        }
    }
}
