using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{

    public GameObject player;
    private Rigidbody enemyBody;
    public GameObject enemyUI;
    public GameObject enemyUILookAt;
    private PlayerController playerScript;
    public bool isGrappled;

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
            Debug.Log("Ahhhhh");
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
