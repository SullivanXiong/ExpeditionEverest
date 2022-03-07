using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerUp : MonoBehaviour
{
    private NewController playerController;

    // these are legacy booleans
    public bool canClimb;
    public bool canGrapple;

    public GameObject grappleImage;
    public GameObject pickImage;


    // Start is called before the first frame update
    void Start()
    {
        playerController = gameObject.GetComponent<NewController>();

        grappleImage.SetActive(false);
        pickImage.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "PowerUpGrappleHook")
        {
            canClimb = true; // not necessary for new player controller
            playerController.canGrapple = true;
            grappleImage.SetActive(true);
            Destroy(other.gameObject);
        }
        else if (other.tag == "PowerUpClimbingPick")
        {
            canGrapple = true; // not necessary for new player controller
            playerController.canClimb = true;
            pickImage.SetActive(true);
            Destroy(other.gameObject);
        }
    }
}
