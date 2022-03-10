using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiveObjects : MonoBehaviour
{
    public GameObject[] itemsToGive;
    public GameObject player;
    public GameObject itemSpawnArea;
    public Vector3 startSpawn;
    public float offset;

    private bool alreadyGave;
    private bool collidedWithPlayer;


    private void Start()
    {
        startSpawn = itemSpawnArea.transform.position;
        startSpawn.x = startSpawn.x - itemSpawnArea.transform.localScale.x;
    }

    // Update is called once per frame
    void Update()
    {
        if (collidedWithPlayer && Input.GetKeyDown(KeyCode.F) && !alreadyGave)
        {
            Debug.Log("Giving Items");
            foreach (GameObject item in itemsToGive) {
                Instantiate(item, startSpawn + new Vector3(offset, 0f, 0f), Quaternion.identity);
                offset += 5;
            }
            alreadyGave = true;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            collidedWithPlayer = true;
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            collidedWithPlayer = false;
        }
    }
}
