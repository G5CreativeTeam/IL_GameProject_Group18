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
    public bool gameCompleted;

    public bool deactivateDialogue;

    public SerializableList<PlotData> plotList;
    public SerializableList<PestData> pestList; 

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
        this.gameCompleted = false;

        this.elapsedTime = 0;
        this.deactivateDialogue = false;

        plotList = new();
        pestList = new();
    }

    
}

[System.Serializable]
public class PlotData
{
    public string id;
    public bool hasPlant;
    public PlantType PlantType;
    public PlantData plantData;
}

[System.Serializable]
public class PlantData
{
    public bool isWatered = false;
    public bool isFertilized = false;
    public bool currentlyWP;
    public bool currentlyFP;
    public bool currentlyHP;
    public bool currentlyBroken;
    public bool currentlyWithered;
    public float growthTimer;
    public float witherTimer;
    public int currentGrowthPhase;
    public bool stopGrowing = false;
    public bool isAlive = true;
    public bool isReadyToHarvest = false;
    public int health ;
    public bool isAttacked;
}

[System.Serializable]
public class PestData
{
    public float x;
    public float y;
    public float z;
    public float scaleX;
    public float scaleY;
    public float scaleZ;
    public float speed;
    public bool originalPest;
    public bool newSpawn;
    public PestType type;


}
