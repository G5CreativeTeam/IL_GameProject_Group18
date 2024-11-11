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
    public GameObject FinishedScreen;

    [HideInInspector] public float elapsedTime = 0.0f;
    private bool gameOngoing = false;
    public struct LevelGoals {
        public bool moneyBased;
        public bool plantNumBased;
        public bool guardPlantBased;
    }


    // Start is called before the first frame update
    private void Start()
    {
        
        StartCoroutine(StartCountdown());
        
    }

    // Update is called once per frame
    void Update()
    {
        if (gameOngoing)
        {
            elapsedTime += Time.deltaTime;
            Debug.Log("Check");
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
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }
}
