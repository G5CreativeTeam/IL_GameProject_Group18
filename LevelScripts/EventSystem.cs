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


    public int pestSpawnRate = 1;
    public int pestMax = 15;
    public int pestSpeedMultiplier = 1;
    public int pestDamageMultiplier = 1;

    [HideInInspector] public int numOfPlants;
    [HideInInspector] public int numOfPests;

    public GameObject ResultScreen;
    public GameObject FinishedScreen;

    [HideInInspector] public float elapsedTime;
    private bool gameOngoing = false;
    public struct LevelGoals {
        public bool moneyBased;
        public bool plantNumBased;
        public bool guardPlantBased;
    }


    // Start is called before the first frame update
    private void Start()
    {
        elapsedTime = 0.0f;
        Time.timeScale = 1.0f;
        StartCoroutine(StartCountdown());
        
    }

    // Update is called once per frame
    void Update()
    {
        if (gameOngoing)
        {
            elapsedTime += Time.deltaTime;
            
        }
        
        if (RoundFinished() && gameOngoing)
        {
            Debug.Log("Game Finished!");
            StartCoroutine(EndResults());
   
        } 
    }

    IEnumerator EndResults()
    {
        Debug.Log("Finished");
        FinishedScreen.SetActive(true);
        yield return new WaitForSeconds(2);
        FinishedScreen.SetActive(false);
        Debug.Log("Double Finished");
        ResultScreen.SetActive(true);
        StopGame();
    }


    IEnumerator StartCountdown()
    {
        Debug.Log(3);
        yield return new WaitForSeconds(1);
        Debug.Log(2);
        yield return new WaitForSeconds(1);
        Debug.Log(1);
        yield return new WaitForSeconds(1);
        Debug.Log("Start!");
        yield return new WaitForSeconds(1);
        StartGame();
    }
    public bool RoundFinished()
    {
        return (int)elapsedTime == levelTime+1 ;
    }

    public void StopGame()
    {
        Time.timeScale = 0;
        gameOngoing = false;
    }

    public void StartGame()
    {
        Time.timeScale = 1;
        gameOngoing = true;
    }

    public void ExitToMainMenu()
    {
        SceneManager.LoadSceneAsync(0);
    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }
}
