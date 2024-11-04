using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatsScript : MonoBehaviour
{
    public int score = 0;
    private int seedPlanted = 0;
    private int plantsLost = 0;

    public int moneyAvailable = 1000;

    public int addAmount(int value, int amount)
    {
        amount += value;
        return amount;
        
    }

    public int deductAmount(int value, int amount)
    {
        amount -= value;
        Debug.Log(amount);
        return amount;
    }
}
