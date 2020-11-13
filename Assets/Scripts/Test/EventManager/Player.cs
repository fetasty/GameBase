using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    void Start()
    {
        EventManager.Instance.AddListener<Enemy>("EnemyDie", OnEnemyDie);
        EventManager.Instance.AddListener("AddScore", OnAddScore);
    }

    private void OnDestroy() {
        EventManager.Instance.RemoveListener<Enemy>("EnemyDie", OnEnemyDie);
        EventManager.Instance.RemoveListener("AddScore", OnAddScore);
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
