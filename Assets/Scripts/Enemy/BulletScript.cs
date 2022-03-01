using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{

    public float timeAlive = 5f;
    public float bulletSpeed = 2f;
    public float bulletDamage = 5f;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * (Time.deltaTime * bulletSpeed));
        if (timeAlive <= 0)
        {
            Destroy(gameObject);
        }
        else
        {
            timeAlive -= Time.deltaTime;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<PlayerController>().curHealth -= bulletDamage;
            Destroy(gameObject);
        }
        else if (other.gameObject.layer != LayerMask.NameToLayer("Ignore Raycast") && other.gameObject.tag != "Enemy")
        {
            Destroy(gameObject);
        }
    }
}
