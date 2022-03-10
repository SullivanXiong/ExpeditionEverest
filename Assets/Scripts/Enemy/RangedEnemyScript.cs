using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RangedEnemyScript : MonoBehaviour
{

    [Header("Sound Effects")]
    private AudioSource audioSrc;
    public AudioClip gunshotSound;
    public float gunshotSoundVol = 1f;

    [Header("Reference GameObjects")]
    public GameObject player;
    public GameObject bulletPrefab;
    public GameObject gunBarrel;
    public GameObject enemyModel;

    // reference scripts
    private BaseEnemyScript baseEnemyScript;
    private NavMeshAgent enemyNavAgent;
    private Animator enemyAnimator;
    private NewController playerController;

    [Header("Ranged Enemy Variables")]
    public float detectRange = 5f;
    public float avoidRange = 5f;
    public float fireCooldown = 1f;
    public float curFireCooldown;

    // tracking
    private bool hasFoundDestination;

    // Start is called before the first frame update
    void Start()
    {
        baseEnemyScript = gameObject.GetComponent<BaseEnemyScript>();
        enemyNavAgent = gameObject.GetComponent<NavMeshAgent>();
        enemyAnimator = enemyModel.GetComponent<Animator>();
        audioSrc = gameObject.GetComponent<AudioSource>();
        playerController = gameObject.GetComponent<NewController>();

        curFireCooldown = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (!baseEnemyScript.isDead)
        {
            if (curFireCooldown > 0f)
            {
                curFireCooldown -= Time.deltaTime;
            }

            if (enemyNavAgent.enabled && enemyNavAgent.velocity.magnitude > 0)
            {
                enemyAnimator.SetBool("isMoving", true);
            }
            else
            {
                enemyAnimator.SetBool("isMoving", false);
            }

            if (!baseEnemyScript.inGetUpState && !baseEnemyScript.isRagdolled && !baseEnemyScript.isGrappled) // if not in a state where enemy is active
            {
                if (Vector3.Distance(transform.position, player.transform.position) <= detectRange)
                {
                    enemyAnimator.SetBool("playerInRange", true);

                    // look at the player
                    Vector3 lookAtPosition = player.transform.position;
                    lookAtPosition.y = transform.position.y;
                    transform.LookAt(lookAtPosition);

                    if (Vector3.Distance(transform.position, player.transform.position) <= avoidRange)
                    {
                        if (!hasFoundDestination)
                        {
                            MoveToRandomPosition();
                        }
                    }
                    else
                    {
                        hasFoundDestination = false;
                    }

                    //// set the destination to the player's position
                    //enemyNavAgent.destination = player.transform.position;
                    //enemyNavAgent.destination = startPos;

                    TryAttack();

                }
                else
                {
                    enemyAnimator.SetBool("playerInRange", false);
                }
            }
        }
    }

    void MoveToRandomPosition()
    {
        Vector3 randomDirection = Random.insideUnitSphere * detectRange;
        randomDirection += transform.position;

        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, detectRange, 1);
        NavMeshPath path = new NavMeshPath();
        if (enemyNavAgent.CalculatePath(hit.position, path))
        {
            enemyNavAgent.SetDestination(hit.position);
            hasFoundDestination = true;
        }
        else
        {
            hasFoundDestination = false;
        }
    }

    void TryAttack()
    {
        if (curFireCooldown <= 0f)
        {
            // if we have a line of fire to the player
            RaycastHit aimOut;
            if (Physics.Raycast(gunBarrel.transform.position, (player.transform.position - gunBarrel.transform.position).normalized, out aimOut, detectRange))
            {
                if (aimOut.transform.tag == "Player")
                {
                    audioSrc.PlayOneShot(gunshotSound, gunshotSoundVol);
                    Instantiate(bulletPrefab, gunBarrel.transform.position, Quaternion.LookRotation(player.transform.position - gunBarrel.transform.position));
                    curFireCooldown = fireCooldown;
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectRange);
        Gizmos.DrawRay(gunBarrel.transform.position, (player.transform.position - gunBarrel.transform.position).normalized * detectRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, avoidRange);

    }
}
