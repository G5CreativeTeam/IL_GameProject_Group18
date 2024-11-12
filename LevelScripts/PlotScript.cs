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
            
            if (eventData.pointerDrag.GetComponent<SeedScript>() != null && !hasPlant)
            {
                SeedDrop(eventData.pointerDrag.GetComponent<SeedScript>());
   
            } else if (hasPlant) {
                if (eventData.pointerDrag.GetComponent<ShovelScript>())
                {
                    ShovelDrop();

                } else if (eventData.pointerDrag.GetComponent<WateringCanScript>())
                {
                    WateringCanDrop();
                } else if (eventData.pointerDrag.GetComponent<FertilizerScript>())
                {
                    FertilizerDrop();
                }
            }
        }
    }

    public void SeedDrop(SeedScript draggableItem)
    {
        if (draggableItem.plant != null)
        {
            plantObject = Instantiate(draggableItem.plant, transform);
            plant = plantObject.GetComponent<PlantScript>();
            plant.originalPlant = false;
            plant.eventSystem.GetComponent<StatsScript>().seedPlanted++;
            plant.eventSystem.GetComponent<EventSystem>().numOfPlants++;
        }
    }
    public void WateringCanDrop()
    {
        if (!transform.GetChild(0).gameObject.GetComponent<PlantScript>().isWatered)
        {
            transform.GetChild(0).gameObject.GetComponent<PlantScript>().isWatered = true;
            transform.GetChild(0).gameObject.GetComponent<PlantScript>().waterTimer = transform.GetChild(0).gameObject.GetComponent<PlantScript>().timeUntilWater;
            transform.GetChild(0).gameObject.GetComponent<PlantScript>().currentlyWP = false;
            
            Destroy(transform.GetChild(0).Find("WaterIndicator(Clone)").gameObject);
            Debug.Log("Successfully Watered");
        }
    }

    public void ShovelDrop()
    {
        Debug.Log("Plant Destroyed");
        plant.eventSystem.GetComponent<EventSystem>().numOfPlants--;
        Destroy(transform.GetChild(0).gameObject);
    }

    public void FertilizerDrop()
    {
        if (!transform.GetChild(0).gameObject.GetComponent<PlantScript>().isFertilized)
        {
            transform.GetChild(0).gameObject.GetComponent<PlantScript>().isFertilized = true;
            transform.GetChild(0).gameObject.GetComponent<PlantScript>().currentlyFP = false;
            
            Destroy(transform.GetChild(0).Find("FertilizeIndicator(Clone)").gameObject);
            Debug.Log("Successfully Fertilized");
        }
    }

}
