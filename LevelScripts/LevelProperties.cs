using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelProperties : MonoBehaviour, IDataPersistence
{
    [Header("General Properties")]
    public int levelTime = 60;
    public int target = 1500;
    public bool noTimerMode = false;
    public bool unlimitedMoney = false;
    public bool haveDoneDialogue = false;

    [Header("Pest Properties")]
    public float pestSpawnRate = 1;
    public int pestEachSwarm = 5;
    public int pestMax = 15;
    public int pestSpeedMultiplier = 1;
    public int pestDamageMultiplier = 1;
    public float pestMultiply = 2;
    public int multiplyAt = 120;


    [Header("Related Screens & Objects")]
    public GameObject ResultScreen;
    public GameObject FinishedScreen;
    public GameObject countdownScreen;
    public GameObject GameUI;
    public GameObject dialogueUI;

    [HideInInspector] public float elapsedTime = 0.0f;
    [HideInInspector] public bool isCarryingObject;
    [HideInInspector] public GameObject objectCarried;



    private bool gameOngoing = false;

    public static LevelProperties Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Found more than one Persistence Manager in the scene");
        }
        Instance = this;
    }

    // Start is called before the first frame update
    private void Start()
    {
        if (dialogueUI.activeInHierarchy == false && !haveDoneDialogue)
        {
            dialogueUI.SetActive(true);
        } else
        {
            GameUI.SetActive(true);
            InitiateGame();
        }

        if (noTimerMode)
        {
            GameUI.transform.Find("Timer").gameObject.SetActive(false);
            GameUI.transform.Find("Money").gameObject.SetActive(false);
            GameUI.transform.Find("Objectives").gameObject.SetActive(false);

        }
        
        
    }

    // Update is called once per frame
    void Update()
    {
        if (gameOngoing)
        {
            elapsedTime += Time.deltaTime;
            
        }
        if (!noTimerMode)
        {
            if (RoundFinished() && gameOngoing)
            {
                StartCoroutine(EndResults());
            }
        }
    }

    public void InitiateGame()
    {
        StartTime();
        StartCoroutine(StartCountdown());
    }

    IEnumerator EndResults()
    {
        
        FinishedScreen.SetActive(true);
        yield return new WaitForSeconds(2);
        FinishedScreen.SetActive(false);
        
        ResultScreen.SetActive(true);
        StopGame();
    }


    IEnumerator StartCountdown()
    {
        countdownScreen.SetActive(true);

        countdownScreen.transform.GetChild(0).gameObject.SetActive(true);
        yield return new WaitForSeconds(1);

        countdownScreen.transform.GetChild(0).gameObject.SetActive(false);
        countdownScreen.transform.GetChild(1).gameObject.SetActive(true);
        yield return new WaitForSeconds(1);

        countdownScreen.transform.GetChild(1).gameObject.SetActive(false);
        countdownScreen.transform.GetChild(2).gameObject.SetActive(true);
        yield return new WaitForSeconds(1);
        countdownScreen.transform.GetChild(2).gameObject.SetActive(false);
        countdownScreen.transform.GetChild(3).gameObject.SetActive(true);
        
        yield return new WaitForSeconds(1);
        countdownScreen.SetActive(false);
        StartGame();
    }
    public bool RoundFinished()
    {
        return (int)elapsedTime == levelTime+1 ;
    }

    public void StopGame()
    {
        StopTime();
        gameOngoing = false;
    }

    public void StartGame()
    {
        StartTime();
        gameOngoing = true;
    }

    public void StartTime()
    {
        Time.timeScale = 1;
    }

    public void StopTime()
    {
        Time.timeScale = 0;
    }

    public void QuitToMainMenu()
    {
        SceneManager.LoadSceneAsync(0);
    }

    public void EndGame()
    {
        elapsedTime = (levelTime + 1) - 5;
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadData(GameData gameData)
    {
        this.elapsedTime = gameData.elapsedTime;
        this.haveDoneDialogue = gameData.haveDoneDialogue;
    }

    public void SaveData(ref GameData gameData)
    {
        gameData.elapsedTime = this.elapsedTime;
        gameData.haveDoneDialogue = this.haveDoneDialogue;
    }
}
