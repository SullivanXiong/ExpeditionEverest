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

    [Header("Ranged Enemy Variables")]
    public float detectRange = 5f;
    public float avoidRange = 5f;
    public float fireCooldown = 1f;
    public float curFireCooldown;

    [Header("Boss details")]
    public bool isBoss = false;
    public BossActionController bossScript;

    // Start is called before the first frame update
    void Start()
    {
        baseEnemyScript = gameObject.GetComponent<BaseEnemyScript>();
        enemyNavAgent = gameObject.GetComponent<NavMeshAgent>();
        enemyAnimator = enemyModel.GetComponent<Animator>();
        audioSrc = gameObject.GetComponent<AudioSource>();

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

                    //// set the destination to the player's position
                    //enemyNavAgent.destination = player.transform.position;
                    //enemyNavAgent.destination = startPos;

                    if (isBoss) {
                        if (bossScript.attacking1) {
                            TryAttack();
                        }
                        else if (bossScript.attacking2) {
                            TryAttack2();
                        }
                    }
                    else {
                        TryAttack();
                    }
                }
                else
                {
                    enemyAnimator.SetBool("playerInRange", false);
                }

                if (Vector3.Distance(transform.position, player.transform.position) <= avoidRange)
                {
                    int i = 1;
                }
            }
        }
    }

    void TryAttack()
    {
        if (curFireCooldown <= 0f)
        {
            audioSrc.PlayOneShot(gunshotSound, gunshotSoundVol);
            Instantiate(bulletPrefab, gunBarrel.transform.position, Quaternion.LookRotation(player.transform.position - gunBarrel.transform.position));
            curFireCooldown = fireCooldown;
        }
    }

    void TryAttack2()
    {
        if (curFireCooldown <= 0f)
        {
            audioSrc.PlayOneShot(gunshotSound, gunshotSoundVol);
            Instantiate(bulletPrefab, gunBarrel.transform.position, Quaternion.LookRotation(player.transform.position - gunBarrel.transform.position));
            Instantiate(bulletPrefab, gunBarrel.transform.position, Quaternion.LookRotation(player.transform.position + new Vector3(2, 0, 2) - gunBarrel.transform.position));
            Instantiate(bulletPrefab, gunBarrel.transform.position, Quaternion.LookRotation(player.transform.position + new Vector3(-2, 0, -2) - gunBarrel.transform.position));
            Instantiate(bulletPrefab, gunBarrel.transform.position, Quaternion.LookRotation(player.transform.position + new Vector3(4, 0, 4) - gunBarrel.transform.position));
            Instantiate(bulletPrefab, gunBarrel.transform.position, Quaternion.LookRotation(player.transform.position + new Vector3(-4, 0, -4) - gunBarrel.transform.position));
            Instantiate(bulletPrefab, gunBarrel.transform.position, Quaternion.LookRotation(player.transform.position + new Vector3(6, 0, 6) - gunBarrel.transform.position));
            Instantiate(bulletPrefab, gunBarrel.transform.position, Quaternion.LookRotation(player.transform.position + new Vector3(-6, 0, -6) - gunBarrel.transform.position));
            curFireCooldown = fireCooldown;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, avoidRange);
    }
}
