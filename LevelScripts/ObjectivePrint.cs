using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ObjectivePrint : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObjective objective;
    public TextMeshProUGUI textHandler;

    void Start()
    {
        textHandler = GetComponent<TextMeshProUGUI>();
        textHandler.text = $"{objective.prefixLabel} {objective.targetValue} {objective.suffixLabel}";
    }

    // Update is called once per frame
    void Update()
    {
        textHandler.text = $"{objective.prefixLabel} {objective.targetValue} {objective.suffixLabel}";

        if (objective.completed)
        {

            //textHandler.color = Color.green;
        }
    }
}
