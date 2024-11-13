using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class ResultText : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject eventSystem;
 
    private string ResultMessage;
    private TextMeshProUGUI textMeshPro;

    void Start()
    {
        if (eventSystem.GetComponent<EventSystem>().target <= eventSystem.GetComponent<StatsScript>().moneyAvailable)
        {
            ResultMessage = "You Won!";
        } else
        {
            ResultMessage = "You Lost!";
        }
        
        textMeshPro = GetComponent<TextMeshProUGUI>();
        textMeshPro.text = $"{ResultMessage}";

    }

    // Update is called once per frame


}
