using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerUp : MonoBehaviour
{
    private NewController playerController;

    private AudioSource audioSrc;
    public AudioClip equipSound;
    public float equipSoundVol = 1f;
    public AudioClip healthSound;
    public float healthSoundVol = 1f;

    // these are legacy booleans
    public bool canClimb;
    public bool canGrapple;
    public float healthPowerUpAmt = 30f;

    public GameObject grappleImage;
    public GameObject grappleSlider;
    public GameObject pickImage;

    // Start is called before the first frame update
    void Start()
    {
        playerController = gameObject.GetComponent<NewController>();
        audioSrc = gameObject.GetComponent<AudioSource>();

        grappleImage.SetActive(false);
        pickImage.SetActive(false);
        grappleSlider.SetActive(false);

        if (canClimb)
        {
            pickImage.SetActive(true);
            playerController.canClimb = true;
        }
        if (canGrapple)
        {
            grappleImage.SetActive(true);
            grappleSlider.SetActive(true);
            playerController.canGrapple = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "PowerUpGrappleHook")
        {
            audioSrc.PlayOneShot(equipSound, equipSoundVol);
            canClimb = true; // not necessary for new player controller
            playerController.canGrapple = true;
            grappleImage.SetActive(true);
            grappleSlider.SetActive(true);
            Destroy(other.gameObject);
        }
        else if (other.tag == "PowerUpClimbingPick")
        {
            audioSrc.PlayOneShot(equipSound, equipSoundVol);
            canGrapple = true; // not necessary for new player controller
            playerController.canClimb = true;
            pickImage.SetActive(true);
            Destroy(other.gameObject);
        }
        else if (other.tag == "PowerUpHealth")
        {
            if (!(playerController.curHealth >= playerController.maxHealth)) // dont pick up if health is 100%
            {
                audioSrc.PlayOneShot(healthSound, healthSoundVol);
                playerController.AddHealth(healthPowerUpAmt);
                Destroy(other.gameObject);
            }
        }
    }
}
