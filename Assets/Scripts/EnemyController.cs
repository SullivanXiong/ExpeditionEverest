using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{

    // TODO so they try to get back to their original position

    public float maxHealth = 100f;
    public float curHealth;
    public float destroyDistanceY = -50;
    private Vector3 startPos;

    public float movementSpeed = 15f;

    public GameObject player;
    public GameObject bullet;
    public float attackDetectDistance = 100f;
    public float fireCooldown;
    public float coolDownTrack;
    private bool isAttacking;

    public bool isStunned;
    public float stunDuration = 2f;
    public float curStuntime;
    public GameObject stunnedParticles;

    // Start is called before the first frame update
    void Start()
    {
        curHealth = maxHealth;
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {

        if (curStuntime <= 0)
        {
            isStunned = false;
        }
        else
        {
            curStuntime -= Time.deltaTime;
        }

        if (curHealth <= 0 || transform.position.y <= destroyDistanceY)
        {
            Destroy(gameObject);
        }

        if (!isStunned)
        {

            if (transform.position != startPos)
            {
                Debug.Log("Moving Enemy Back");
                transform.position = Vector3.MoveTowards(transform.position, startPos, Time.deltaTime * movementSpeed);
            }

            stunnedParticles.SetActive(false);

            if (transform.position != startPos)
            {

            }

            if (isAttacking == true && coolDownTrack > 0)
            {
                coolDownTrack -= Time.deltaTime;
            }
            if (isAttacking == true && coolDownTrack <= 0)
            {
                isAttacking = false;
            }

            if (Vector3.Distance(player.transform.position, transform.position) <= attackDetectDistance)
            {
                AttackPlayer();
            }
        }
        else
        {
            stunnedParticles.SetActive(true);
        }
    }

    void AttackPlayer()
    {
        transform.LookAt(player.transform);
        if (isAttacking == false)
        {
            Instantiate(bullet, transform.position, transform.rotation);
            isAttacking = true;
            coolDownTrack = fireCooldown;
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, attackDetectDistance);
    }
}
