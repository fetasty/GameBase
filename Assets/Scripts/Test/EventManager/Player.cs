using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    void Start()
    {
        MessageManager.Instance.AddListener<Enemy>("EnemyDie", OnEnemyDie);
        MessageManager.Instance.AddListener("AddScore", OnAddScore);
    }

    private void OnDestroy() {
        MessageManager.Instance.RemoveListener<Enemy>("EnemyDie", OnEnemyDie);
        MessageManager.Instance.RemoveListener("AddScore", OnAddScore);
    }

    void Update()
    {
        
    }

    private void OnEnemyDie(Enemy enemy)
    {
        Debug.Log("Player get enemy die: " + enemy.type);
    }

    private void OnAddScore()
    {
        Debug.Log("Player add score");
    }
}
