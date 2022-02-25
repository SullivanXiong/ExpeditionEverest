using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{

    public GameObject player;
    private Rigidbody enemyBody;
    public GameObject enemyUI;
    public GameObject enemyUILookAt;
    private PlayerController playerScript;
    public bool isGrappled;
    public bool needReset;
    // movement speed when enemy will get up
    public float nonMovementSpeedThres = 0.5f;

    private void Start()
    {
        enemyBody = gameObject.GetComponent<Rigidbody>();
        playerScript = player.GetComponent<PlayerController>();
        enemyBody.isKinematic = true;
    }

    private void Update()
    {
        if (isGrappled == true)
        {
            needReset = true;
            //Debug.Log("Ahhhhh");
        }
        // this may be disgusting
        else if (!isGrappled && needReset && enemyBody.velocity.magnitude <= nonMovementSpeedThres && enemyBody.angularVelocity.magnitude <= nonMovementSpeedThres)
        {
            //Debug.Log("Im free...");

            enemyBody.isKinematic = true;
            transform.rotation = new Quaternion(0, transform.rotation.y, 0, 0);
            transform.position = new Vector3(transform.position.x, transform.localScale.y / 2, transform.position.z);
            gameObject.GetComponent<NavMeshAgent>().enabled = true;
            gameObject.GetComponent<EnemyFollow>().enabled = true;

            needReset = false;
        }

        AdjustUI();
    }

    private void AdjustUI()
    {
        enemyUI.transform.position = new Vector3(transform.position.x, transform.position.y + 4, transform.position.z);
    }

    private void FixedUpdate()
    {
        // add same gravity force as player is experiencing
        enemyBody.AddForce(Vector3.down * -Physics.gravity.y * (playerScript.gravityMultiplier - 1));
    }

}
