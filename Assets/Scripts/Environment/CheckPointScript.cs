using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointScript : MonoBehaviour
{
    private bool checkPointActive = true;
    private AudioSource audioSrc;
    public AudioClip checkpointSound;
    public float checkpointSoundVol = 1f;

    private void Start()
    {
        audioSrc = gameObject.GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && checkPointActive)
        {
            audioSrc.PlayOneShot(checkpointSound, checkpointSoundVol);

            Debug.Log("Player Reached Checkpoint");
            other.gameObject.GetComponent<NewController>().SetNewStartPos(other.gameObject.transform.position);
            checkPointActive = false;
        }
    }
}
