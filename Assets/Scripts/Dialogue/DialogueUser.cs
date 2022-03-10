using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueUser: MonoBehaviour
{
    private GameObject textObject;
    private TMPro.TMP_Text textComp;
    private InstructionalUIBase instrUIBase;

    private bool collidedWithPlayer;
    public bool startedDialogue;
    public DialogueManager dialogueManager;
    public string[] sentences;
    public string name;
    public string messageInRange = "Press 'F' to Interact";

    private bool promptOn;


    // Start is called before the first frame update
    void Start()
    {
        textObject = GameObject.Find("InstructionalUI");
        instrUIBase = textObject.GetComponent<InstructionalUIBase>();
        textComp = textObject.GetComponent<TMPro.TMP_Text>();

        startedDialogue = false;

    }

    // Update is called once per frame
    void Update()
    {
        if (collidedWithPlayer && Input.GetKeyDown(KeyCode.F))
        {
            dialogueManager.Start();
            startedDialogue = true;
        }

        if (dialogueManager.gameObject.active && startedDialogue)
        {
            dialogueManager.StartDialogue(this);
            startedDialogue = false;
        }

        if (collidedWithPlayer && !Input.GetKeyDown(KeyCode.F) && !startedDialogue && !dialogueManager.gameObject.active)
        {
            instrUIBase.somethingDisplayed = true;
            textComp.enabled = true;
            promptOn = true;

            textComp.text = messageInRange;
        }
        else if (!collidedWithPlayer && promptOn)
        {
            instrUIBase.somethingDisplayed = false;
            textComp.enabled = false;
            promptOn = false;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            collidedWithPlayer = true;
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            collidedWithPlayer = false;
        }
    }
}
