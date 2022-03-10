using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InstructionalUIManager : MonoBehaviour
{

    private GameObject textObject;
    private TMPro.TMP_Text textComp;
    private InstructionalUIBase instrUIBase;

    [Header("Instructional UI Variables")]
    public string messageToDisplay = "Set an instructional message to load when touching this Object";
    public float displayTime = 5f;
    public bool repeatShow = false;

    // tracking variables
    [Header("Tracking Variables - DO NOT modify")]
    public bool currentlyDisplayed = false;
    public bool needToDisplay = false;
    public bool alreadyShown = false;
    public float displayTimeTrack;

    // Start is called before the first frame update
    void Start()
    {
        textObject = GameObject.Find("InstructionalUI");
        instrUIBase = textObject.GetComponent<InstructionalUIBase>();
        textComp = textObject.GetComponent<TMPro.TMP_Text>();
        textComp.enabled = false;

        displayTimeTrack = displayTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (needToDisplay && !currentlyDisplayed && !instrUIBase.somethingDisplayed)
        {
            instrUIBase.somethingDisplayed = true;

            textComp.text = messageToDisplay; // display the message
            textComp.enabled = true;

            currentlyDisplayed = true; // set the tracking variables
            needToDisplay = false;
            alreadyShown = true;
        }
        else if (currentlyDisplayed)
        {
            displayTimeTrack -= Time.deltaTime;
        }

        if (currentlyDisplayed && displayTimeTrack <= 0)
        {
            instrUIBase.somethingDisplayed = false;

            textComp.enabled = false;

            displayTimeTrack = displayTime;
            currentlyDisplayed = false;
            needToDisplay = false;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Player" && !currentlyDisplayed) {
            if (repeatShow)
            {
                needToDisplay = true;
            }
            else
            {
                if (!alreadyShown)
                {
                    needToDisplay = true;
                }
            }
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Player" && !currentlyDisplayed) {
            if (repeatShow)
            {
                needToDisplay = true;
            }
            else
            {
                if (!alreadyShown)
                {
                    needToDisplay = true;
                }
            }
        }
    }
}
