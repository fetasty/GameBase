using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    void Start()
    {
        MessageCenter.Instance.AddListener<Enemy>("EnemyDie", OnEnemyDie);
    }

    private void OnDestroy() {
        MessageCenter.Instance.RemoveListener<Enemy>("EnemyDie", OnEnemyDie);
    }

    void Update()
    {
        
    }

    private void OnEnemyDie(Enemy enemy)
    {
        Debug.Log("Boss get enemy die: " + enemy.type);
    }
}
