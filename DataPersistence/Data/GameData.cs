using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int money;
    public int score;
    public int seedPlanted;
    public int plantsLost;
    public int plantsHarvested;
    public int moneyAvailable;
    public int numOfPlants;
    public int numOfPests;
    public int yamHarvested;
    public int carrotHarvested;
    public int potatoHarvested;

    public bool deactivateDialogue;

    public SerializableList<PlotData> plotList;

    public GameObject[] pests;
    public float elapsedTime;

    public GameData()
    { 
        this.score = 0;
        this.seedPlanted = 0;
        this.plantsLost = 0;
        this.plantsHarvested = 0;
        this.moneyAvailable = -1;
        this.numOfPlants = 0;
        this.numOfPests = 0;
        this.yamHarvested = 0;
        this.carrotHarvested = 0;
        this.potatoHarvested = 0;

        this.elapsedTime = 0;
        this.deactivateDialogue = false;


        plotList = new SerializableList<PlotData>();
    }

    
}

[System.Serializable]
public class PlotData
{
    public string id;
    public bool hasPlant;
    public PlantType PlantType;
    public GameObject plantObject;
    public PlantScript plant;
    public PlantData plantData;
}

[System.Serializable]
public class PlantData
{
    public bool originalPlant;
    public bool isWatered;
    public bool isFertilized;
    public bool currentlyWP;
    public bool currentlyFP;
    public float growthTimer;

    //[Header("Growth Setting")]
    //public float firstGrowthTime = 10.0f;
    //public float secondGrowthTime = 10.0f;
    //public float witherTime = 30.0f;

    [Header("Attributes")]
    public int health;
    //public int sellPrice;
    //public int scoreValue;
    //public PlantType plant;
    //public Sprite[] plantSprites;
    //public PlantPhases[] phases;

    //[Header("Central Logic")]
    //public GameObject levelProperties;

    //[Header("Indicators")]
    //public GameObject waterIndicator;
    //public GameObject fertilizeIndicator;
    //public GameObject harvestIndicator;
    //public GameObject attackedIndicator;
    //public GameObject deathIndicator;

    //[Header("Audio")]
    //public GameObject growAudio;
    //public GameObject sellAudio;
    //public GameObject witherAudio;

    //[Header("Animation")]
    //public float animSpeed = 0.03f;
    //public float interlude = 0.005f;
    
    //private string[] growthPhases = new string[] { "sprout", "growing", "ripe", "withered"};
    //private float[] growthTimerSet;
    private int currentGrowthPhase;
    private bool isAttacked;
    //private RectTransform rectTransform;
    //private GameObject indicatorPointer;
}
