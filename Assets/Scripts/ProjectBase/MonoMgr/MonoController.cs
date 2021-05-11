using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 只允许在MonoMgr中使用
/// </summary>
public class MonoController : MonoBehaviour
{
    public event UnityAction updateEvent;
    private void Update()
    {
        if (updateEvent != null) { updateEvent.Invoke(); }
    }
}
