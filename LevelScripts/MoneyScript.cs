using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoneyScript : MonoBehaviour
{
    public int moneyAvailable = 1000;

    

    public void addMoney(int value)
    {
        moneyAvailable += value;
    }

    public void deductMoney(int value)
    {
        moneyAvailable -= value;
    }
}
