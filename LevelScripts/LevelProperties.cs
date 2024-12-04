using Ink.Parsed;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelProperties : MonoBehaviour, IDataPersistence
{
    [Header("General Properties")]
    public string levelText = "Level";
    public int levelTime = 60;
    public bool noTimerMode = false;
    public bool unlimitedMoney = false;
    public bool deactivateDialogue = false;
    public GameObjective[] Targets;
    public int nextLevelIndex;


    [Header("General Pest Properties")]
    //public float pestSpawnRate = 1;
    //public int pestEachSwarm = 5;
    public int pestMax = 15;
    public int pestSpeedMultiplier = 1;
    public int pestDamageMultiplier = 1;
    public float pestMultiply = 2;
    public float multiplyAt = 120;


    [Header("Related Screens & Objects")]
    public GameObject ResultScreen;
    public GameObject FinishedScreen;
    public GameObject countdownScreen;
    public GameObject GameUI;
    public GameObject dialogueUI;
    public GameObject MenuUI;
    public GameObject MoneyCounter;
    public GameObject FloatingText;
    public GameObject LevelLabelObject;

    [Header("Shortcut Keys")]
    public KeyCode pause;

    [HideInInspector] public float elapsedTime = 0.0f;
    [HideInInspector] public bool isCarryingObject = false;
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
        if (dialogueUI.activeInHierarchy == false && !deactivateDialogue)
        {
            dialogueUI.SetActive(true);
        } else
        {
            GameUI.SetActive(true);
            InitiateGame();
        }
        LevelLabelObject.GetComponent<TextMeshProUGUI>().text = levelText;
        Debug.Log(LevelLabelObject.GetComponent<TextMeshProUGUI>().text);
        

        if (noTimerMode)
        {
            GameUI.transform.Find("Timer").gameObject.SetActive(false);
            GameUI.transform.Find("Money").gameObject.SetActive(false);
            GameUI.transform.Find("Objectives").gameObject.SetActive(false);

        }
        foreach (GameObjective target in Targets)
        {
            target.Initialize(gameObject.GetComponent<StatsScript>());
        }
        MoneyCounter.GetComponent<TextMeshProUGUI>().text = gameObject.GetComponent<StatsScript>().moneyAvailable.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        MoneyCounter.GetComponent<TextMeshProUGUI>().text = gameObject.GetComponent<StatsScript>().moneyAvailable.ToString();
        if (gameOngoing)
        {
            elapsedTime += Time.deltaTime;
            
        }
        foreach (GameObjective target in Targets)
        {
            target.UpdateCompletion();
            
        }

        if (Input.GetKeyDown(pause) && gameOngoing)
        {
            if (!MenuUI.activeInHierarchy)
            {
                MenuUI.SetActive(true);
                StopGame();
            }
            else
            {
                MenuUI.SetActive(false);
                StartGame();
            }
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
        gameOngoing = false;
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
        //bool completeStatus = false;
        //foreach (GameObjective target in Targets)
        //{
        //    target.UpdateCompletion();
        //    completeStatus = target.completed;
        //}
        //return completeStatus;

        return elapsedTime >= levelTime + 1;
    }

    public void StopGame()
    {
        StopTime();
        
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

    public void LoadScene(int level)
    {
        SceneManager.LoadSceneAsync(level);
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

    public GameObject SpawnFloatingNumber(Transform transform,int number)
    {
        GameObject floatingNumber = Instantiate(FloatingText, new Vector3 (transform.position.x, transform.position.y,-5),Quaternion.identity,transform);
        
        //floatingNumber.GetComponent<RectTransform>().anchorMin = new Vector2(1,0);
        //Debug.Log(transform.localPosition.x);
        //Debug.Log(transform.localPosition.y);
        TextMeshProUGUI textComponent = floatingNumber.GetComponent<TextMeshProUGUI>();
        textComponent.text = number.ToString() ;
        if (number < 0)
        {
            floatingNumber.GetComponent<TextMeshProUGUI>().color = Color.red;
        }
        else
        {
            floatingNumber.GetComponent<TextMeshProUGUI>().color = Color.yellow;
            textComponent.text = "+"+number.ToString() ;

        }
        StartCoroutine(MoveFloatingText(floatingNumber));
        return floatingNumber;
    }

    private IEnumerator MoveFloatingText(GameObject floatingTextInstance)
    {
        float moveDuration = 1.5f; // Duration for movement
        float fadeDuration = 1f; // Duration for fade-out
        Vector3 startPosition = floatingTextInstance.transform.position;
        TextMeshProUGUI textComponent = floatingTextInstance.GetComponent<TextMeshProUGUI>();
        Vector3 endPosition = startPosition + new Vector3(0, 0.5f, 0); // Move upwards by 2 units
        Color originalColor = textComponent.color;

        // Step 1: Move the text to the target position
        float moveTimer = 0f;
        while (moveTimer < moveDuration)
        {
            floatingTextInstance.transform.position = Vector3.Lerp(startPosition, endPosition, moveTimer / moveDuration);
            moveTimer += Time.deltaTime;
            yield return null; // Wait until the next frame
        }

        // Ensure it snaps to the exact position at the end
        floatingTextInstance.transform.position = endPosition;

        // Step 2: Fade out the text after reaching the top position
        float fadeTimer = 0f;
        while (fadeTimer < fadeDuration)
        {
            float fadeProgress = fadeTimer / fadeDuration;
            textComponent.color = new Color(
                originalColor.r,
                originalColor.g,
                originalColor.b,
                Mathf.Lerp(1f, 0f, fadeProgress) // Reduce alpha to 0
            );
            fadeTimer += Time.deltaTime;
            yield return null;
        }

        // Ensure the text is fully invisible before destroying
        textComponent.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);

        Destroy(floatingTextInstance); // Destroy the floating text object
    }

    public void LoadData(GameData gameData)
    {
        this.elapsedTime = gameData.elapsedTime;
        this.deactivateDialogue = gameData.deactivateDialogue;
    }

    public void SaveData(ref GameData gameData)
    {
        gameData.elapsedTime = this.elapsedTime;
        gameData.deactivateDialogue = this.deactivateDialogue;
    }

    public void NextLevel ()
    {

    }
}
[System.Serializable]
public enum StatVariable
{
    score,
    seedPlanted,
    plantsHarvested ,
    carrotsHarvested ,
    potatoHarvested ,
    yamHarvested ,
    moneyAvailable,
}

[System.Serializable]
public class GameObjective
{
    public string label;
    public StatVariable targetVariable; // Enum for selecting variables
    public int targetValue;
    [HideInInspector] public bool completed = false;

    private StatsScript statScript;

    public void Initialize(StatsScript statScript)
    {
        this.statScript = statScript;
    }

    public int CurrentValue
    {
        get
        {
            if (statScript == null) return 0;

            // Access the selected variable from StatScripts
            switch (targetVariable)
            {
                case StatVariable.score:
                    return statScript.score;
                case StatVariable.seedPlanted:
                    return statScript.seedPlanted;
                case StatVariable.plantsHarvested:
                    return statScript.plantsHarvested;
                case StatVariable.carrotsHarvested:
                    return statScript.carrotsHarvested;
                case StatVariable.moneyAvailable:
                    return statScript.moneyAvailable;
                case StatVariable.yamHarvested:
                    return statScript.yamHarvested;
                case StatVariable.potatoHarvested:
                    return statScript.potatoHarvested;
                default:
                    return 0;
            }
        }
    }

    public void UpdateCompletion()
    {
        //if (completed) { return; }
        completed = CurrentValue >= targetValue;
    }

    
}
