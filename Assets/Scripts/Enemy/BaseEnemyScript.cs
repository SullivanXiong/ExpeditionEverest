using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class BaseEnemyScript : MonoBehaviour
{
    [Header("Sound Effects")]
    private AudioSource audioSrc;
    public AudioClip hitSound;
    public float hitSoundVol = 1f;
    public AudioClip deathSound;
    public float deathSoundVol = 1f;
    public AudioClip grappledSound;
    public float grappledSoundVol = 1f;

    [Header("Particle Systems")]
    public ParticleSystem hurtParticleSystem;
    public ParticleSystem stunParticleSystem;

    [Header("Reference GameObjects")]
    public GameObject player;
    public GameObject mainCamera;
    public GameObject enemyUIPrefab;
    private GameObject enemyUI;
    public GameObject enemyModel;
    private CapsuleCollider enemyCollider;

    // reference scripts
    private Slider enemyHealthSlider;
    private Rigidbody enemyBody;
    private NewController playerController;
    private NavMeshAgent enemyAgent;
    private Animator enemyAnimator;

    // tracking variables
    [Header("Tracking Variables")]
    public float curHealth;
    public bool isRagdolled;
    public bool isGrappled;
    public bool inGetUpState;
    private bool needStartGetUp;
    private bool isStunned;
    public bool isGrounded;
    public RaycastHit groundHit;
    public bool isDead;
    public bool isNavAgent;
    public int numGrapples = 0;
    public int numGetUpCompletions = 0;

    private float pullToDist;
    private GameObject pullToWhat;
    private float pullToSpeed;

    [Header("Enemy Variables")]
    public float maxHealth;
    public float stunTime;
    public float deathLength = 10f; // how long enemy execute death
    public float gettingUpLength = 2.083f;


    // Start is called before the first frame update
    void Start()
    {
        // instantiate enemy UI
        enemyUI = Instantiate(enemyUIPrefab, new Vector3(transform.position.x, transform.position.y + 3.5f, transform.position.z), Quaternion.identity);

        // get reference scripts
        enemyHealthSlider = enemyUI.transform.Find("EnemyHealth").GetComponent<Slider>();
        enemyBody = gameObject.GetComponent<Rigidbody>();
        playerController = player.GetComponent<NewController>();
        enemyAgent = gameObject.GetComponent<NavMeshAgent>();
        enemyAnimator = enemyModel.GetComponent<Animator>();
        enemyCollider = gameObject.GetComponent<CapsuleCollider>();
        audioSrc = gameObject.GetComponent<AudioSource>();

        curHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        isNavAgent = enemyAgent.enabled;
        enemyHealthSlider.value = curHealth;

        if (!isDead && curHealth <= 0 && isGrounded)
        {
            StartCoroutine(KillEnemy());
        }

        if (!isDead && !inGetUpState && !isGrappled)
        {
            if (needStartGetUp)
            {
                if (isGrounded)
                {
                    StartCoroutine(GetUpWithAnimation());
                    needStartGetUp = false;
                }
            }
        }

        if (!isDead)
        {
            AdjustUIAndGround();
        }

        Debug.DrawRay(transform.position, new Vector3(0f, -1f, 0f) * 3f, Color.red);
    }

    IEnumerator KillEnemy()
    {
        audioSrc.PlayOneShot(deathSound, deathSoundVol);

        isDead = true;

        SwitchOffRagdoll();
        Destroy(enemyUI);

        enemyCollider.enabled = false; // turn off collider
        enemyAnimator.SetBool("isDead", true);

        yield return new WaitForSeconds(deathLength);

        Destroy(gameObject);
    }

    public void FixedUpdate()
    {
        isGrounded = Physics.Raycast(transform.position, new Vector3(0, -1, 0), out groundHit, 3f);

        if (!isDead)
        {
            if (isGrappled)
            {
                if (Vector3.Distance(transform.position, pullToWhat.transform.position) > pullToDist)
                {
                    // we know that if we're grappled we're a rigidbody, so set the velocity
                    Vector3 pullDirection = pullToWhat.transform.position - transform.position;
                    enemyBody.velocity = pullDirection * pullToSpeed;
                }
                // grapple end condition is set by the grappling hook script
            }

            if (!isGrappled && isRagdolled)
            {
                // apply gravity consistent with player gravity
                enemyBody.AddForce(Vector3.up * (playerController.gravity - Physics.gravity.y));
            }
        }
    }

    private void AdjustUIAndGround()
    {
        enemyUI.transform.position = new Vector3(transform.position.x, transform.position.y + 3.5f, transform.position.z);
        enemyUI.transform.LookAt(mainCamera.transform.position);
    }

    public void DamageEnemy(float damage)
    {
        if (curHealth - damage < 0)
        {
            curHealth = 0;
        }
        else
        {
            curHealth -= damage;
        }

        if (curHealth > 0)
        {
            audioSrc.PlayOneShot(hitSound, hitSoundVol);
        }
        hurtParticleSystem.Play();
    }

    public void RagdollEnemy()
    {
        enemyAgent.enabled = false;
        enemyBody.isKinematic = false;

        isRagdolled = true;
    }

    public void SwitchOffRagdoll()
    {
        enemyBody.isKinematic = true;
        isRagdolled = false;
    }

    public void HitByGrapple(float pullInDistance, float pullSpeed, GameObject pullTo)
    {
        audioSrc.PlayOneShot(grappledSound, grappledSoundVol);

        numGrapples += 1;

        if (inGetUpState)
        {
            inGetUpState = false;
        }

        RagdollEnemy();

        // look at what we're getting pulled to
        transform.LookAt(pullTo.transform.position);

        isGrappled = true;
        enemyAnimator.SetBool("isGrappled", true);

        pullToDist = pullInDistance;
        pullToWhat = pullTo;
        pullToSpeed = pullSpeed;
    }

    public void StopBeingGrappled()
    {
        enemyBody.velocity = new Vector3(0f, -0.1f, 0f); // have to set y velocity to a small negative value
        isGrappled = false;
        enemyAnimator.SetBool("isGrappled", false);

        needStartGetUp = true;
    }

    private IEnumerator GetUpWithAnimation()
    {
        inGetUpState = true;
        enemyAnimator.SetTrigger("getUp");

        ReOrient();

        yield return new WaitForSeconds(gettingUpLength); // this wait should be the length of the get up animation
        numGetUpCompletions += 1;

        if (numGetUpCompletions == numGrapples)
        {
            inGetUpState = false;
            enemyAgent.enabled = true;
            numGetUpCompletions = 0;
            numGrapples = 0;
        }


    }

    private void ReOrient()
    {
        SwitchOffRagdoll();

        Vector3 reorientVector = transform.forward;
        reorientVector.y = 0f;

        transform.position = new Vector3(transform.position.x, groundHit.point.y + transform.localScale.y, transform.position.z);
        transform.rotation = Quaternion.LookRotation(reorientVector);

        //check if our rotation indicates we need to play our getting up animation, or can we just reposition slightly and continue
        //if (Mathf.Abs(transform.rotation.eulerAngles.x) > 20 || Mathf.Abs(transform.rotation.eulerAngles.z) > 20)
        //{
        //    reorientRotation = transform.up;
        //    reorientRotation.y = 0f; // dont want to include y direction
        //}
        //else
        //{
        //    reorientRotation = transform.forward;
        //    reorientRotation.y = 0f; // dont want to include y direction
        //}

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Kills"))
        {
            Destroy(enemyUI);
            Destroy(gameObject);
        }
    }
}
