using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;
using System.ComponentModel;

public class StatsScript : MonoBehaviour, IDataPersistence
{
    [HideInInspector] public int score = 0;
    [HideInInspector] public int seedPlanted = 0;
    public int plantsLost = 0;
    [HideInInspector] public int plantsHarvested = 0;
    
    public int numOfPlants = 0;
    public int numOfPests = 0;
    
    [HideInInspector] public int carrotsHarvested = 0;
    [HideInInspector] public int potatoHarvested = 0;
    [HideInInspector] public int yamHarvested = 0;

    public int moneyAvailable = 1000;

    public int AddAmount(int value, int amount)
    {
        amount += value;
        return amount;
        
    }

    public void DeductAmount(int value)
    {
        moneyAvailable -= value;
       
    }

    public void LoadData(GameData gameData)
    {
        this.score = gameData.score;
        this.seedPlanted = gameData.seedPlanted;
        this.plantsLost = gameData.plantsLost;
        this.plantsHarvested = gameData.plantsHarvested;
        
        this.numOfPests = gameData.numOfPests;
        this.numOfPlants = gameData.numOfPlants;
        this.carrotsHarvested = gameData.carrotHarvested;
        this.potatoHarvested = gameData.potatoHarvested;
        this.yamHarvested = gameData.yamHarvested;

        if (gameData.moneyAvailable != -1)
        {
            this.moneyAvailable = gameData.moneyAvailable;
        }
    }

    public void SaveData(ref GameData gameData)
    {
        gameData.score = this.score;
        gameData.seedPlanted = this.seedPlanted;
        gameData.plantsLost = this.plantsLost;
        gameData.plantsHarvested = this.plantsHarvested;
        gameData.moneyAvailable = this.moneyAvailable;
        gameData.numOfPlants = this.numOfPlants;
        gameData.numOfPests = this.numOfPests;
        gameData.carrotHarvested = this.carrotsHarvested;
        gameData.potatoHarvested= this.potatoHarvested;
        gameData.yamHarvested= this.yamHarvested;
    }
}
