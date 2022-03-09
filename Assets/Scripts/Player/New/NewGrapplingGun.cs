using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewGrapplingGun : MonoBehaviour
{
    // line drawer for grappling hook
    private LineRenderer lineRenderer;
    private SpringJoint springJoint;

    // reference to what the raycast for the grapple hit (if any)
    private RaycastHit grappleHit;

    // script reference variables
    private NewController playerController;

    [Header("Reference Objects")]
    public GameObject player;
    public Camera mainCamera; // this is just the point at center of screen
    public GameObject grappleSrc;
    public GameObject camFollowTarget;

    [Header("Grapple Settings")]
    public float grappleRange = 5f;

    [Header("Distance of Camera to Reference")]
    public float cameraDistance = 8f;

    // tracking variables
    private bool isGrappling;

    // Start is called before the first frame update
    void Start()
    {
        // get reference scripts
        lineRenderer = gameObject.GetComponent<LineRenderer>();
        playerController = player.GetComponent<NewController>();

        // set init booleans
        isGrappling = false;

        ResetLineRenderer();
    }

    // Update is called once per frame
    void Update()
    {
        playerController.isGrappling = isGrappling;
        //Debug.Log(isGrappling);

        if (isGrappling)
        {
            if (Input.GetKey(KeyCode.Mouse0))
            {
                Ascend();
            }
        }

        if (playerController.canGrapple) // only try anything if we CAN grapple
        {
            if (Input.GetKeyDown(KeyCode.Mouse1) && !isGrappling)
            {
                TryGrapple();
            }
            if (Input.GetKey(KeyCode.Mouse1) && isGrappling)
            {
                ContinueGrapple();
            }
            else
            {
                StopGrapple();
            }
        }
    }

    private void LateUpdate()
    {
        if (isGrappling && playerController.isCharControllerOn && !playerController.isRigidBodyOn)
        {
            // this ensures the beginning part of grappling hook is always at the grappleSrc position while grappling
            // goes in late update so player position can update before this is changed
            lineRenderer.SetPosition(0, grappleSrc.transform.position);
        }
    }

    // this does the same thing as fixed update just for when we are in rigidbody state
    private void FixedUpdate()
    {
        if (isGrappling && !playerController.isCharControllerOn && playerController.isRigidBodyOn)
        {
            lineRenderer.SetPosition(0, grappleSrc.transform.position);
        }
    }

    void ContinueGrapple()
    {
        int i = 0;
        // switch player to rigidbody if they are a character controller that is not grounded
        if (playerController.isCharControllerOn && !playerController.isRigidBodyOn && !playerController.isCharControllerGrounded)
        {
            playerController.SwitchToRigidbody();
        }
    }

    void TryGrapple()
    {
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f)); // this creates a ray at the center of the camera view
        if (Physics.Raycast(ray.origin + camFollowTarget.transform.TransformDirection(new Vector3(0f, 0f, cameraDistance)), ray.direction, out grappleHit, grappleRange)
            && grappleHit.transform.gameObject.layer == LayerMask.NameToLayer("Grappleable"))
        {
            // switch player to rigidbody if they are a character controller that is not grounded
            if (playerController.isCharControllerOn && !playerController.isRigidBodyOn && !playerController.isCharControllerGrounded) {
                playerController.SwitchToRigidbody();
            }

            // create a joint swing
            CreateJointSwing(grappleHit.point);

            isGrappling = true;
        }
    }

    void StopGrapple()
    {
        if (isGrappling)
        {
            // get the velocity from the last swing, USED IN PLAYER AIR CONTROL
            playerController.lastSwingReleaseVelocity = playerController.playerBody.velocity;
        }

        isGrappling = false;

        if (springJoint != null)
        {
            Destroy(springJoint);
        }

        ResetLineRenderer();
    }

    private void Ascend()
    {
        Vector3 moveVector = (grappleHit.point - grappleSrc.transform.position).normalized;
        float distanceFromPoint = Vector3.Distance(player.transform.position, grappleHit.point);

        if (distanceFromPoint > 4f)
        {
            player.GetComponent<Rigidbody>().velocity += moveVector * 0.18f;
        }
        springJoint.maxDistance = distanceFromPoint * 0.2f;
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
