using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoneyScript : MonoBehaviour
{
    public int moneyAvailable = 1000;
    private TextMeshProUGUI textMeshPro;


    public void Start()
    {
        textMeshPro = GetComponent<TextMeshProUGUI>();
        textMeshPro.text = moneyAvailable.ToString();
    }

    public void Update()
    {
        textMeshPro.text = moneyAvailable.ToString();
    }

    public void addMoney(int value)
    {
        moneyAvailable += value;
    }

    public void deductMoney(int value)
    {
        moneyAvailable -= value;
    }
}
