using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiveObjects : MonoBehaviour
{
    public GameObject[] itemsToGive;
    public GameObject player;

    private bool alreadyGave;
    private bool collidedWithPlayer;

    // Update is called once per frame
    void Update()
    {
        if (collidedWithPlayer && Input.GetKeyDown(KeyCode.F) && !alreadyGave)
        {
            Debug.Log("Giving Items");
            foreach (GameObject item in itemsToGive) {
                Instantiate(item, player.transform.position, Quaternion.identity);
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
