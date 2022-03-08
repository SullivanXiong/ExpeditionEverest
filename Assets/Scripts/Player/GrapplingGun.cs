using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class GrapplingGun : MonoBehaviour
{

    private LineRenderer lr;
    private RaycastHit grappleHit;
    private bool didHit;
    private bool hitEnemy;
    private bool enemyCloseEnough;
    private SpringJoint joint;

    private PlayerController playerScript;
    private PowerUp playerPowers;
    public Transform gunTip, rayCastOrigin, player;
    public float grappleDistance = 100f;
    public float enemyPullSpeed = 10f;
    public float distancePullSeperate = 5f;
    public Slider cooldownSlider;
    public float cooldownPullEnemyMax;
    private float cooldownTrack;

    public float grappleDamage = 5f;
    public bool onCooldown;

    void Start()
    {
        lr = GetComponent<LineRenderer>();
        ResetLrPos();

        playerScript = player.GetComponent<PlayerController>();
        playerPowers = player.GetComponent<PowerUp>();
        onCooldown = false;
        cooldownTrack = cooldownPullEnemyMax;
    }

    private void Update()
    {
        if (onCooldown)
        {
            cooldownTrack -= Time.deltaTime;
        }
        if (cooldownTrack <= 0)
        {
            cooldownTrack = cooldownPullEnemyMax;
            onCooldown = false;
        }

        cooldownSlider.value = cooldownTrack / cooldownPullEnemyMax * 100;

        if (Input.GetKeyDown(KeyCode.Mouse1) && playerPowers.canGrapple && !onCooldown)
        {
            TryGrapple();
        }

        if (Input.GetKey(KeyCode.Mouse1) && didHit == true)
        {
            if (hitEnemy == true && !(grappleHit.transform == null))
            {
                enemyCloseEnough = Vector3.Distance(grappleHit.transform.position, player.transform.position) <= distancePullSeperate;

                if (!enemyCloseEnough)
                {
                    PullAction(grappleHit.transform.gameObject);
                    SetLrPos();
                }
                else if (enemyCloseEnough)
                {
                    grappleHit.transform.GetComponent<Rigidbody>().velocity = new Vector3(0, Physics.gravity.y, 0);
                    grappleHit.transform.GetComponent<EnemyController>().isGrappled = false;
                    KillGrapple();
                }
            }
            else if (hitEnemy == true && (grappleHit.transform == null))
            {
                KillGrapple();
            }
            else
            {
                SetLrPos();
            }

            playerScript.isGrappling = true;
        }
        else
        {
            // can change this to just kill the grapple and enemy will keep their inertia
            if (hitEnemy == true && !(grappleHit.transform == null))
            {
                grappleHit.transform.GetComponent<Rigidbody>().velocity = new Vector3(0, Physics.gravity.y, 0);
                grappleHit.transform.GetComponent<EnemyController>().isGrappled = false;
                KillGrapple();
            }
            else
            {
                KillGrapple();
            }
        }

        Debug.DrawRay(rayCastOrigin.transform.position, rayCastOrigin.transform.forward * grappleDistance, Color.green);
    }

    private void KillGrapple()
    {
        ResetLrPos();
        if (joint != null)
        {
            Destroy(joint);
        }
        didHit = false;
        hitEnemy = false;
        enemyCloseEnough = false;
        playerScript.isGrappling = false;
    }

    private void SetLrPos()
    {
        lr.SetPosition(0, gunTip.transform.position);

        if (Input.GetKey(KeyCode.Mouse0))
        {
            Ascend();
        }

        if (hitEnemy == true && !(grappleHit.transform == null))
        {
            lr.SetPosition(1, grappleHit.transform.position);
        }
        else
        {
            lr.SetPosition(1, grappleHit.point);
        }
    }

    private void Ascend()
    {
        Vector3 moveVector = (grappleHit.point - gunTip.transform.position).normalized;
        float distanceFromPoint = Vector3.Distance(player.transform.position, grappleHit.point);

        if (distanceFromPoint > 4f)
        {
            player.GetComponent<Rigidbody>().velocity += moveVector * 0.18f;
        }
        joint.maxDistance = distanceFromPoint * 0.8f;
    }

    private void ResetLrPos()
    {
        lr.SetPosition(0, gunTip.transform.position);
        lr.SetPosition(1, gunTip.transform.position);
    }

    private void TryGrapple()
    {
        if (Physics.Raycast(rayCastOrigin.transform.position, rayCastOrigin.forward, out grappleHit, grappleDistance)
            && grappleHit.transform.gameObject.layer == LayerMask.NameToLayer("Grappleable")) // grapple hits
        {
            if (grappleHit.transform.tag == "Enemy")
            {
                // Note this raycast may be hitting something of the enemy that doesnt have rigidbody attached, could be causing
                // bug where pulling doesn't work
                //Debug.Log(grappleHit.transform.gameObject);

                GameObject enemy = grappleHit.transform.gameObject;

                HandleEnemy(enemy);
                PullAction(enemy);

                hitEnemy = true;
                onCooldown = true;

                SetLrPos();
            }
            else
            {
                CreateJointSwing();
                hitEnemy = false;
                SetLrPos();
            }

            // set tracking variable
            didHit = true;
        }
        else
        {
            ResetLrPos();

            // set tracking variable
            hitEnemy = false;
            didHit = false;

        }
    }

    void HandleEnemy(GameObject enemy)
    {
        enemy.GetComponent<EnemyController>().curHealth -= grappleDamage;

        // disable navmesh transform
        enemy.GetComponent<NavMeshAgent>().enabled = false;
        enemy.GetComponent<NavMeshController>().enabled = false;

        enemy.GetComponent<Rigidbody>().isKinematic = false;
        enemy.GetComponent<EnemyController>().isGrappled = true;
    }

    void PullAction(GameObject enemy)
    {
        Vector3 moveVector = (player.transform.position - enemy.transform.position).normalized;
        enemy.GetComponent<Rigidbody>().velocity = moveVector * enemyPullSpeed;
    }

    void CreateJointSwing()
    {
        // create and configure the spring joint
        joint = player.gameObject.AddComponent<SpringJoint>();
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = grappleHit.point;

        float distanceFromPoint = Vector3.Distance(player.transform.position, grappleHit.point);

        // configurable
        joint.maxDistance = distanceFromPoint * 0.8f;
        joint.minDistance = distanceFromPoint * 0.25f;

        joint.spring = 4.5f;
        joint.damper = 7f;
        joint.massScale = 4.5f;
    }

}
