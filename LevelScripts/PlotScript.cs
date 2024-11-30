using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlotScript : MonoBehaviour, IDropHandler, IDataPersistence
{
    [SerializeField] private string id;

    [ContextMenu("Generate guid for id")]
    private void GenerateGuid()
    {
        id = System.Guid.NewGuid().ToString();
    }
    [Header("Sprites")]
    public Sprite DryLand;
    public Sprite WetLand;


    [Header("Audio")]
    public GameObject shovelAudio;
    public GameObject plantAudio;
    public GameObject waterAudio;
    public GameObject fertilizerAudio;

    [HideInInspector] public bool hasPlant;
    [HideInInspector] public GameObject plantObject;
    [HideInInspector] public bool toolActive;

    private PlantScript plant;


    //private class plantData
    //{
    //    [HideInInspector] public bool originalPlant = true;
    //    [HideInInspector] public bool isWatered;
    //    [HideInInspector] public bool isFertilized;
    //    [HideInInspector] public bool currentlyWP;
    //    [HideInInspector] public bool currentlyFP;
    //    [HideInInspector] public float growthTimer, waterTimer;

    //    [Header("Growth Setting")]
    //    public float firstGrowthTime;
    //    public float secondGrowthTime;

    //    public float timeUntilWater;
    //    public float timeUntilFertilized;

    //    [Header("Attributes")]
    //    public int health;
    //    public int sellPrice;
    //    public int scoreValue ;
    //    public Sprite[] plantSprite;


    //    [Header("Indicators")]
    //    public GameObject waterIndicator;
    //    public GameObject fertilizeIndicator;

    //    [Header("Audio")]
    //    public GameObject growAudio;
    //    public GameObject sellAudio;

    //    private int currentGrowthPhase = 0;

    //}
    public void Start()
    {
        toolActive = false;
    }

    public void Update()
    {
        if (hasPlant && plantObject != null) 
        { 
            if (plantObject.GetComponent<PlantScript>().isWatered && gameObject.GetComponent<SpriteRenderer>().sprite != WetLand)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = WetLand;
            } else if ( !plantObject.GetComponent<PlantScript>().isWatered)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = DryLand;
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
            plantAudio.GetComponent<AudioSource>().Play();
            plant.levelProperties.GetComponent<StatsScript>().seedPlanted++;
            plant.levelProperties.GetComponent<StatsScript>().numOfPlants++;
            
            draggableItem.cooldownTimer = 0;
            if (!LevelProperties.Instance.unlimitedMoney)
            {
                LevelProperties.Instance.GetComponent<StatsScript>().DeductAmount(draggableItem.price);
            }
            hasPlant = true;
            
        }
    }
    public void WateringCanDrop()
    {
        if (!transform.GetChild(0).gameObject.GetComponent<PlantScript>().isWatered)
        {
            transform.GetChild(0).gameObject.GetComponent<PlantScript>().isWatered = true;

            transform.GetChild(0).gameObject.GetComponent<PlantScript>().currentlyWP = false;
            waterAudio.GetComponent<AudioSource>().Play();
            Destroy(transform.GetChild(0).Find("WaterIndicator(Clone)").gameObject);
            
        }
    }

    public void ShovelDrop()
    {
        
        
        shovelAudio.GetComponent<AudioSource>().Play();
        plant.levelProperties.GetComponent<StatsScript>().numOfPlants--;
        Destroy(transform.GetChild(0).gameObject);
        hasPlant = false;
        gameObject.GetComponent<SpriteRenderer>().sprite = DryLand;

    }

    public void FertilizerDrop()
    {
        if (!transform.GetChild(0).gameObject.GetComponent<PlantScript>().isFertilized)
        {
            
            fertilizerAudio.GetComponent<AudioSource>().Play();
            Destroy(transform.GetChild(0).Find("FertilizeIndicator(Clone)").gameObject);

            transform.GetChild(0).gameObject.GetComponent<PlantScript>().isFertilized = true;
            transform.GetChild(0).gameObject.GetComponent<PlantScript>().currentlyFP = false;
            //Instantiate(fertilizer,transform);

        }
    }

    public void LoadData(GameData gameData)
    {
        
        plotData matchingPlot = gameData.plotList.Find(plot => plot.id == id);
        //Debug.Log("Is it null or not?"+matchingPlot == null);
        //Debug.Log("is the list empty?" + gameData.plotList.Count);
        //Debug.Log(matchingPlot);
        if (matchingPlot != null)
        {
            hasPlant = matchingPlot.hasPlant;
            plantObject = matchingPlot.plantObject;
            if (plantObject != null)
            {
                Instantiate(plantObject, transform);
            }
            plant = matchingPlot.plant;
            
        } else
        {
            hasPlant = false;
            plant = null;
            plantObject = null;
        }


    }

    public void SaveData(ref GameData gameData)
    {
        plotData plotSave = new plotData();
        
        plotSave.id = id;
        plotSave.hasPlant = hasPlant;
        plotSave.plant = plant;
        plotSave.plantObject = plantObject;

        plotData matchingPlot = gameData.plotList.Find(plot => plot.id == id);

        if (matchingPlot != null)
        {
            matchingPlot.hasPlant = plotSave.hasPlant;
            matchingPlot.plantObject = plotSave.plantObject;
            matchingPlot.plant = plotSave.plant;
            
        }
        else
        {
            List<plotData> list = new List<plotData>();
            list.Add(plotSave);
            
            gameData.plotList.Add(plotSave);
            foreach (plotData plotData in gameData.plotList)
            {
                Debug.Log(plotData.hasPlant);
            }
            
            
        }

        
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag.GetComponent<SeedScript>() != null && LevelProperties.Instance.GetComponent<StatsScript>().moneyAvailable >= eventData.pointerDrag.GetComponent<SeedScript>().price) { 
            SeedDrop(eventData.pointerDrag.GetComponent<SeedScript>());
        }
    }
}
