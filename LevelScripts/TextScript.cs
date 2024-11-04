using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class TextScript : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject eventSystem;
    private int printedNumber;
    private TextMeshProUGUI textMeshPro;

    public bool printMoney;
    public bool printScore;
    public bool printTarget;
    public bool printPlantNum;


    void Start()
    {
        if (printMoney)
        {
            PrintMoney();
        } else if (printScore)
        {
            PrintScore();
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (printMoney)
        {
            PrintMoney();
        }
        else if (printScore)
        {
            PrintScore();
        }
    }

    void PrintScore()
    {
        printedNumber = eventSystem.GetComponent<StatsScript>().score;
        textMeshPro = GetComponent<TextMeshProUGUI>();
        textMeshPro.text = $"{printedNumber}";
    }

    void PrintMoney()
    {
        printedNumber = eventSystem.GetComponent<StatsScript>().moneyAvailable;
        textMeshPro = GetComponent<TextMeshProUGUI>();
        textMeshPro.text = $"{printedNumber}";
    }
}
