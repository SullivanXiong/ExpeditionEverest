using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueUser: MonoBehaviour
{
    private bool collidedWithPlayer;
    public bool startedDialogue;
    public DialogueManager dialogueManager;
    public string[] sentences;
    public string name;

    // Start is called before the first frame update
    void Start()
    {
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

        if (dialogueManager.gameObject.active & startedDialogue)
        {
            dialogueManager.StartDialogue(this);
            startedDialogue = false;
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
