using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Ground")
        {
            DamageEnemies();
        }
    }

    void DamageEnemies()
    {
        GameObject[] enemies =  GetAllEnemies();

        foreach (GameObject enemy in enemies)
        {
            BaseEnemyScript baseEnemyScript = enemy.GetComponent<BaseEnemyScript>();
            baseEnemyScript.DamageEnemy(1000);
        }
    }

    GameObject[] GetAllEnemies()
    {
        return GameObject.FindGameObjectsWithTag("Enemy");
    }
}
