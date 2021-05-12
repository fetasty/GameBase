using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoReturn : MonoBehaviour
{
    void Start()
    {
    }

    private void OnEnable()
    {
        MonoMgr.Instance.Invoke(ReturnSelf, 3);
    }

    private void ReturnSelf()
    {
        PoolMgr.Instance.Return(gameObject);
    }
}
