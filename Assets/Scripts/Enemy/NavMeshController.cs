using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavMeshController : MonoBehaviour
{
    [Header("Needed References")]
    public GameObject player;
    public GameObject bullet;

    [Header("Combat Parameters")]
    public float detectionRange;
    public float fireCooldown;

    private bool canFire;
    private float curCooldown;

    // Start is called before the first frame update
    void Start()
    {
        canFire = true;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateAttackCooldown();

        // if player is within range 
        if (Vector3.Distance(gameObject.transform.position, player.transform.position) <= detectionRange)
        {
            transform.LookAt(player.transform);

            TryFire();
        }
    }

    private void TryFire()
    {
        if (canFire)
        {
            Instantiate(bullet, new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), transform.rotation);

            canFire = false;
            curCooldown = fireCooldown;
        }
    }

    private void UpdateAttackCooldown()
    {
        if (!canFire)
        {
            curCooldown -= Time.deltaTime;
        }
        if (curCooldown <= 0)
        {
            canFire = true;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
