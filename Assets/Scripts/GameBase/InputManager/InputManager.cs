using System.Collections.Generic;
using UnityEngine;

public class InputManager : Singleton<InputManager>
{
    private HashSet<KeyCode> listenKeySet = new HashSet<KeyCode>();
    public InputManager()
    {
        MonoManager.Instance.AddUpdateListener(Update);
    }

    ~InputManager()
    {
        MonoManager.Instance.RemoveUpdateListener(Update);
    }

    public void AddListenKey(KeyCode key)
    {
        listenKeySet.Add(key);
    }

    public void RemoveListenKey(KeyCode key)
    {
        listenKeySet.Remove(key);
    }
    public void Update()
    {
        CheckKey();
    }

    private void CheckKey()
    {
        foreach (var key in listenKeySet)
        {
            if (Input.GetKeyDown(key))
            {
                MessageCenter.Instance.SendMessage<KeyCode>(BaseMessage.KeyDown, key);
            }
            else if (Input.GetKeyUp(key))
            {
                MessageCenter.Instance.SendMessage<KeyCode>(BaseMessage.KeyUp, key);
            }
        }
    }
}
