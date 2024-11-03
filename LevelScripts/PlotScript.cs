using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlotScript : MonoBehaviour, IDropHandler
{
    public bool hasPlant = false;
    public PlantScript plant;

    private GameObject plantObject;
    public void OnDrop(PointerEventData eventData)
    {
        hasPlant = transform.childCount > 0;
        
        if (eventData != null)
        {
            
            itemScript draggableItem = eventData.pointerDrag.GetComponent<itemScript>();

            if (!hasPlant)
            {
                if (draggableItem.plant != null)
                {
                    plantObject = Instantiate(draggableItem.plant, transform);
                    plant = plantObject.GetComponent<PlantScript>();
                    plant.originalPlant = false;
                } 
   
            } else  {
                if (draggableItem.shovel)
                {
                    Debug.Log("Plant Destroyed");
                    
                    Destroy(transform.GetChild(0).gameObject);

                } else if (draggableItem.wateringCan)
                {
                    if (!transform.GetChild(0).gameObject.GetComponent<PlantScript>().isWatered)
                    {
                        transform.GetChild(0).gameObject.GetComponent<PlantScript>().isWatered = true;
                        transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
                        Debug.Log("Successfully Watered");
                    }
                }
            }
           
        }
        
        
    }

}
