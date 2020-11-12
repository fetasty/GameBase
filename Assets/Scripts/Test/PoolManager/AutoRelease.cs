using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRelease : MonoBehaviour
{
    private void OnEnable()
    {
        Invoke(nameof(Release), 1f);
    }

    void Release()
    {
        PoolManager.Instance.PushObject(this.gameObject.name,
        this.gameObject);
    }
}
