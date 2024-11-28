using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq; 

public class GameDataPersistence : MonoBehaviour
{
    [Header("File Storage Config")]
    [SerializeField] private string fileName;

    
    private GameData gameData;
    private List<IDataPersistence> dataPersistenceObjects;
    private FileDataHandler dataHandler;
    public static GameDataPersistence Instance { get; private set; }

    
    private void Awake()
    {
        if (Instance  != null)
        {
            Debug.LogError("Found more than one Persistence Manager in the scene");
        }
        Instance = this;
    }

    private void Start()
    {
        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();
        LoadGame();
    }

    public void NewGame()
    {
        this.gameData = new GameData();
    }
    public void LoadGame()
    {
        this.gameData = dataHandler.Load();


        if (this.gameData == null)
        {
            NewGame();
        }
        foreach (IDataPersistence dataPersistanceObj in dataPersistenceObjects)
        {
            dataPersistanceObj.LoadData(gameData);
            
        }
        
    }

    public void SaveGame()
    {
        foreach(IDataPersistence dataPersistanceObj in dataPersistenceObjects)
        {
            
            dataPersistanceObj.SaveData(ref gameData);
            
        }


        dataHandler.Save(gameData);
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
}
