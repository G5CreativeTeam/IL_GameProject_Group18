using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PriceScript : MonoBehaviour
{
    private int price;
    private TextMeshProUGUI _textMeshProUGUI;
    private void Start()
    {
        price = GetComponentInParent<SeedScript>().price;
        _textMeshProUGUI = GetComponent<TextMeshProUGUI>();
        _textMeshProUGUI.text = price.ToString();
    }

    private void Update()
    {
        price = GetComponentInParent<SeedScript>().price;
        _textMeshProUGUI.text = price.ToString();
    }
}
