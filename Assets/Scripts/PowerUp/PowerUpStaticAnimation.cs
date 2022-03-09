using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpStaticAnimation : MonoBehaviour
{
    public float rotationSpeed = 1f;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(transform.up, rotationSpeed);
    }
}
