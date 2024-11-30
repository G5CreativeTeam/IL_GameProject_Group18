using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class ObjectiveManager : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject levelProperties;
    public string text;
    public GameObject objectiveLine;
    
    private TextMeshProUGUI textMeshPro;

    void Start()
    {
        textMeshPro = GetComponent<TextMeshProUGUI>();
        textMeshPro.text = text+"\n";
        foreach (GameObjective target in levelProperties.GetComponent<LevelProperties>().Targets)
        {
            //textMeshPro.text += $"{target.label} {target.targetValue}\n";
            //Debug.Log(textMeshPro.text);
            GameObject temp = Instantiate(objectiveLine, transform);
            temp.GetComponent<ObjectivePrint>().objective = target;
            //temp.GetComponent<ObjectivePrint>().setActive(true);
        }
    }

    // Update is called once per frame

}
