using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstInstruction : MonoBehaviour
{
    public GameObject targetObject;
    public GameObject nextDialogue;
    public GameObject currentDialogue;

    public void Update()
    {
        PlotScript plot = targetObject.GetComponent<PlotScript>();

        if (plot.hasPlant)
        {
            Debug.Log("Planted");
            currentDialogue.SetActive(false);
            nextDialogue.SetActive(true);
        }
    }
}
