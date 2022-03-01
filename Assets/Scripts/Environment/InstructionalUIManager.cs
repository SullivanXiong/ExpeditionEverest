using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InstructionalUIManager : MonoBehaviour
{
    public string messageToDisplay = "Set an instructional message to load when touching this Object";
    public float displayTime;
    private GameObject textObject;
    private TMPro.TMP_Text textComp;
    private bool displayed = false;

    // Start is called before the first frame update
    void Start()
    {
        displayTime = 5f;
        textObject = GameObject.Find("InstructionalUI");
        textComp = textObject.GetComponent<TMPro.TMP_Text>();
        textComp.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (displayed && displayTime > 0) {
            displayTime -= Time.deltaTime * 1f;
        }
        else if (displayTime <= 0) {
            textComp.enabled = false;
            displayed = false;
            displayTime = 5f;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.collider.tag);
        if (collision.collider.tag == "Player" && !displayed) {
            textComp.text = messageToDisplay;
            textComp.enabled = true;
            displayed = true;
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Player" && !displayed) {
            textComp.text = messageToDisplay;
            textComp.enabled = true;
            displayed = true;
        }
    }
}
