using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManagerTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PoolManager.Instance.GetObject("Test/Cube");
        }
        if (Input.GetMouseButtonDown(1))
        {
            PoolManager.Instance.GetObject("Test/Sphere");
        }
    }
}
