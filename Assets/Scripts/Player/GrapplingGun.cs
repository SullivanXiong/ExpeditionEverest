using UnityEngine;

public class GrapplingGun : MonoBehaviour
{

    private LineRenderer lr;
    private RaycastHit grappleHit;
    private Vector3 lrAttachPt;
    private bool didHit;
    private SpringJoint joint;
    private PlayerController playerScript;

    public Transform gunTip, rayCastOrigin, player;
    public float grappleDistance = 100f;

    void Start()
    {
        lr = GetComponent<LineRenderer>();
        lr.SetPosition(0, gunTip.transform.position);
        lr.SetPosition(1, gunTip.transform.position);

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
            lr.SetPosition(0, gunTip.transform.position);
            lr.SetPosition(1, lrAttachPt);
            playerScript.isGrappling = true;
        }
        else
        {
            lr.SetPosition(0, gunTip.transform.position);
            lr.SetPosition(1, gunTip.transform.position);
            if (joint != null)
            {
                Destroy(joint);
            }
            playerScript.isGrappling = false;
        }

        Debug.DrawRay(rayCastOrigin.transform.position, rayCastOrigin.transform.forward * grappleDistance, Color.green);
    }

    private void TryGrapple()
    {
        if (Physics.Raycast(rayCastOrigin.transform.position, rayCastOrigin.forward, out grappleHit, grappleDistance)
            && grappleHit.transform.gameObject.layer == LayerMask.NameToLayer("Grappleable")) // grapple hits
        {
            lrAttachPt = grappleHit.point;
            lr.SetPosition(0, gunTip.transform.position);
            lr.SetPosition(1, lrAttachPt);

            if (grappleHit.transform.tag == "Enemy")
            {
                Vector3 inFrontOfPlayer = player.transform.position + player.transform.forward * 4;

                GameObject enemy = grappleHit.transform.gameObject;
                Debug.Log("Hit Enemy");
                enemy.GetComponent<EnemyController>().isStunned = true;
                enemy.GetComponent<EnemyController>().curStuntime = enemy.GetComponent<EnemyController>().stunDuration;

                // temp means of moving enemy to player
                enemy.transform.position = inFrontOfPlayer;
            }
            else
            {
                CreateJointSwing();
            }

            // set tracking variable
            didHit = true;
        }
        else
        {
            // reset line positions
            lr.SetPosition(0, gunTip.transform.position);
            lr.SetPosition(1, gunTip.transform.position);

            // set tracking variable
            didHit = false;
        }
    }

    //void CreateJointEnemy()
    //{
    //    joint = grappleHit.transform.gameObject.AddComponent<SpringJoint>();
    //    joint.autoConfigureConnectedAnchor = false;
    //    joint.connectedAnchor = player.transform.position;

    //    float distanceFromPoint = Vector3.Distance(player.transform.position, grappleHit.transform.position);

    //    joint.maxDistance = distanceFromPoint * 0.8f;
    //    joint.minDistance = distanceFromPoint * 0.2f;

    //    joint.spring = 50f;
    //    joint.damper = 7f;
    //    joint.massScale = 4.5f;
    //}

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