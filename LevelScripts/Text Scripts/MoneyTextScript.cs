using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class MoneyTextScript : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject eventSystem;
    public string text;
    private int printedNumber;
    private TextMeshProUGUI textMeshPro;

    void Start()
    {
        printedNumber = eventSystem.GetComponent<StatsScript>().moneyAvailable;
        textMeshPro = GetComponent<TextMeshProUGUI>();
        textMeshPro.text = $"{printedNumber}";

    }

    // Update is called once per frame
    void Update()
    {
        printedNumber = eventSystem.GetComponent<StatsScript>().moneyAvailable;
        textMeshPro = GetComponent<TextMeshProUGUI>();
        textMeshPro.text = $"{printedNumber}";
    }

}
