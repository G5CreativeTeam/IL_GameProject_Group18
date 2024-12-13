using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimerScript : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] GameObject EventSystem;
    float timerValue;
    float ongoingTime;
    // Update is called once per frame
    private void Start()
    {
        timerValue = EventSystem.GetComponent<LevelProperties>().levelTime+1;
    }

    void Update()
    {
        if (!LevelProperties.Instance.endlessMode)
        {
            if (timerValue > 0)
            {
                timerValue = (EventSystem.GetComponent<LevelProperties>().levelTime) - LevelProperties.Instance.elapsedTime;
                if (timerValue <= 6)
                {
                    timerText.color = new Color(0.9F, 0, 0, 1);
                }
            }
            else
            {
                timerValue = 0;
            }
        } else
        {
            timerValue = LevelProperties.Instance.elapsedTime;
        }
        
        int minutes = Mathf.FloorToInt(timerValue / 60);
        int seconds = Mathf.FloorToInt(timerValue % 60);
        timerText.text = string.Format("{0:00}:{1:00}",minutes,seconds);
    }
}
