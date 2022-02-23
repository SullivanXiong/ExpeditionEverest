using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public bool canGrapple = false;
    public bool canClimb = false;
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
        Debug.Log(other);
        if (other.tag == "PowerUpGrappleHook")
        {
            canGrapple = true;
            Destroy(other.gameObject);
        }
        else if (other.tag == "PowerUpClimbingPick")
        {
            canClimb = true;
            Destroy(other.gameObject);
        }
    }
}
