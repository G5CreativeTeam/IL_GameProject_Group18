using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreScript : MonoBehaviour
{
    public int currentScore = 0;
    public TextMeshProUGUI textMeshPro;

    public void Start()
    {
        textMeshPro = GetComponent<TextMeshProUGUI>();
        textMeshPro.text = currentScore.ToString();
    }

    public void Update()
    {
        textMeshPro.text = currentScore.ToString();
    }
    public void addScore(int value)
    {
        currentScore += value;
    }

    public void deductScore(int value)
    {
        currentScore -= value;
    }
}
