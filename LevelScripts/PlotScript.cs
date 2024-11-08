using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlotScript : MonoBehaviour, IDropHandler
{
    public bool hasPlant = false;
    private PlantScript plant;

    private GameObject plantObject;
    public void OnDrop(PointerEventData eventData)
    {
        hasPlant = transform.childCount > 0;
        
        if (eventData != null)
        {
            itemScript draggableItem = eventData.pointerDrag.GetComponent<itemScript>();
            if (!hasPlant)
            {
                SeedDrop(draggableItem);
   
            } else  {
                if (draggableItem.shovel)
                {
                    ShovelDrop();

                } else if (draggableItem.wateringCan)
                {
                    WateringCanDrop();
                } else if (draggableItem.fertilizer)
                {
                    FertilizerDrop();
                }
            }
        }
    }

    public void SeedDrop(itemScript draggableItem)
    {
        if (draggableItem.plant != null)
        {
            plantObject = Instantiate(draggableItem.plant, transform);
            plant = plantObject.GetComponent<PlantScript>();
            plant.originalPlant = false;
            plant.eventSystem.GetComponent<StatsScript>().seedPlanted++;
        }
    }
    public void WateringCanDrop()
    {
        if (!transform.GetChild(0).gameObject.GetComponent<PlantScript>().isWatered)
        {
            transform.GetChild(0).gameObject.GetComponent<PlantScript>().isWatered = true;
            transform.GetChild(0).gameObject.GetComponent<PlantScript>().waterTimer = transform.GetChild(0).gameObject.GetComponent<PlantScript>().timeUntilWater;
            transform.GetChild(0).gameObject.GetComponent<PlantScript>().currentlyWP = false;
            //transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
            Destroy(transform.GetChild(0).Find("WaterIndicator(Clone)").gameObject);
            Debug.Log("Successfully Watered");
        }
    }

    public void ShovelDrop()
    {
        Debug.Log("Plant Destroyed");

        Destroy(transform.GetChild(0).gameObject);
    }

    public void FertilizerDrop()
    {
        if (!transform.GetChild(0).gameObject.GetComponent<PlantScript>().isFertilized)
        {
            transform.GetChild(0).gameObject.GetComponent<PlantScript>().isFertilized = true;
            //transform.GetChild(0).gameObject.GetComponent<PlantScript>().fertilizeTimer = transform.GetChild(0).gameObject.GetComponent<PlantScript>().timeUntilFertilized;
            transform.GetChild(0).gameObject.GetComponent<PlantScript>().currentlyFP = false;
            //transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
            Destroy(transform.GetChild(0).Find("FertilizeIndicator(Clone)").gameObject);
            Debug.Log("Successfully Fertilized");
        }
    }

}
