using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{

    public float timeAlive = 5f;
    public float bulletSpeed = 2f;
    public float bulletDamage = 5f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

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
        Debug.Log("BulletHit");
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<PlayerController>().curHealth -= bulletDamage;
            Destroy(gameObject);
        }
    }
}
