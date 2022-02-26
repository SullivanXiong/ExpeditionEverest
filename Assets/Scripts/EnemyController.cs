using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{

    public GameObject player;
    private Rigidbody enemyBody;
    private PlayerController playerScript;

    public GameObject enemyUIPrefab;
    private GameObject enemyUI;
    public GameObject enemyUILookAt;
    private Slider enemyHealthSlider;

    public bool isGrappled;
    public bool needReset;
    // movement speed when enemy will get up
    public float nonMovementSpeedThres = 0.5f;

    public float maxHealth = 100f;
    public float curHealth;

    private void Start()
    {
        enemyBody = gameObject.GetComponent<Rigidbody>();
        playerScript = player.GetComponent<PlayerController>();
        enemyBody.isKinematic = true;

        enemyUI = Instantiate(enemyUIPrefab, new Vector3(transform.position.x, transform.position.y + 4, transform.position.z), Quaternion.identity);
        enemyHealthSlider = enemyUI.transform.Find("EnemyHealth").GetComponent<Slider>() ;

        curHealth = maxHealth;
    }

    private void Update()
    {
        if (curHealth <= 0)
        {
            Destroy(enemyUI);
            // Kill Enemy Here
            Destroy(gameObject);
        }

        enemyHealthSlider.value = curHealth;

        if (isGrappled == true)
        {
            needReset = true;
            //Debug.Log("Ahhhhh");
        }
        // this may be disgusting
        else if (!isGrappled && needReset && enemyBody.velocity.magnitude <= nonMovementSpeedThres && enemyBody.angularVelocity.magnitude <= nonMovementSpeedThres)
        {
            //Debug.Log("Im free...");
            RevertToNavMeshAgent();

        }

        AdjustUI();
    }

    private void RevertToNavMeshAgent()
    {
        enemyBody.isKinematic = true;
        transform.rotation = new Quaternion(0, transform.rotation.y, 0f, 0f);
        transform.position = new Vector3(transform.position.x, transform.localScale.y / 2, transform.position.z);
        gameObject.GetComponent<NavMeshAgent>().enabled = true;
        gameObject.GetComponent<EnemyFollow>().enabled = true;
        needReset = false;
    }

    private void AdjustUI()
    {
        enemyUI.transform.position = new Vector3(transform.position.x, transform.position.y + 4, transform.position.z);
        enemyUI.transform.LookAt(enemyUILookAt.transform.position);
    }

    private void FixedUpdate()
    {
        // add same gravity force as player is experiencing
        enemyBody.AddForce(Vector3.down * -Physics.gravity.y * (playerScript.gravityMultiplier - 1));
    }

}
