using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class TextScript : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject eventSystem;
    int money;
    private TextMeshProUGUI textMeshPro;

    void Start()
    {
        
        money = eventSystem.GetComponent<MoneyScript>().moneyAvailable;
        textMeshPro = GetComponent<TextMeshProUGUI>();
        textMeshPro.text = $"Money\n{money}";
    }

    // Update is called once per frame
    void Update()
    {
        money = eventSystem.GetComponent<MoneyScript>().moneyAvailable;
        textMeshPro.text = $"Money\n{money}";
    }
}
