using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EventSystem : MonoBehaviour
{
    public int level;
    public int difficultyLevel = 1;
    public int levelTime = 60;
    public int target = 1500;
    

    public GameObject ResultScreen;

    private float elapsedTime = 0.0f;
    public struct LevelGoals {
        public bool moneyBased;
        public bool plantNumBased;
        public bool guardPlantBased;
    }


    // Start is called before the first frame update
    private void Start()
    {
        StartGame();
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;
        if (roundFinished() && Time.timeScale != 0)
        {
            Debug.Log("Game Finished!");
            StopGame();
            ResultScreen.SetActive(true);
            
        } 
    }

    public bool roundFinished()
    {
        return (int)elapsedTime == levelTime+1 || gameObject.GetComponent<StatsScript>().moneyAvailable == target;
    }

    public void StopGame()
    {
        Time.timeScale = 0;
    }

    public void StartGame()
    {
        Time.timeScale = 1;
    }

    public void exitToMainMenu()
    {
        SceneManager.LoadSceneAsync(0);
    }
}
