using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;


public class GameDataPersistence : MonoBehaviour
{
    [Header("File Storage Config")]
    private string fileName;
    [SerializeField] private bool useEncryption;

    
    private GameData gameData;
    private List<IDataPersistence> dataPersistenceObjects;
    private FileDataHandler dataHandler;
    public static GameDataPersistence Instance { get; private set; }

    
    private void Awake()
    {
        fileName = SceneManager.GetActiveScene().name + "-saveFile.json";
        if (Instance  != null)
        {
            Debug.LogError("Found more than one Persistence Manager in the scene.  Destroying newest one");
            return;

        }
        Instance = this;

        this.dataHandler = new FileDataHandler(Application.persistentDataPath + "/saves", fileName, useEncryption);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();
        LoadGame();
    }

    public void OnSceneUnloaded(Scene scene)
    {
        SaveGame();
    }

    public void NewGame()
    {
        //if (this.dataHandler != null)
        //{
        //    this.dataPersistenceObjects.Clear();
        //    this.dataHandler = null;
        //}
        this.gameData = new GameData();
    }
    public void LoadGame()
    {
        this.gameData = dataHandler.Load();


        if (this.gameData == null)
        {
            return;
        }
        foreach (IDataPersistence dataPersistanceObj in dataPersistenceObjects)
        {
            dataPersistanceObj.LoadData(gameData);
            
        }
        Debug.Log("Loaded data");
    }

    public void SaveGame()
    {

        if (this.gameData == null)
        {
            return;
        }
        this.dataHandler = new FileDataHandler(Application.persistentDataPath + "/saves", fileName, useEncryption);
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();
        foreach (IDataPersistence dataPersistanceObj in dataPersistenceObjects)
        {
            
            dataPersistanceObj.SaveData(ref gameData);
            
        }


        dataHandler.Save(gameData);
        Debug.Log("Saved data");
    }

    private void OnApplicationQuit()
    {
        
        SaveGame();
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>()
           .OfType<IDataPersistence>();


        return new List<IDataPersistence>(dataPersistenceObjects);
    }

    public bool HasGameData()
    {
        return this.gameData != null;
    }
}
