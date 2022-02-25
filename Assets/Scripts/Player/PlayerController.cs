using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    // tracking variables
    private Vector3 movementInput;
    private Vector3 rotateInputX;
    private Rigidbody playerBody;
    private PowerUp playerPowers;

    public Vector3 startPos;
    public bool isGrounded;
    private GameObject curGround;
    public bool isGrappling;
    public bool isClimbing;
    public Slider healthSlider;

    // modifiable player attributes

    // attack variables
    public GameObject attackRefPoint;    // where raycast for attack will come from
    public float attackRange = 5f;
    public float attackMeleeDamage = 10f;

    // player health
    public float maxHealth = 100f;
    public float curHealth;

    // movement variables
    public float movementSpeed = 15f;
    public float climbingSpeed = 7.5f;
    public float lookSensitivityX = 5f;
    public float lookSensitivityY = 5f;
    public float jumpForce = 10f;
    public float gravityMultiplier = 1f;
    public float grappleControlForce = 5f;

    // misc. variables
    public float respawnY = -50;


    private void Start()
    {
        Time.timeScale = 1;

        playerBody = gameObject.GetComponent<Rigidbody>();
        playerPowers = gameObject.GetComponent<PowerUp>();

        startPos = transform.position;
        curHealth = maxHealth;

        if (gravityMultiplier < 1)
        {
            gravityMultiplier = 1;
        }
    }

    private void Update()
    {
        // ui health
        healthSlider.value = curHealth;

        // get inputs - more responsive in update
        rotateInputX = new Vector3(0f, Input.GetAxisRaw("Mouse X"), 0f);

        if (isClimbing)
        {
            movementInput = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0f).normalized;
        }
        else
        {
            movementInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical")).normalized;
        }

        // jump handling
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded)
            {
                if (CheckOnClimbable() && playerPowers.canClimb)
                {
                    isClimbing = true;
                }
                else
                {
                    isClimbing = false;
                    playerBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                }
            }
            else
            {
                if (CheckOnClimbable() && playerPowers.canClimb)
                {
                    isClimbing = true;
                }
                else
                {
                    isClimbing = false;
                }
            }
        }
        else
        {
            isClimbing = false;
        }

        // climbing check for hold
        if (Input.GetKey(KeyCode.Space))
        {
            if (CheckOnClimbable() && playerPowers.canClimb)
            {
                isClimbing = true;
            }
            else
            {
                isClimbing = false;
            }
        }

        // attack handling
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            int i = 1;
        }

        // attack distance rays
        Debug.DrawRay(attackRefPoint.transform.position, attackRefPoint.transform.forward * attackRange, Color.red);

        // climb rays
        Debug.DrawRay(new Vector3(transform.position.x, transform.position.y - transform.localScale.y, transform.position.z), transform.forward * 1.5f, Color.blue);
        Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + transform.localScale.y, transform.position.z), transform.forward * 1.5f, Color.blue);
        Debug.DrawRay(new Vector3(transform.position.x, transform.position.y, transform.position.z), transform.forward * 1.5f, Color.blue);
    }

    private bool CheckOnClimbable()
    {
        RaycastHit hit;
        if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y - transform.localScale.y, transform.position.z), transform.forward, out hit, 1.5f) ||
            Physics.Raycast(new Vector3(transform.position.x, transform.position.y + transform.localScale.y, transform.position.z), transform.forward, out hit, 1.5f) ||
            Physics.Raycast(new Vector3(transform.position.x, transform.position.y, transform.position.z), transform.forward, out hit, 1.5f))
        {
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Climbable"))
            {
                return true;
            }
        }
        return false;
    }

    private void FixedUpdate()
    {
        if (!isClimbing)
        {
            playerBody.AddForce(Vector3.down * -Physics.gravity.y * (gravityMultiplier - 1));
        }

        // move player
        if (isGrappling && !isGrounded)
        {
            playerBody.AddForce(movementInput * grappleControlForce, ForceMode.Acceleration);
        }
        // this may be causing some stuttering, come back to this eventually
        else if (isClimbing)
        {
            playerBody.velocity = transform.TransformDirection(new Vector3(movementInput.x * climbingSpeed, movementInput.y * climbingSpeed, 0f));
        }
        else
        {
            playerBody.velocity = transform.TransformDirection(new Vector3(movementInput.x * movementSpeed, playerBody.velocity.y, movementInput.z * movementSpeed));
        }

        // rotate player -- may need to check for a better way to do this
        // camera is rotating on fixed update, this could be causing some lag
        Quaternion deltaRotation = Quaternion.Euler(rotateInputX * lookSensitivityX);
        playerBody.MoveRotation(playerBody.rotation * deltaRotation);

    }

    private void LateUpdate()
    {
        if (transform.position.y <= respawnY || curHealth <= 0)
        {
            transform.position = startPos;
            curHealth = maxHealth;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(attackRefPoint.transform.position, attackRefPoint.transform.forward * attackRange);
        Gizmos.color = Color.blue;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // this code may be messy on the > 0 check
        // may need to do more research/testing to see if this threshold is appropriate
        // same with OnCollisionExit
        if (collision.GetContact(0).normal.y > 0)
        {
            isGrounded = true;
            curGround = collision.gameObject;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject == curGround)
        {
            isGrounded = false;
        }
    }
}
