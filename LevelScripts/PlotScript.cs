using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    [Header("Save Properties")]
    public GameObject carrotPlant;
    public GameObject potatoPlant;
    public GameObject yamPlant;

    [HideInInspector] public bool hasPlant;
    [HideInInspector] public PlantType type = PlantType.none;
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
                Debug.Log("ISWATERED");
            } else if ( !plantObject.GetComponent<PlantScript>().isWatered)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = DryLand;
            }
        } else
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = DryLand;
        }
    }
    
    public void SeedDrop(SeedScript item)
    {
        if (item.plant != null)
        {
            plantObject = Instantiate(item.plant, transform);


            plant = plantObject.GetComponent<PlantScript>();
            plant.originalPlant = false;
            plantAudio.GetComponent<AudioSource>().Play();
            plant.levelProperties.GetComponent<StatsScript>().seedPlanted++;
            plant.levelProperties.GetComponent<StatsScript>().numOfPlants++;

            //if (!LevelProperties.Instance.unlimitedMoney)
            //{
            //    GameObject floatingnum = LevelProperties.Instance.SpawnFloatingNumber(transform.parent, -1*draggableItem.price);
            //    floatingnum.transform.localScale = new Vector3(1f, 1f, 1f);
            //}
            
            type = plant.GetComponent<PlantScript>().plant;
            item.originalSeed.GetComponent<SeedScript>().cooldownTimer = 0;
            if (!LevelProperties.Instance.unlimitedMoney)
            {
                LevelProperties.Instance.GetComponent<StatsScript>().DeductAmount(item.price);
            }
            hasPlant = true;
           
        } else
        {
            Debug.Log("Couldn't plant");
        }
    }
    public void WateringCanDrop()
    {
        if (!transform.GetChild(0).gameObject.GetComponent<PlantScript>().isWatered)
        {
            transform.GetChild(0).gameObject.GetComponent<PlantScript>().isWatered = true;

            transform.GetChild(0).gameObject.GetComponent<PlantScript>().currentlyWP = false;
            transform.GetChild(0).gameObject.GetComponent<PlantScript>().witherTimer = transform.GetChild(0).gameObject.GetComponent<PlantScript>().witherTime;
            waterAudio.GetComponent<AudioSource>().Play();
            Destroy(transform.GetChild(0).Find("WaterIndicator(Clone)").gameObject);
            
        }
    }

    public void ShovelDrop()
    {

        type = PlantType.none;
        shovelAudio.GetComponent<AudioSource>().Play();
        plant.levelProperties.GetComponent<StatsScript>().numOfPlants--;
        plant.levelProperties.GetComponent<StatsScript>().plantsLost++;
        for (int i = 0;i<transform.childCount;i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
        
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
        
        PlotData matchingPlot = gameData.plotList.Find(plot => plot.id == id);

        if (matchingPlot != null)
        {
            hasPlant = matchingPlot.hasPlant;
            type = matchingPlot.PlantType;
            if (hasPlant && type != PlantType.none)
            {
                switch (type)
                {
                    case PlantType.carrot:
                        plantObject = Instantiate(carrotPlant);
                        return;
                    case PlantType.potato:
                        plantObject = Instantiate(potatoPlant);
                        return;
                    case PlantType.yam:
                        plantObject = Instantiate(yamPlant);
                        return;

                }
            }
            plant = plantObject.GetComponent<PlantScript>();
            
        } else
        {
            hasPlant = false;
            plant = null;
            plantObject = null;
        }


    }

    public void SaveData(ref GameData gameData)
    {
        PlotData plotSave = new();
        
        plotSave.id = id;
        plotSave.hasPlant = hasPlant;
        plotSave.plant = plant;
        plotSave.plantObject = plantObject;
        plotSave.PlantType = type;

        PlotData matchingPlot = gameData.plotList.Find(plot => plot.id == id);

        if (matchingPlot != null)
        {
            matchingPlot.hasPlant = plotSave.hasPlant;
            matchingPlot.plantObject = plotSave.plantObject;
            matchingPlot.plant = plotSave.plant;
            
        }
        else
        {
            //List<plotData> list = new();
            //list.Add(plotSave);
            
            gameData.plotList.Add(plotSave);
            foreach (PlotData plotData in gameData.plotList)
            {
                Debug.Log(plotData.hasPlant);
            }
            
            
        }

        
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag.GetComponent<SeedScript>() != null 
            && LevelProperties.Instance.GetComponent<StatsScript>().moneyAvailable >= eventData.pointerDrag.GetComponent<SeedScript>().price) { 
            SeedDrop(eventData.pointerDrag.GetComponent<SeedScript>());
        }
    }
}
