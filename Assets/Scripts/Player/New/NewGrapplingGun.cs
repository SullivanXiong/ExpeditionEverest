using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewGrapplingGun : MonoBehaviour
{
    // line drawer for grappling hook
    private LineRenderer lineRenderer;
    private SpringJoint springJoint;

    // reference to what the raycast for the grapple hit (if any)
    private RaycastHit grappleHit;

    // script reference variables
    private NewController playerController;

    [Header("Sound Effects")]
    private AudioSource audioSrc;
    public AudioClip grappleShootSound;
    public float grappleShootSoundVol = 1f;
    public AudioClip grapplePullSound;
    public float grapplePullSoundVol = 1f;
    

    [Header("Reference Objects")]
    public GameObject player;
    public Camera mainCamera; // this is just the point at center of screen
    public GameObject grappleSrc;
    public GameObject camFollowTarget;
    public Slider cooldownSlider;

    [Header("Grapple Settings")]
    public float grappleRange = 5f;
    public float enemyPullInDistance = 5f;
    public float enemyPullInSpeed = 10f;
    public bool isAscending;
    public float ascendSpeed = 10f;
    public float minAscendDist = 5f;
    public GameObject pullToWhat;
    public float grappleDamage = 10f;
    public float enemyGrappleCooldown = 2f;
    public float curCooldown;

    [Header("Distance of Camera to Reference")]
    public float cameraDistance = 8f;

    // tracking variables
    private bool isGrappling;
    private bool hitEnemy;

    // Start is called before the first frame update
    void Start()
    {
        // get reference scripts
        lineRenderer = gameObject.GetComponent<LineRenderer>();
        playerController = player.GetComponent<NewController>();
        audioSrc = gameObject.GetComponent<AudioSource>();

        pullToWhat = player;

        // set init booleans
        isGrappling = false;
        curCooldown = enemyGrappleCooldown;

        ResetLineRenderer();
    }

    // Update is called once per frame
    void Update()
    {
        cooldownSlider.value = curCooldown / enemyGrappleCooldown * 100;
        if (curCooldown <= enemyGrappleCooldown) {
            curCooldown += Time.deltaTime;
        }

        playerController.isGrappleAscending = isAscending;
        playerController.isGrappling = isGrappling;
        playerController.isGrapplingEnemy = hitEnemy;
        //Debug.Log(isGrappling);

        if (playerController.canGrapple) // only try anything if we CAN grapple
        {
            if (Input.GetKeyDown(KeyCode.Mouse1) && !isGrappling)
            {
                TryGrapple();
            }

            if (Input.GetKey(KeyCode.Mouse1) && isGrappling)
            {
                ContinueGrapple();

                if (Input.GetKey(KeyCode.LeftShift) && !hitEnemy && Vector3.Distance(player.transform.position, grappleHit.point) > minAscendDist)
                {
                    isAscending = true;
                }
                else
                {
                    isAscending = false;
                }
            }
            else
            {
                StopGrapple();
            }
        }

        // handling ascent for player controller
        if (playerController.canGrapple && isGrappling && isAscending && playerController.isCharControllerOn) 
        {
            playerController.charController.Move((grappleHit.point - player.transform.position).normalized * ascendSpeed * Time.deltaTime);
            float distanceFromPoint = Vector3.Distance(grappleHit.point, player.transform.position);
            springJoint.maxDistance = distanceFromPoint * 0.8f;
            springJoint.minDistance = distanceFromPoint * 0.25f;
        } 
    }

    private void LateUpdate()
    {
        if (isGrappling && playerController.isCharControllerOn && !playerController.isRigidBodyOn)
        {
            // this ensures the beginning part of grappling hook is always at the grappleSrc position while grappling
            // goes in late update so player position can update before this is changed
            lineRenderer.SetPosition(0, grappleSrc.transform.position);
            if (hitEnemy)
            {
                lineRenderer.SetPosition(1, grappleHit.transform.position);
            }
        }
    }

    // this does the same thing as fixed update just for when we are in rigidbody state
    private void FixedUpdate()
    {
        if (isGrappling && !playerController.isCharControllerOn && playerController.isRigidBodyOn)
        {
            lineRenderer.SetPosition(0, grappleSrc.transform.position);
            if (hitEnemy)
            {
                lineRenderer.SetPosition(1, grappleHit.transform.position);
            }
        }

        //handling ascent for rigidbody
        if (playerController.canGrapple && isGrappling && isAscending && playerController.isRigidBodyOn)
        {
            playerController.playerBody.velocity = ((grappleHit.point - player.transform.position).normalized * ascendSpeed);
            float distanceFromPoint = Vector3.Distance(grappleHit.point, player.transform.position);
            springJoint.maxDistance = distanceFromPoint * 0.8f;
            springJoint.minDistance = distanceFromPoint * 0.25f;
        }
    }

    void ContinueGrapple()
    {
        // switch player to rigidbody if they are a character controller grappling (BUT not grappling an enemy)
        if (playerController.isCharControllerOn && !playerController.isRigidBodyOn && !playerController.isCharControllerGrounded && !hitEnemy)
        {
            playerController.SwitchToRigidbody();
        }
        if (hitEnemy && Vector3.Distance(grappleHit.transform.position, pullToWhat.transform.position) <= enemyPullInDistance)
        {
            StopGrapple();
        }
        if (hitEnemy && grappleHit.transform.gameObject.GetComponent<BaseEnemyScript>().isDead)
        {
            StopGrapple();
        }
    }

    void TryGrapple()
    {
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f)); // this creates a ray at the center of the camera view
        if (Physics.Raycast(ray.origin + camFollowTarget.transform.TransformDirection(new Vector3(0f, 0f, cameraDistance)), ray.direction, out grappleHit, grappleRange)
            && grappleHit.transform.gameObject.layer == LayerMask.NameToLayer("Grappleable"))
        {

            if (grappleHit.transform.gameObject.tag == "Enemy")
            {
                if (curCooldown <= enemyGrappleCooldown)
                {
                    return;
                }

                audioSrc.PlayOneShot(grappleShootSound, grappleShootSoundVol);

                GameObject enemy = grappleHit.transform.gameObject;
                BaseEnemyScript hitEnemyScript = enemy.GetComponent<BaseEnemyScript>();

                hitEnemyScript.DamageEnemy(grappleDamage);
                hitEnemyScript.HitByGrapple(enemyPullInDistance, enemyPullInSpeed, pullToWhat); // pulling to player gives better effect than pulling to gun
                hitEnemy = true;
                curCooldown = 0;
            }
            else
            {
                audioSrc.PlayOneShot(grappleShootSound, grappleShootSoundVol);
                // switch player to rigidbody if they are a character controller that is not grounded
                if (playerController.isCharControllerOn && !playerController.isRigidBodyOn && !playerController.isCharControllerGrounded)
                {
                    playerController.SwitchToRigidbody();
                }

                // create a joint swing
                CreateJointSwing(grappleHit.point);

                hitEnemy = false;
            }

            isGrappling = true;
        }
    }

    void StopGrapple()
    {
        if (isGrappling)
        {
            // get the velocity from the last swing, DEFUNCT
            playerController.lastSwingReleaseVelocity = playerController.playerBody.velocity;
        }

        // THIS COULD BE A PROBLEM CONDITION
        if (hitEnemy && isGrappling)
        {
            BaseEnemyScript enemyScript = grappleHit.transform.gameObject.GetComponent<BaseEnemyScript>();
            if (enemyScript.isGrappled)
            {
                // stop enemy grapple
                grappleHit.transform.gameObject.GetComponent<BaseEnemyScript>().StopBeingGrappled();
            }
        }

        if (springJoint != null)
        {
            Destroy(springJoint);
        }

        ResetLineRenderer();
        isGrappling = false;
        isAscending = false;
    }

    void ResetLineRenderer()
    {
        lineRenderer.SetPosition(0, grappleSrc.transform.position);
        lineRenderer.SetPosition(1, grappleSrc.transform.position);
    }

    void CreateJointSwing(Vector3 hitPoint)
    {
        // set line renderer position
        lineRenderer.SetPosition(1, hitPoint);

        // create and configure the spring joint
        springJoint = player.AddComponent<SpringJoint>();
        springJoint.autoConfigureConnectedAnchor = false;
        springJoint.connectedAnchor = hitPoint;

        float distanceFromPoint = Vector3.Distance(player.transform.position, hitPoint);

        // configurable 
        // TODO: WE SHOULDN'T HAVE A FORCE FOR THE PLAYER GETTING TOO CLOSE TO THE ORIGIN
        springJoint.maxDistance = distanceFromPoint * 0.8f;
        springJoint.minDistance = distanceFromPoint * 0.25f;

        springJoint.spring = 4.5f;
        springJoint.damper = 7f;
        springJoint.massScale = 4.5f;
    }

    void OnDrawGizmosSelected()
    {
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Gizmos.color = Color.green;
        Gizmos.DrawRay(ray.origin + camFollowTarget.transform.TransformDirection(new Vector3(0f, 0f, cameraDistance)), ray.direction * grappleRange);
    }
}
