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

    public bool haveDoneDialogue;

    public SerializableList<plotData> plotList;

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
        this.haveDoneDialogue = false;

        plotList = new SerializableList<plotData>();
    }

    
}

[System.Serializable]
public class plotData
{
    public string id;
    public bool hasPlant;
    public GameObject plantObject;
    public PlantScript plant;
}
