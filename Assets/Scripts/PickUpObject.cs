using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpObject : MonoBehaviour
{
    public Transform theDest;
    private bool Holding = false;
    public float thrust;
    void Update()
    {
        if (Holding)
        {
            GetComponent<SphereCollider>().enabled = false;
            GetComponent<Rigidbody>().useGravity = false;
            this.transform.position = theDest.position;
        }

        if (Input.GetKeyDown(KeyCode.G) && (Holding == true))
        {
            Holding = false;
            GetComponent<SphereCollider>().enabled = true;
            GetComponent<Rigidbody>().useGravity = true;
            GetComponent<Rigidbody>().AddForce(theDest.forward * thrust); }
    }

    void OnMouseDown()
    {
        if (!Holding)
        {
            Holding = true;
        }
    }
}
