using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveUser : MonoBehaviour
{
    public WaveSpawner[] waveSpawners;
    public WaveSpawner.Wave[] wave;

    private int waveCount;

    // Start is called before the first frame update
    void Start()
    {
        waveCount = wave.Length;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.tag);
        if (other.tag == "Player")
        {
            SpawnEnemiesFromSpawnPoints();
        }
    }

    // Trigger (can be any trigger)
    public void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision " + collision.collider.tag);
        if  (collision.collider.tag == "Player")
        {
            SpawnEnemiesFromSpawnPoints();
        }
    }

    void SpawnEnemiesFromSpawnPoints()
    {
        if (waveCount > 0)
        {
            for (int i = 0; i < waveSpawners.Length; i++)
            {
                waveSpawners[i].SpawnWave(wave[wave.Length - waveCount]);
            }
            waveCount--;
        }
    }
}
