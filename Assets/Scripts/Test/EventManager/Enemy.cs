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
            MessageCenter.Instance.SendMessage<Enemy>("EnemyDie", this);
            MessageCenter.Instance.SendMessage("AddScore");
        }
    }
}
