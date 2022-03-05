using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewController : MonoBehaviour
{
    // TODO: FIGURE OUT A WAY TO INTERPOLATE CAMERA MOVEMENT
    // SWITCHING TO RIGIDBODY REDUCES THE IN-GAME FRAMERATE BECAUSE WE HAVE TO
    // SWITCH THE CAMERA SETTING TO FIXED UPDATE TO PREVENT STUTTERING

    // script reference variables
    private CharacterController charController;
    [HideInInspector]
    public Rigidbody playerBody;
    private CapsuleCollider playerBodyCollider;
    private Cinemachine.CinemachineBrain cameraBrain;

    [Header("Reference Objects")]
    public GameObject camFollowTarget;
    public GameObject mainCamera;

    // internal tracking variables
    private Vector3 movementVector;
    private Vector2 turnVector;
    private Vector3 velocityVector;

    public bool isRigidBodyOn;
    public bool isCharControllerOn;
    public bool isRBGrounded;
    public bool isCharControllerGrounded;
    public bool isPlayerGrounded;

    public bool isClimbing;
    public bool isGrappling;

    [Header("Movement Variables")]
    public float movementSpeed = 6f;
    public float jumpHeight = 5f;
    public float grappleControlForce = 5f;

    [Header("Action Variables")]
    public float climbingReach = 5f;

    [Header("Character Controller Physics")]
    public float gravity = -9.81f;
    [System.NonSerialized]
    public Vector3 lastSwingReleaseVelocity;

    [Header("Sensitivity")]
    public float xSensitivity = 1f;
    public float ySensitivity = 1f;
    public float yClamping = 85f;


    // Start is called before the first frame update
    void Start()
    {
        // lock cursor
        Cursor.lockState = CursorLockMode.Locked;

        // get reference scripts
        charController = gameObject.GetComponent<CharacterController>();
        playerBody = gameObject.GetComponent<Rigidbody>();
        playerBodyCollider = gameObject.GetComponent<CapsuleCollider>();
        cameraBrain = mainCamera.GetComponent<Cinemachine.CinemachineBrain>();

        // turn to char controller state
        SwitchToCharController();

        // default camera to late update
        cameraBrain.m_UpdateMethod = Cinemachine.CinemachineBrain.UpdateMethod.LateUpdate;
    }

    // Update is called once per frame
    void Update()
    {
        // deactivate rigidbody when we hit the ground
        if (isRigidBodyOn && isRBGrounded) // AND on the ground
        {
            // switch back to char controller
            SwitchToCharController();
        }

        // get movement inputs
        movementVector = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical")).normalized;

        // get rotation inputs
        turnVector.x += Input.GetAxisRaw("Mouse X") * xSensitivity * Time.deltaTime;
        turnVector.y += Input.GetAxisRaw("Mouse Y") * ySensitivity * Time.deltaTime;
        turnVector.y = Mathf.Clamp(turnVector.y, -yClamping, yClamping);

        isClimbing = CheckFacingClimbable();
        //Debug.Log(isClimbing);

        if (isCharControllerOn && !isRigidBodyOn)
        {
            if (isCharControllerGrounded && velocityVector.y < 0)
            {
                // this must be 2f
                velocityVector.y = -2f;
            }

            if (Input.GetKeyDown(KeyCode.Space) && isPlayerGrounded)
            {
                velocityVector.y += Mathf.Sqrt(jumpHeight * -3.0f * gravity);
            }

            MovePlayerCharController();
            // maybe this needs to be a different method for when our player is a rigidbody
            // too lazy to try right now
            RotatePlayer();
        }

        // this has to go here
        isPlayerGrounded = UpdatePlayerGrounded();
        //Debug.Log(isPlayerGrounded);

    }

    public void FixedUpdate()
    {
        // physics engine is already applying a force of -9.81, we need to match 
        // the char controller gravity, just some math, 'gravity' is our own custom gravity
        playerBody.AddForce(Vector3.up * (gravity - Physics.gravity.y));

        if (isRigidBodyOn && !isCharControllerOn)
        {
            if (!isPlayerGrounded)
            {
                if (isGrappling)
                {
                    playerBody.AddForce(transform.TransformDirection(movementVector) * grappleControlForce, ForceMode.Acceleration);
                }
                else
                {
                    // just for holding our movement direction adjusted for the rotation of the character
                    Vector3 movVectorTransformed = transform.TransformDirection(movementVector);
                    playerBody.velocity = new Vector3(lastSwingReleaseVelocity.x + (movVectorTransformed.x * movementSpeed), playerBody.velocity.y, 
                                                        lastSwingReleaseVelocity.z + (movVectorTransformed.z * movementSpeed));
                    Debug.Log(playerBody.velocity);
                }
            }
            RotatePlayer();
        }
    }

    public void SwitchToRigidbody()
    {
        // capturing charactercontroller movement to be transferred to rigidbody
        Vector3 capturedMovement = transform.TransformDirection(movementVector) * movementSpeed;

        // edit scripts on player
        charController.enabled = false;
        playerBodyCollider.enabled = true;
        playerBody.isKinematic = false;
        playerBody.velocity = new Vector3(capturedMovement.x, velocityVector.y, capturedMovement.z);

        // edit tracking bools
        isCharControllerOn = false;
        isRigidBodyOn = true;
        isRBGrounded = false; // we will never be a rigidbody if we are on the ground

        // clear characterController velocity vector - prevents player from trying to complete the jump when switching
        // back to charcontroller
        velocityVector = new Vector3(0f, 0f, 0f);

        //edit camera mode to fixed update to smooth
        cameraBrain.m_UpdateMethod = Cinemachine.CinemachineBrain.UpdateMethod.FixedUpdate;
    }

    public void SwitchToCharController()
    {
        // edit scripts on player
        playerBodyCollider.enabled = false;
        playerBody.isKinematic = true;
        charController.enabled = true;

        // edit tracking variables
        isCharControllerOn = true;
        isRigidBodyOn = false;
        isRBGrounded = true;

        // edit camera mode to fixed update to smooth
        cameraBrain.m_UpdateMethod = Cinemachine.CinemachineBrain.UpdateMethod.LateUpdate;
    }

    void MovePlayerCharController()
    {
        // move handling
        charController.Move(transform.TransformDirection(movementVector) * movementSpeed * Time.deltaTime);

        // apply gravity
        velocityVector.y += gravity * Time.deltaTime;
        charController.Move(velocityVector * Time.deltaTime);

        // this NEEDS to go here for some reason, check if we're grounded
        isCharControllerGrounded = charController.isGrounded;
    }

    void RotatePlayer()
    {
        // rotation handling
        transform.localRotation = Quaternion.Euler(transform.localRotation.y, turnVector.x, 0);
        camFollowTarget.transform.localRotation = Quaternion.Euler(-turnVector.y, camFollowTarget.transform.localRotation.x, 0);
    }

    public bool CheckFacingClimbable()
    {
        // suspected source of stuttering climbing problem, flickering between returning true and returning false when climbing W + D or W + A and rotating
        // in strafing direction
        RaycastHit climbableHit;
        if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y - transform.localScale.y, transform.position.z), transform.forward, out climbableHit, climbingReach) ||
            Physics.Raycast(new Vector3(transform.position.x, transform.position.y + transform.localScale.y, transform.position.z), transform.forward, out climbableHit, climbingReach) ||
            Physics.Raycast(new Vector3(transform.position.x, transform.position.y, transform.position.z), transform.forward, out climbableHit, climbingReach))
        {
            if (climbableHit.transform.gameObject.layer == LayerMask.NameToLayer("Climbable"))
            {
                return true;
            }
        }
        return false;
    }

    // just collapses the ground checks into one variable
    public bool UpdatePlayerGrounded()
    {
        if (isCharControllerOn && !isRigidBodyOn && isCharControllerGrounded)
        {
            return true;
        }
        else if (isRigidBodyOn && !isCharControllerOn && isCharControllerGrounded)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // this is only running when rigidbody is activated,
    // i.e. we only need this when a rigidbody is in the air and want to check if it hit the ground
    private void OnCollisionEnter(Collision collision)
    {
        // this code may be messy on the > 0 check
        // may need to do more research/testing to see if this threshold is appropriate
        // same with OnCollisionExit
        // UPDATE: 0.05 felt more appropriate
        //if (collision.gameObject.layer == LayerMask.NameToLayer("Kills"))
        //{
        //    transform.position = startPos;
        //    curHealth = maxHealth;
        //}
        if (collision.GetContact(0).normal.y > 0.05)
        {
            isRBGrounded = true;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(new Vector3(transform.position.x, transform.position.y - transform.localScale.y, transform.position.z), transform.forward * climbingReach);
        Gizmos.DrawRay(new Vector3(transform.position.x, transform.position.y + transform.localScale.y, transform.position.z), transform.forward * climbingReach);
        Gizmos.DrawRay(new Vector3(transform.position.x, transform.position.y, transform.position.z), transform.forward * climbingReach);
    }
}
