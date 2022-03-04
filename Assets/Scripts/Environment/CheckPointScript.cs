using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointScript : MonoBehaviour
{
    private bool checkPointActive = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && checkPointActive)
        {
            Debug.Log("Player Reached Checkpoint");
            other.gameObject.GetComponent<PlayerController>().startPos = other.gameObject.transform.position;
            checkPointActive = false;
        }
    }
}
