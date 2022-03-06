using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationStateController : MonoBehaviour
{
    [Header("GameObject References")]
    public GameObject player;


    // script references
    private NewController playerController;
    private Animator playerAnimator;

    // tracking variables
    private Vector3 movementVector;
    private bool checkedAir;
    private bool checkedClimbing;

    // Start is called before the first frame update
    void Start()
    {
        playerAnimator = gameObject.GetComponent<Animator>();
        playerController = player.gameObject.GetComponent<NewController>();
    }

    // Update is called once per frame
    void Update()
    {
        movementVector = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));

        CheckMoving();
        CheckInAir();
        CheckClimbing();
    }

    // this is a little glitchy right now
    void CheckClimbing()
    {
        if (playerController.isClimbing)
        {
            if (!checkedClimbing)
            {
                playerAnimator.SetBool("isClimbing", true);
                checkedClimbing = true;
            }

            if (movementVector.magnitude > 0)
            {
                playerAnimator.speed = 1;
            }
            else
            {
                playerAnimator.speed = 0;
            }
        }
        else
        {
            playerAnimator.SetBool("isClimbing", false);
            checkedClimbing = false;
            playerAnimator.speed = 1;
        }
    }

    void CheckInAir()
    {
        if (!playerController.isPlayerGrounded)
        {
            if (!checkedAir)
            {
                playerAnimator.SetBool("inAir", true);
                checkedAir = true;
            }
        }
        else
        {
            playerAnimator.SetBool("inAir", false);
            checkedAir = false;
        }
    }

    void CheckMoving()
    {
        if (movementVector.magnitude > 0)
        {
            playerAnimator.SetBool("isMoving", true);
        }
        else
        {
            playerAnimator.SetBool("isMoving", false);
        }
    }
}
