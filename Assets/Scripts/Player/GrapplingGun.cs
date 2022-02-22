using UnityEngine;

public class GrapplingGun : MonoBehaviour
{

    private LineRenderer lr;
    private RaycastHit grappleHit;
    private bool didHit;
    private bool hitEnemy;
    private bool enemyCloseEnough;
    private SpringJoint joint;

    private PlayerController playerScript;
    public Transform gunTip, rayCastOrigin, player;
    public float grappleDistance = 100f;
    public float enemyPullSpeed = 10f;
    public float distancePullSeperate = 5f;

    void Start()
    {
        lr = GetComponent<LineRenderer>();
        ResetLrPos();

        playerScript = player.GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            TryGrapple();
        }

        if (Input.GetKey(KeyCode.Mouse1) && didHit == true)
        {
            if (hitEnemy == true)
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
            else
            {
                SetLrPos();
            }

            playerScript.isGrappling = true;
        }
        else
        {
            // can change this to just kill the grapple and enemy will keep their inertia
            if (hitEnemy == true)
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

        if (hitEnemy == true)
        {
            lr.SetPosition(1, grappleHit.transform.position);
        }
        else
        {
            lr.SetPosition(1, grappleHit.point);
        }
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
                enemy.GetComponent<Rigidbody>().isKinematic = false;
                enemy.GetComponent<EnemyController>().isGrappled = true;

                PullAction(enemy);

                hitEnemy = true;
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