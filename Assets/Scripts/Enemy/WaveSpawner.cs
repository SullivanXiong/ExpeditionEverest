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
        Instantiate(_enemy, _sp, transform.rotation);
    }
}
