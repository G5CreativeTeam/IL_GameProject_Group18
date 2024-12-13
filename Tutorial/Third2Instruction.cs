using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Third2Instruction : MonoBehaviour
{
    public GameObject targetObject;
    public GameObject nextDialogue;
    public GameObject currentDialogue;

    // Update is called once per frame
    void Update()
    {
        PlotScript plot = targetObject.GetComponent<PlotScript>();

        if (plot.plantObject.GetComponent<PlantScript>().isReadyToHarvest)
        {
            currentDialogue.SetActive(false);
            nextDialogue.SetActive(true);
        }
    }
}
