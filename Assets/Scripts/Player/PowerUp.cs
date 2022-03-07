using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerUp : MonoBehaviour
{
    public bool canGrapple = false;
    public bool canClimb = false;
    public GameObject grappleImage;
    public GameObject pickImage;


    // Start is called before the first frame update
    void Start()
    {
        grappleImage.SetActive(false);
        pickImage.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "PowerUpGrappleHook")
        {
            canGrapple = true;
            grappleImage.SetActive(true);
            Destroy(other.gameObject);
        }
        else if (other.tag == "PowerUpClimbingPick")
        {
            canClimb = true;
            pickImage.SetActive(true);
            Destroy(other.gameObject);
        }
    }
}
