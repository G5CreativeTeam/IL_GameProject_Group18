using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EventSystem : MonoBehaviour
{
    public int level;
    public int difficultyLevel = 1;
    public int levelTime = 100;
    public int target = 1500;
    

    public GameObject ResultScreen;

    public struct levelGoals {
        public bool moneyBased;
        public bool plantNumBased;
        public bool guardPlantBased;
    }
   

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if (roundFinished() && Time.timeScale != 0)
        {
            Debug.Log("Game Finished!");
            StopGame();
            ResultScreen.SetActive(true);
            
        } 
    }

    public bool roundFinished()
    {
        return (int)Time.time == levelTime || gameObject.GetComponent<StatsScript>().moneyAvailable == target;
    }

    public void StopGame()
    {
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
    }

    public void exitToMainMenu()
    {
        SceneManager.LoadSceneAsync(0);
    }
}
