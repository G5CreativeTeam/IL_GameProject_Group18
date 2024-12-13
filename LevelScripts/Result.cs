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
    public GameObject WinMessage ;
    public GameObject LoseMessage ;

    [Header("Labels")]
    public GameObject HeaderResultLabel;
    public GameObject MoneyLabel;
    //public GameObject ScoreLabel;
    public GameObject TubersLostLabel;
    public GameObject TubersPlantedLabel;
    public GameObject TargetLabel;

    public TextAsset[] WinInkFiles;
    public TextAsset[] LostInkFiles;

    public GameObject winButtonGroup;
    public GameObject loseButtonGroup;

    public DialogueManager DialogueManager;
    
    private bool status = false;

    void OnEnable()
    {
        DialogueManager = GetComponent<DialogueManager>();
        int randomNumber;
        MoneyLabel.GetComponent<TextMeshProUGUI>().text = LevelProperties.Instance.GetComponent<StatsScript>().moneyAvailable.ToString();
        //ScoreLabel.GetComponent<TextMeshProUGUI>().text = LevelProperties.Instance.GetComponent<StatsScript>().score.ToString();
        TubersLostLabel.GetComponent<TextMeshProUGUI>().text = LevelProperties.Instance.GetComponent<StatsScript>().plantsLost.ToString();
        TubersPlantedLabel.GetComponent<TextMeshProUGUI>().text = LevelProperties.Instance.GetComponent<StatsScript>().plantsHarvested.ToString();
        if (!LevelProperties.Instance.endlessMode)
        {
            TargetLabel.GetComponent<TextMeshProUGUI>().text = LevelProperties.Instance.GetComponent<LevelProperties>().Targets[0].targetValue.ToString();
        }
        else
        {
            TargetLabel.GetComponent<TextMeshProUGUI>().text = ((int)LevelProperties.Instance.elapsedTime).ToString();
        }
        


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
            WinMessage.SetActive(true);
            WinCharacter.SetActive(true);
            winButtonGroup.SetActive(true);
            randomNumber = Random.Range(0, WinInkFiles.Length);
            DialogueManager.inkFile = WinInkFiles[randomNumber];
        }
        else
        {
            LoseMessage.SetActive(true);
            LostCharacter.SetActive(true);
            loseButtonGroup.SetActive(true);
            randomNumber = Random.Range(0, LostInkFiles.Length);
            DialogueManager.inkFile = LostInkFiles[randomNumber];
        }



    }
}
