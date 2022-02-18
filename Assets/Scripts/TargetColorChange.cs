using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetColorChange : MonoBehaviour
{
    public Color newColor;


    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("HERE");
        transform.GetComponent<Renderer>().material.color = newColor;
    }
}
