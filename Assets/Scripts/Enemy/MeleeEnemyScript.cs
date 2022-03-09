using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MeleeEnemyScript : MonoBehaviour
{
    // reduce enemy speed as they get lower on health, positive feedback loop

    [Header("Sound Effects")]
    private AudioSource audioSrc;
    public AudioClip attackGruntSound;
    public float attackGruntSoundVol = 1f;

    [Header("Reference GameObjects")]
    public GameObject player;
    public GameObject enemyModel;

    // reference scripts
    private BaseEnemyScript baseEnemyScript;
    private NavMeshAgent enemyNavAgent;
    private Animator enemyAnimator;
    private NewController playerController;

    [Header("Melee Enemy Variables")]
    public float detectRange = 40f;
    public float attackRange = 5f;
    public float damageDeal = 10f;

    [Header("This should equal our transition + attack animation time")]
    public float attackCooldown = 1f;

    [Header("Tracking Variables - DO NOT MODIFY")]
    public bool isAttacking;
    public bool playerInRange;

    // Start is called before the first frame update
    void Start()
    {
        baseEnemyScript = gameObject.GetComponent<BaseEnemyScript>();
        enemyNavAgent = gameObject.GetComponent<NavMeshAgent>();
        enemyAnimator = enemyModel.GetComponent<Animator>();
        playerController = player.GetComponent<NewController>();

        audioSrc = gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(transform.position, (player.transform.position - transform.position).normalized * attackRange);

        if (enemyNavAgent.enabled && enemyNavAgent.velocity.magnitude > 0)
        {
            enemyAnimator.SetBool("isMoving", true);
        }
        else
        {
            enemyAnimator.SetBool("isMoving", false);
        }

        playerInRange = PlayerDetected();

        if (!baseEnemyScript.isDead && !isAttacking)
        {
            if (!baseEnemyScript.inGetUpState && !baseEnemyScript.isRagdolled && !baseEnemyScript.isGrappled) // if not in a state where enemy is active
            {
                if (PlayerInAttackRange())
                {
                    enemyNavAgent.isStopped = true;
                    if (!isAttacking)
                    {
                        StartCoroutine(TryAttack());
                    }
                }
                else if (PlayerDetected())
                {
                    enemyNavAgent.isStopped = false;
                    enemyNavAgent.SetDestination(player.transform.position);
                }
            }
        }
    }

    IEnumerator TryAttack()
    {
        Vector3 lookAtPosition = player.transform.position;
        lookAtPosition.y = transform.position.y;
        transform.LookAt(lookAtPosition);

        isAttacking = true;
        enemyAnimator.SetTrigger("punch");

        yield return new WaitForSeconds(attackCooldown / 2);

        audioSrc.PlayOneShot(attackGruntSound, attackGruntSoundVol); // play grunt sound

        RaycastHit attackHit;
        // raycast in front of enemy to see if player still there
        if (Physics.Raycast(transform.position, (player.transform.position - transform.forward).normalized, out attackHit, attackRange))
        {
            Debug.Log(attackHit.transform);
            if (attackHit.transform.tag == "Player")
            {
                playerController.DealDamage(damageDeal);
            }
        }

        yield return new WaitForSeconds(attackCooldown / 2);

        isAttacking = false;
        Debug.Log(isAttacking);
    }

    bool PlayerInAttackRange()
    {
        if (Vector3.Distance(transform.position, player.transform.position) <= attackRange)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    bool PlayerDetected()
    {
        if (Vector3.Distance(transform.position, player.transform.position) <= detectRange)
        {
            enemyAnimator.SetBool("playerInRange", true);
            return true;
        }
        else
        {
            enemyAnimator.SetBool("playerInRange", false);
            return false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
