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
        CheckMoving();
        CheckInAir();
        CheckClimbing();
    }

    void CheckClimbing()
    {
        if (playerController.isClimbing)
        {
            if (!checkedClimbing)
            {
                playerAnimator.SetBool("isClimbing", true);
                checkedClimbing = true;
            }
        }
        else
        {
            playerAnimator.SetBool("isClimbing", false);
            checkedClimbing = false;
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
        movementVector = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
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
