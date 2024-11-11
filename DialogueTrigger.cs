using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;

    private DialogueManager dialogueManager ;

    private void Start()
    {
        dialogueManager = FindObjectOfType<DialogueManager>();
    }
    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (dialogueManager.dialogueText.text != dialogueManager.sentences.Peek())
            {
                StopAllCoroutines();
                dialogueManager.dialogueText.text = dialogueManager.sentences.Peek();
            }
        }
    }
    public void TriggerDialogue()
    {
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
    }

}
