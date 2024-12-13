using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngineInternal;

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
    public GameObject plantObject;
    [HideInInspector] public bool toolActive;

    //private PlantScript plant;

    public void Start()
    {
        toolActive = false;
    }

    public void Update()
    {
        if (hasPlant && plantObject != null) 
        { 
            if (plantObject.GetComponent<PlantScript>().isWatered && gameObject.GetComponent<SpriteRenderer>().sprite != WetLand && plantObject.GetComponent<PlantScript>().isAlive)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = WetLand;
                
            } else if ( !plantObject.GetComponent<PlantScript>().isWatered || !plantObject.GetComponent<PlantScript>().isAlive)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = DryLand;
            }
        } else
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = DryLand;
            toolActive = false;
        }
    }
    
    public void SeedDrop(SeedScript item)
    {
        if (item.plant != null)
        {
            plantObject = Instantiate(item.plant, transform);

            PlantScript plant = plantObject.GetComponent<PlantScript>();
            
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

        }
    }
    public void WateringCanDrop()
    {
        if (!plantObject.GetComponent<PlantScript>().isWatered)
        {
            plantObject.GetComponent<PlantScript>().isWatered = true;

            plantObject.GetComponent<PlantScript>().currentlyWP = false;
            plantObject.GetComponent<PlantScript>().witherTimer = plantObject.transform.gameObject.GetComponent<PlantScript>().witherTime;
            waterAudio.GetComponent<AudioSource>().Play();
            Destroy(plantObject.transform.Find("WaterIndicator(Clone)").gameObject);
            
        }
    }

    public void ShovelDrop()
    {
        PlantScript plant = plantObject.GetComponent<PlantScript>();
        type = PlantType.none;
        shovelAudio.GetComponent<AudioSource>().Play();
        if (plant.isAlive)
        {
            plant.levelProperties.GetComponent<StatsScript>().numOfPlants--;
            plant.levelProperties.GetComponent<StatsScript>().plantsLost++;
        }
        
        for (int i = 0;i<transform.childCount;i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
        
        hasPlant = false;
        gameObject.GetComponent<SpriteRenderer>().sprite = DryLand;
        plantObject = null;
    }

    public void FertilizerDrop()
    {
        if (!plantObject.GetComponent<PlantScript>().isFertilized)
        {
            
            fertilizerAudio.GetComponent<AudioSource>().Play();
            Destroy(plantObject.transform.Find("FertilizeIndicator(Clone)").gameObject);

            plantObject.transform.gameObject.GetComponent<PlantScript>().isFertilized = true;
            plantObject.GetComponent<PlantScript>().currentlyFP = false;
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
                int enumType = (int)type;

                switch (enumType)
                {
                    case 0:
                        plantObject = Instantiate(carrotPlant,gameObject.transform);
                        break;
                    case 1:
                        plantObject = Instantiate(potatoPlant, gameObject.transform);
                        break;
                    case 2:
                        plantObject = Instantiate(yamPlant, gameObject.transform);
                        break;
                }
                
                PlantScript plant = plantObject.GetComponent<PlantScript>();
                plant.originalPlant = false;
                plant.isWatered = matchingPlot.plantData.isWatered;

                if (!plant.isWatered)
                {
                    plant.currentlyWP = false;
                }
                plant.isFertilized = matchingPlot.plantData.isFertilized;

                if (!plant.isFertilized)
                {

                    plant.currentlyFP = false;  
                }
                plant.health = matchingPlot.plantData.health;

                plant.currentGrowthPhase = matchingPlot.plantData.currentGrowthPhase;
                plant.isAlive = matchingPlot.plantData.isAlive;
                plant.currentlyBroken = matchingPlot.plantData.currentlyBroken;
                plant.currentlyWithered = matchingPlot.plantData.currentlyWithered;
                Debug.Log(plant.currentlyWithered);
                if (!plant.isAlive)
                {
                    if (plant.currentlyWithered)
                    {
                        if (plant.currentGrowthPhase == -3)
                        {
                            Debug.Log("WIthered1");
                            plant.gameObject.GetComponent<SpriteRenderer>().sprite = plant.firstWitherSprite;


                        }
                        else if (plant.currentGrowthPhase == -4)
                        {

                            plant.gameObject.GetComponent<SpriteRenderer>().sprite = plant.secondWitherSprite;

                        }

                        if ((int)plant.plant == (int)PlantType.carrot)
                        {
                            Debug.Log("witheredplanted?");
                            plant.gameObject.transform.localPosition = Vector3.zero;
                        }
                    }
                    else if (plant.currentlyBroken)
                    {
                        if (plant.currentGrowthPhase == -1)
                        {
                            plant.gameObject.GetComponent<SpriteRenderer>().sprite = plant.firstBrokenSprite;


                        }
                        else if (plant.currentGrowthPhase == -2)
                        {

                            plant.gameObject.GetComponent<SpriteRenderer>().sprite = plant.secondBrokenSprite;

                        }
                    }
                    plant.gameObject.GetComponent<GridLayoutGroup>().padding.top = 0;
                    plant.gameObject.transform.parent.GetComponent<PlotScript>().toolActive = false;
                    Instantiate(plant.deathIndicator, plant.gameObject.transform);
                    plant.gameObject.transform.localScale = Vector3.one;
                }

                plant.growthTimer = matchingPlot.plantData.growthTimer;
                plant.witherTimer = matchingPlot.plantData.witherTimer;
                Debug.Log("currentgrowthphase is " + matchingPlot.plantData.currentGrowthPhase);

                plant.stopGrowing = matchingPlot.plantData.stopGrowing;

                
                plant.isReadyToHarvest = matchingPlot.plantData.isReadyToHarvest;
                if (plant.isReadyToHarvest)
                {
                    plant.currentlyHP = false;
                }


            }
            //plant = plantObject.GetComponent<PlantScript>();
            
        } else
        {
            hasPlant = false;
            //plant = null;
            plantObject = null;
        }


    }

    public void SaveData(ref GameData gameData)
    {
        PlotData plotSave = new();
        
        plotSave.id = id;
        plotSave.hasPlant = hasPlant;
        plotSave.PlantType = type;
        PlantScript plant;
        plotSave.plantData = new();
        if (plotSave.hasPlant && plantObject != null )
        {
            plant = plantObject.GetComponent<PlantScript>();
            
            plotSave.plantData.health = plant.health;
            plotSave.plantData.currentlyWP = plant.currentlyWP;
            plotSave.plantData.currentlyFP = plant.currentlyFP;
            Debug.Log(plant.currentlyWithered);
            plotSave.plantData.currentlyWithered = plant.currentlyWithered;
            plotSave.plantData.currentlyBroken = plant.currentlyBroken;
            plotSave.plantData.growthTimer = plant.growthTimer;
            plotSave.plantData.witherTimer = plant.witherTimer;
            plotSave.plantData.currentGrowthPhase = plant.currentGrowthPhase;
            plotSave.plantData.stopGrowing = plant.stopGrowing;


            plotSave.plantData.isWatered = plant.isWatered;


            plotSave.plantData.isFertilized = plant.isFertilized;
            plotSave.plantData.isAlive = plant.isAlive;
            plotSave.plantData.isReadyToHarvest = plant.isReadyToHarvest;
        }
        

        PlotData matchingPlot = gameData.plotList.Find(plot => plot.id == id);
        //matchingPlot.plantData = new();

        if (matchingPlot != null)
        {
            matchingPlot.hasPlant = hasPlant;
            matchingPlot.PlantType = type;

            matchingPlot.plantData.isWatered = plotSave.plantData.isWatered;
            matchingPlot.plantData.isFertilized = plotSave.plantData.isFertilized;
            matchingPlot.plantData.health = plotSave.plantData.health;
            matchingPlot.plantData.currentlyWP = plotSave.plantData.currentlyWP;
            matchingPlot.plantData.currentlyFP = plotSave.plantData.currentlyFP;
            matchingPlot.plantData.currentlyWithered = plotSave.plantData.currentlyWithered;
            matchingPlot.plantData.currentlyBroken = plotSave.plantData.currentlyBroken;
            matchingPlot.plantData.growthTimer = plotSave.plantData.growthTimer;
            matchingPlot.plantData.witherTimer = plotSave.plantData.witherTimer;
            matchingPlot.plantData.currentGrowthPhase = plotSave.plantData.currentGrowthPhase;
            matchingPlot.plantData.stopGrowing = plotSave.plantData.stopGrowing;
            matchingPlot.plantData.isAlive = plotSave.plantData.isAlive;
            matchingPlot.plantData.isReadyToHarvest =  plotSave.plantData.isReadyToHarvest;

        }
        else
        {
            //List<plotData> list = new();
            //list.Add(plotSave);
            gameData.plotList.Add(plotSave);
            //foreach (PlotData plotData in gameData.plotList)
            //{
            //    Debug.Log(plotData.hasPlant);
            //}
        }

        
    }

    public void OnDrop(PointerEventData eventData)
    {
        //if (eventData.pointerDrag.GetComponent<SeedScript>() != null 
        //    && LevelProperties.Instance.GetComponent<StatsScript>().moneyAvailable >= eventData.pointerDrag.GetComponent<SeedScript>().price) { 
        //    SeedDrop(eventData.pointerDrag.GetComponent<SeedScript>());
        //}
    }
}
