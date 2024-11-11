using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimerScript : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] GameObject EventSystem;
    float remainingTime;
    // Update is called once per frame
    private void Start()
    {
        remainingTime = EventSystem.GetComponent<EventSystem>().levelTime+1;
    }

    void Update()
    {
        if (remainingTime > 0)
        {
            remainingTime = (EventSystem.GetComponent<EventSystem>().levelTime) - EventSystem.GetComponent<EventSystem>().elapsedTime;
            if (remainingTime <= 6)
            {
                timerText.color = new Color(0.9F, 0, 0, 1);
            }
        } else
        {
            remainingTime = 0;
        }
        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}",minutes,seconds);
    }
}
