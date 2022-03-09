using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewController : MonoBehaviour
{
    // TODO: FIGURE OUT A WAY TO INTERPOLATE CAMERA MOVEMENT
    // SWITCHING TO RIGIDBODY REDUCES THE IN-GAME FRAMERATE BECAUSE WE HAVE TO
    // SWITCH THE CAMERA SETTING TO FIXED UPDATE TO PREVENT STUTTERING

    [Header("Sound Effects")]
    private AudioSource audioSrc;
    public float audioSrcVolume;
    public AudioClip jumpSound;
    public float jumpSoundVol = 1f;
    public AudioClip punchSound;
    public float punchSoundVol = 0.5f;

    // script reference variables
    public CharacterController charController;
    [HideInInspector]
    public Rigidbody playerBody;
    private CapsuleCollider playerBodyCollider;
    private Cinemachine.CinemachineBrain cameraBrain;

    [Header("Reference Objects")]
    public GameObject camFollowTarget;
    public GameObject mainCamera;
    public Camera mainCameraCam;
    public Slider healthSlider;

    [Header("Distance of Camera to Reference - Check ThirdPersonCamera under 'Body'")]
    public float cameraDistance = 8f;

    [Header("Animation References")]
    public AnimationClip attackAnimation;

    [Header("Animation Variables (MUST MATCH THOSE IN ANIMATION CONTROLLER) - DETERMINES ATTACK COOLDOWN")]
    public float attackSpeedMultiplier = 1.5f;
    public float attackTransitionTime = 0.1f;
    private float attackTimeTotal;
    public bool canAttack;

    // internal tracking variables
    private Vector3 movementVector;
    private Vector2 turnVector;
    private Vector3 charContrYVelVector;

    [Header("Movement Variables")]
    public float movementSpeed = 6f;
    public float jumpHeight = 5f;
    public float jumpHeightFromWall = 3f;
    public float grappleControlForce = 5f;
    public float climbingSpeed = 3f;

    [Header("Action Variables")]
    public float climbingReach = 5f;
    public float attackReach = 5f;
    public float meleeDamage = 20f;

    [Header("Combat Variables")]
    public float maxHealth = 100f;
    //[System.NonSerialized]
    public float curHealth;

    [Header("Character Controller Physics")]
    public float gravity = -9.81f;
    public Vector3 lastSwingReleaseVelocity;

    [Header("Sensitivity")]
    public float xSensitivity = 1f;
    public float ySensitivity = 1f;
    public float yClamping = 85f;

    [Header("Reset Variables")]
    public Vector3 startPos;

    // state tracking variables
    [Header("DO NOT EDIT (only for visibility)")]
    public bool isRigidBodyOn;
    public bool isCharControllerOn;
    public bool isRBGrounded;
    public bool isCharControllerGrounded;
    public bool isPlayerGrounded;

    public bool isClimbing;
    // are we attempting to move away from the wall after climbing down?
    private bool isMovingAwayFromWall;
    public bool isGrappling;
    public bool isGrapplingEnemy;
    public bool isGrappleAscending;

    public bool canGrapple;
    public bool canClimb;

    void Start()
    {
        Time.timeScale = 1;

        // lock cursor
        Cursor.lockState = CursorLockMode.Locked;

        // set player health to max
        curHealth = maxHealth;

        // get reference scripts
        charController = gameObject.GetComponent<CharacterController>();
        playerBody = gameObject.GetComponent<Rigidbody>();
        playerBodyCollider = gameObject.GetComponent<CapsuleCollider>();
        cameraBrain = mainCamera.GetComponent<Cinemachine.CinemachineBrain>();
        audioSrc = gameObject.GetComponent<AudioSource>();
        audioSrcVolume = audioSrc.volume;

        // get total attack animation time, this will determine our attack cooldown
        attackTimeTotal = (attackAnimation.length / attackSpeedMultiplier) + attackTransitionTime;
        canAttack = true;

        // set reset variables
        startPos = transform.position;

        // turn to char controller state
        SwitchToCharController();

        // default camera to late update
        cameraBrain.m_UpdateMethod = Cinemachine.CinemachineBrain.UpdateMethod.LateUpdate;
    }

    // Update is called once per frame
    void Update()
    {
        // update health slider value
        healthSlider.value = curHealth;

        // deactivate rigidbody when we hit the ground (if we are a rigidbody and not grappleAscending)
        if (isRigidBodyOn && isRBGrounded && !isCharControllerOn && !isGrappleAscending) // AND on the ground
        {
            // switch back to char controller
            SwitchToCharController();
        }

        if (canClimb) // only update if we are ALLOWED to climb (defaulted to false)
        {
            // do all of our climbing checks
            HandleClimbing();
        }

        if (isClimbing)
        {
            // adjusts our movement input for climbing
            movementVector = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0f).normalized;
        }
        else
        {
            // get normal movement inputs
            movementVector = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical")).normalized;
        }

        // get rotation inputs
        turnVector.x += Input.GetAxisRaw("Mouse X") * xSensitivity * Time.deltaTime;
        turnVector.y += Input.GetAxisRaw("Mouse Y") * ySensitivity * Time.deltaTime;
        turnVector.y = Mathf.Clamp(turnVector.y, -yClamping, yClamping);

        if (isCharControllerOn && !isRigidBodyOn)
        {
            if (isCharControllerGrounded && charContrYVelVector.y < 0)
            {
                // this must be 2f
                charContrYVelVector.y = -2f;
            }

            // regular jumping for when we are on the ground
            if (Input.GetKeyDown(KeyCode.Space) && isPlayerGrounded && !isClimbing)
            {
                charContrYVelVector.y += Mathf.Sqrt(jumpHeight * -3.0f * gravity);
                audioSrc.PlayOneShot(jumpSound);
            }
            // jumping from a wall
            else if (Input.GetKeyDown(KeyCode.Space) && isClimbing)
            {
                isClimbing = false;
                charContrYVelVector.y += Mathf.Sqrt(jumpHeightFromWall * -3.0f * gravity);
                audioSrc.PlayOneShot(jumpSound);
            }

            MovePlayerCharController();
            // maybe this needs to be a different method for when our player is a rigidbody
            // too lazy to try right now
            if (isClimbing)
            {
                RotateCameraOnly();
            }
            else
            {
                RotatePlayer();
            }
        }

        if (Input.GetKey(KeyCode.Mouse0) && canAttack && !isClimbing)
        {
            StartCoroutine(TryAttack());
        }

        // this has to go here
        isPlayerGrounded = UpdatePlayerGrounded();
    }

    void HandleClimbing()
    {
        ClimbableHitInfo climbableInfo = CheckFacingClimbable();

        if (isMovingAwayFromWall)
        {
            if (!climbableInfo.hitClimbable)
            {
                isMovingAwayFromWall = false;
            }
        }
        else
        {
            if (climbableInfo.hitClimbable && !isGrappling)
            {
                if (isRigidBodyOn && !isCharControllerOn)
                {
                    SwitchToCharController();
                }

                // set player rotation to the normal of the climbable hit
                //Debug.Log(climbableInfo.climbableNormal);
                transform.forward = -climbableInfo.climbableNormal;

                // have to reset charControl jump velocity 
                charContrYVelVector = new Vector3(0f, 0f, 0f);

                isClimbing = true;
            }
            else
            {
                isClimbing = false;
            }
        }
    }

    private struct ClimbableHitInfo
    {
        public bool hitClimbable;
        public Vector3 climbableNormal;
    }

    private ClimbableHitInfo CheckFacingClimbable()
    {
        // suspected source of stuttering climbing problem, flickering between returning true and returning false when climbing W + D or W + A and rotating
        // in strafing direction
        RaycastHit climbableHitTop;
        RaycastHit climbableHitMiddle;
        RaycastHit climbableHitBottom;

        bool topRay = Physics.Raycast(new Vector3(transform.position.x, transform.position.y + transform.localScale.y, transform.position.z), transform.forward, out climbableHitTop, climbingReach);
        bool middleRay = Physics.Raycast(transform.position + transform.TransformDirection(new Vector3(0f, 0f, (playerBodyCollider.radius * 2))), transform.forward, out climbableHitMiddle, climbingReach - (playerBodyCollider.radius * 2));
        bool bottomRay = Physics.Raycast(new Vector3(transform.position.x, transform.position.y - transform.localScale.y, transform.position.z), transform.forward, out climbableHitBottom, climbingReach);

        if (topRay && (middleRay || bottomRay))
        {
            if (topRay && climbableHitTop.transform.gameObject.layer == LayerMask.NameToLayer("Climbable"))
            {
                Debug.DrawRay(climbableHitTop.point, climbableHitTop.normal, Color.cyan);

                ClimbableHitInfo topInfo = new ClimbableHitInfo();
                topInfo.hitClimbable = true;
                topInfo.climbableNormal = climbableHitTop.normal;

                return topInfo;
            }
            else if (middleRay && climbableHitMiddle.transform.gameObject.layer == LayerMask.NameToLayer("Climbable"))
            {
                Debug.DrawRay(climbableHitMiddle.point, climbableHitMiddle.normal, Color.cyan);

                ClimbableHitInfo middleInfo = new ClimbableHitInfo();
                middleInfo.hitClimbable = true;
                middleInfo.climbableNormal = climbableHitMiddle.normal;

                return middleInfo;
            }
            else if (bottomRay && climbableHitBottom.transform.gameObject.layer == LayerMask.NameToLayer("Climbable"))
            {
                Debug.DrawRay(climbableHitBottom.point, climbableHitBottom.normal, Color.cyan);

                ClimbableHitInfo bottomInfo = new ClimbableHitInfo();
                bottomInfo.hitClimbable = true;
                bottomInfo.climbableNormal = climbableHitBottom.normal;

                return bottomInfo;
            }
        }
        // ledge checking, if we weren't already climbing, do not check for ledge until our head reaches the wall
        else if (!topRay && (middleRay || bottomRay) && isClimbing)
        {
            if (middleRay && climbableHitMiddle.transform.gameObject.layer == LayerMask.NameToLayer("Climbable"))
            {
                Debug.DrawRay(climbableHitMiddle.point, climbableHitMiddle.normal, Color.cyan);

                ClimbableHitInfo middleInfo = new ClimbableHitInfo();
                middleInfo.hitClimbable = true;
                middleInfo.climbableNormal = climbableHitMiddle.normal;

                return middleInfo;
            }
            else if (bottomRay && climbableHitBottom.transform.gameObject.layer == LayerMask.NameToLayer("Climbable"))
            {
                Debug.DrawRay(climbableHitBottom.point, climbableHitBottom.normal, Color.cyan);

                ClimbableHitInfo bottomInfo = new ClimbableHitInfo();
                bottomInfo.hitClimbable = true;
                bottomInfo.climbableNormal = climbableHitBottom.normal;

                return bottomInfo;
            }
        }

        ClimbableHitInfo noInfo = new ClimbableHitInfo();
        noInfo.hitClimbable = false;
        noInfo.climbableNormal = new Vector3(0f, 0f, 0f);

        return noInfo;
    }

    private IEnumerator TryAttack()
    {
        // wait for animation to hit its peak, then check if attack hits
        canAttack = false;
        // wait half of the attack animation time to execute the raycast check for attack
        yield return new WaitForSeconds(attackTimeTotal / 2);

        RaycastHit attackHit;
        Ray ray = mainCameraCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.Raycast(ray.origin + camFollowTarget.transform.TransformDirection(new Vector3(0f, 0f, cameraDistance - 1)), ray.direction, out attackHit, attackReach))
        {
            if (attackHit.transform.tag == "Enemy")
            {
                audioSrc.PlayOneShot(punchSound, punchSoundVol); // play at half volume

                GameObject foundEnemy = attackHit.transform.gameObject;
                BaseEnemyScript baseEnemyScript = foundEnemy.GetComponent<BaseEnemyScript>();
                Rigidbody enemyRb = foundEnemy.GetComponent<Rigidbody>();

                baseEnemyScript.DamageEnemy(meleeDamage);
                //baseEnemyScript.RagdollEnemy();
                //enemyRb.AddForce(foundEnemy.transform.position - transform.position, ForceMode.Impulse);
            }
        }

        yield return new WaitForSeconds(attackTimeTotal / 2);

        canAttack = true;
    }

    public void DealDamage(float amount)
    {
        curHealth -= amount;
    }

    public void AddHealth(float amount)
    {
        if (curHealth + amount > maxHealth)
        {
            curHealth = maxHealth;
        }
        else
        {
            curHealth += amount;
        }
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
                if (isGrappling && !isGrapplingEnemy)
                {
                    playerBody.AddForce(transform.TransformDirection(movementVector) * grappleControlForce, ForceMode.Acceleration);
                }
                else
                {
                    // I'm not a fan of the current air control, fix this.
                    // just for holding our movement direction adjusted for the rotation of the character
                    Vector3 movVectorTransformed = transform.TransformDirection(movementVector);
                    //playerBody.velocity = new Vector3(lastSwingReleaseVelocity.x + (movVectorTransformed.x * movementSpeed), playerBody.velocity.y, 
                    //                                    lastSwingReleaseVelocity.z + (movVectorTransformed.z * movementSpeed));
                    //Debug.Log(playerBody.velocity);
                    playerBody.velocity = new Vector3((movVectorTransformed.x * movementSpeed), playerBody.velocity.y, (movVectorTransformed.z * movementSpeed));
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
        playerBody.velocity = new Vector3(capturedMovement.x, charContrYVelVector.y, capturedMovement.z);

        // edit tracking bools
        isCharControllerOn = false;
        isRigidBodyOn = true;
        isRBGrounded = false; // we will never be a rigidbody if we are on the ground

        // clear characterController velocity vector - prevents player from trying to complete the jump when switching
        // back to charcontroller
        charContrYVelVector = new Vector3(0f, 0f, 0f);

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
        if (isClimbing)
        {
            charController.Move(transform.TransformDirection(movementVector) * climbingSpeed * Time.deltaTime);
        }
        // if trying to get off of the wall we were on before
        else if (isMovingAwayFromWall)
        {
            charController.Move(transform.TransformDirection(new Vector3(0f, 0f, -1f)) * movementSpeed * Time.deltaTime);
        }
        else
        {
            charController.Move(transform.TransformDirection(movementVector) * movementSpeed * Time.deltaTime);

            //apply gravity
            charContrYVelVector.y += gravity * Time.deltaTime;
            charController.Move(charContrYVelVector * Time.deltaTime);
        }

        // if player was previously climbing up and went down to hit the ground, stop climbing
        // don't know if here is the proper place, but it works
        bool groundedBefore = isCharControllerGrounded;

        // this NEEDS to go here for some reason, check if we're grounded
        isCharControllerGrounded = charController.isGrounded;

        if (isClimbing && !groundedBefore && isCharControllerGrounded)
        {
            isClimbing = false;
            isMovingAwayFromWall = true;
        }
    }

    // this will rotate the view by proxy
    void RotatePlayer()
    {
        // only allow rotation of player horizontally
        gameObject.transform.localRotation = Quaternion.Euler(transform.localRotation.y, turnVector.x, 0);
        // only camera will rotate vertically
        camFollowTarget.transform.localRotation = Quaternion.Euler(-turnVector.y, camFollowTarget.transform.localRotation.x, 0);
    }

    // this will rotate the view by proxy
    void RotateCameraOnly()
    {
        // dont know why using raw rotation works and local rotation doesn't
        // but oh well
        camFollowTarget.transform.rotation = Quaternion.Euler(-turnVector.y, turnVector.x, 0);
    }

    // just collapses the ground checks into one variable
    public bool UpdatePlayerGrounded()
    {
        if (isCharControllerOn && isCharControllerGrounded && !isRigidBodyOn)
        {
            return true;
        }
        else if (isRigidBodyOn && isRBGrounded && !isCharControllerOn)
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
        if (collision.GetContact(0).normal.y > 0.05)
        {
            isRBGrounded = true;
        }
    }

    // showing all raycasts on player
    private void OnDrawGizmos()
    {
        // grapple raycast visual
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(new Vector3(transform.position.x, transform.position.y - transform.localScale.y, transform.position.z), transform.forward * climbingReach);
        Gizmos.DrawRay(new Vector3(transform.position.x, transform.position.y + transform.localScale.y, transform.position.z), transform.forward * climbingReach);
        Gizmos.DrawRay(transform.position + transform.TransformDirection(new Vector3(0f, 0f, 1f)), transform.forward * (climbingReach - 1));

        // attack raycast visual
        Ray ray = mainCameraCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Gizmos.color = Color.red;
        Gizmos.DrawRay(ray.origin + camFollowTarget.transform.TransformDirection(new Vector3(0f, 0f, cameraDistance - 1)), ray.direction * attackReach);
    }
}
