using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InputMgr : Singleton<InputMgr>
{
    private HashSet<KeyCode> listenKeySet;
    private bool listenKey = true;
    private bool listenMouse = true;
    private float mouseX = 0f;
    private float mouseY = 0f;
    private float mouseScroll = 0f;
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
    public float MouseX { get => mouseX; }
    public float MouseY { get => mouseY; }
    public float MouseScroll { get => mouseScroll; }
    public InputMgr()
    {
        listenKeySet = new HashSet<KeyCode>();
        MonoMgr.Instance.AddUpdateAction(Update);
    }
    public void AddListenKey(KeyCode key)
    {
        listenKeySet.Add(key);
    }
    public void AddListenKeys(params KeyCode[] keys)
    {
        foreach(KeyCode key in keys)
        {
            listenKeySet.Add(key);
        }
    }
    public void RemoveListenKey(KeyCode key)
    {
        listenKeySet.Remove(key);
    }
    public void RemoveListenKeys(params KeyCode[] keys)
    {
        foreach(KeyCode key in keys)
        {
            listenKeySet.Remove(key);
        }
    }
    public void RemoveAllListenKey()
    {
        listenKeySet.Clear();
    }
    private void Update()
    {
        if (listenKey)
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
            mouseX = Input.GetAxis("Mouse X");
            mouseY = Input.GetAxis("Mouse Y");
            mouseScroll = Input.GetAxis("Mouse ScrollWheel");
        }
    }
}
