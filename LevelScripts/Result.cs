using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Result : MonoBehaviour
{
    [Header("Characters")]
    public GameObject WinCharacter;
    public GameObject LostCharacter;

    [Header("Messages")]
    public string WinMessage = "Great Job!";
    public string LoseMessage = "Better luck next time!";

    [Header("Labels")]
    public GameObject HeaderResultLabel;
    public GameObject MoneyLabel;
    //public GameObject ScoreLabel;
    public GameObject TubersLostLabel;
    public GameObject TubersPlantedLabel;
    public GameObject TargetLabel;

    public GameObject winButtonGroup;
    public GameObject loseButtonGroup;
    
    private string ResultMessage;
    
    private bool status = false;

    void OnEnable()
    {
        MoneyLabel.GetComponent<TextMeshProUGUI>().text = LevelProperties.Instance.GetComponent<StatsScript>().moneyAvailable.ToString();
        //ScoreLabel.GetComponent<TextMeshProUGUI>().text = LevelProperties.Instance.GetComponent<StatsScript>().score.ToString();
        TubersLostLabel.GetComponent<TextMeshProUGUI>().text = LevelProperties.Instance.GetComponent<StatsScript>().plantsLost.ToString();
        TubersPlantedLabel.GetComponent<TextMeshProUGUI>().text = LevelProperties.Instance.GetComponent<StatsScript>().plantsHarvested.ToString();
        TargetLabel.GetComponent<TextMeshProUGUI>().text = LevelProperties.Instance.GetComponent<LevelProperties>().Targets[0].targetValue.ToString();


        foreach (GameObjective target in LevelProperties.Instance.Targets)
        {
            status = target.completed;
            
            if (!status)
            {
                break;
            }
        }
        if (status)
        {
            ResultMessage = WinMessage;
            WinCharacter.SetActive(true);
            winButtonGroup.SetActive(true);
        }
        else
        {
            ResultMessage = LoseMessage;
            LostCharacter.SetActive(true);
            loseButtonGroup.SetActive(true);
        }

        HeaderResultLabel.GetComponent<TextMeshProUGUI>().text = $"{ResultMessage}";

    }
}
