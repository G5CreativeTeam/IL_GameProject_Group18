using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlotScript : MonoBehaviour, IDropHandler
{
    public bool hasPlant = false;
    private GameObject plantObject;
    public PlantScript plant; 
    public void OnDrop(PointerEventData eventData)
    {

        Debug.Log(eventData == null);
        if (!hasPlant && eventData != null)
        {
            Debug.Log("Seed Successfully Dropped");
            SeedScript draggableItem = eventData.pointerDrag.GetComponent<SeedScript>();

            plantObject = Instantiate(draggableItem.plant, transform);
            plant = plantObject.GetComponent<PlantScript>();
            plant.originalPlant = false;
            //draggableItem.parentAfterDrag = transform;
            hasPlant = true;
        }
        
        
    }

}
