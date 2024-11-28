using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandardDialogueTrigger : MonoBehaviour
{
    public GameObject DManagerInk;

    // Start is called before the first frame update
    void Start()
    {
        DManagerInk.GetComponent<DialogueManager>().InitiateDialogue();
        DManagerInk.GetComponent<DialogueManager>().AdvanceDialogue();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
