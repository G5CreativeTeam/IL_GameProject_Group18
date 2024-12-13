using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondInstruction : MonoBehaviour
{
    public GameObject targetObject;
    public GameObject nextDialogue;
    public GameObject currentDialogue;

    // Update is called once per frame
    void Update()
    {
        PlotScript plot = targetObject.GetComponent<PlotScript>();

        if (plot.plantObject.GetComponent<PlantScript>().isFertilized)
        {
            currentDialogue.SetActive(false);
            nextDialogue.SetActive(true);
        }
    }
}
