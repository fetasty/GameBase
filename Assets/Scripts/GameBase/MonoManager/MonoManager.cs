using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO
class MonoInner : MonoBehaviour
{
    private void Update()
    {
        MonoManager.Instance.ManagerUpdate();
    }
}

public class MonoManager : SingletonBase<MonoManager>
{
    public void ManagerUpdate()
    {

    }
}
