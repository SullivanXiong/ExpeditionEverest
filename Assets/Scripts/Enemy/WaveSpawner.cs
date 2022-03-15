using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public string name;
        public Transform enemy;
        public int count;
        public float rate;
    }

    public GameObject player;
    public GameObject mainCamera;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SpawnWave(Wave _wave)
    {
        for (int i = 0; i < _wave.count; i++)
        {
            SpawnEnemy(_wave.enemy);
        }
    }

    void SpawnEnemy(Transform _enemy)
    {
        Vector3 _sp = transform.position + new Vector3(Random.Range(-10, 10), 1, Random.Range(-10, 10));
        Transform enemyInstance = Instantiate(_enemy, _sp, transform.rotation);
        GameObject enemyObject = enemyInstance.gameObject;
        BaseEnemyScript baseScript = enemyObject.GetComponent<BaseEnemyScript>();

        baseScript.player = player;
        baseScript.mainCamera = mainCamera;

        if (enemyObject.name == "EnemyMelee(Clone)")
        {
            MeleeEnemyScript meleeScript = enemyObject.GetComponent<MeleeEnemyScript>();
            meleeScript.player = player;
        }
        else if (enemyObject.name == "EnemyRanged(Clone)")
        {
            RangedEnemyScript rangedScript = enemyObject.GetComponent<RangedEnemyScript>();
            rangedScript.player = player;
        }
    }
}
