using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckGrounded : MonoBehaviour
{
    public GameObject player;
    private PlayerController playerScript;

    private void Start()
    {
        playerScript = player.GetComponent<PlayerController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.gameObject.layer != LayerMask.NameToLayer("TriggerCollider"))
        {
            playerScript.isGrounded = true;
            Debug.Log("TRUE");
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.gameObject.layer != LayerMask.NameToLayer("TriggerCollider"))
        {
            playerScript.isGrounded = false;
            Debug.Log("FALSE");
        }
    }

}
