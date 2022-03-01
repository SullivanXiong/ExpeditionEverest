using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    private Queue<string> sentences = new Queue<string>();
    public Text nameText;
    public Text dialogueText;
    public float textSpeed = 1;
    private DialogueUser latestDU;

    // Start is called before the first frame update
    public void Start()
    {
        gameObject.SetActive(true);
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void StartDialogue(DialogueUser dialogueUser)
    {
        sentences.Clear();

        nameText.text = dialogueUser.name;
        latestDU = dialogueUser;

        foreach (string sentence in dialogueUser.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        Debug.Log(sentences.Count);
        if (sentences.Count == 0)
        {
            Debug.Log(sentences.Count);
            EndDialogue();
            return;
        }
        
        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }

    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSecondsRealtime(0.1f * textSpeed);
        }
    }

    public void EndDialogue()
    {
        latestDU.startedDialogue = false;
        gameObject.SetActive(false);
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
