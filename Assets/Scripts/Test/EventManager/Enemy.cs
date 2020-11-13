using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public string type = "哥布林";
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("enemy die!!!");
            EventManager.Instance.TriggerEvent<Enemy>("EnemyDie", this);
            EventManager.Instance.TriggerEvent("AddScore");
        }
    }
}
