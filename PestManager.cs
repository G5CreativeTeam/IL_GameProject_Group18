using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using UnityEngine;

public class PestManager : MonoBehaviour, IDataPersistence
{
    // Start is called before the first frame update
    public GameObject[] pests;
    public LevelPestSpawner[] spawners;
    public GameObject playArea;
    public GameObject spawnArea;
    [HideInInspector]    public SerializableList<PestData> data;

    float realTimer;

    public void LoadData(GameData gameData)
    {
        foreach (PestData p in gameData.pestList)
        {
            GameObject pest;
            pest = Instantiate(pests[(int)p.type], spawnArea.transform.position, new Quaternion(0, 0, 0, 0),playArea.transform);
            pest.transform.position = new Vector3(p.x, p.y, p.z);
            pest.transform.localScale = new Vector3(p.scaleX, p.scaleY, p.scaleZ);
            pest.transform.SetParent(playArea.transform);
            pest.GetComponent<PestLogic>().originalPest = false;
            LevelProperties.Instance.GetComponent<StatsScript>().numOfPests++;

        }
        data = new();
    }

    public void SaveData(ref GameData gameData)
    {
        gameData.pestList = data;
    }

    void Start()
    {
        realTimer = 0;
        foreach (LevelPestSpawner spawner in spawners)
        {
            spawner.timer = 0;
        }
    }

    void Update()
    {
        if(LevelProperties.Instance != null)
        {
            if (LevelProperties.Instance.GetComponent<StatsScript>().numOfPlants > 0 && LevelProperties.Instance.gameOngoing)
            {
                realTimer += Time.deltaTime;
                foreach (LevelPestSpawner spawner in spawners)
                {
                    spawner.timer += Time.deltaTime;
                }
            }

            foreach (LevelPestSpawner spawner in spawners)
            {
                if (spawner.timer >= spawner.pestSpawnRate && LevelProperties.Instance.GetComponent<StatsScript>().numOfPests < LevelProperties.Instance.pestMax &&
                LevelProperties.Instance.GetComponent<StatsScript>().numOfPlants > 0)
                {
                    int randomPoint = Random.Range(0, spawner.spawnPoints.Length);
                    spawner.SpawnPest(spawner.spawnPoints[randomPoint].transform,this);
                    spawner.timer = 0;
                    if (LevelProperties.Instance.GetComponent<StatsScript>().numOfPests >= LevelProperties.Instance.pestMax)
                    {
                        break;
                    }
                }
                if (realTimer >= LevelProperties.Instance.multiplyAt) 
                {
                    spawner.pestSpawnRate /= LevelProperties.Instance.pestMultiply;
                    Debug.Log("Increased");
                    realTimer = 0;
                }
            }

        }
    }
}

[System.Serializable]
public class LevelPestSpawner
{
    public string label;
    [Header("Properties")]
    public GameObject[] pests;
    public float pestSpawnRate = 5;
    [ReadOnly] public float timer;
    //public int pestSwarm = 1;
    //public float swarmRate;

    //public GameObject eventSystem;

    public GameObject[] spawnPoints;

    public void SpawnPest(Transform point, PestManager pm)
    {
        int randomNum = Random.Range(0,pests.Length);
        GameObject pest = GameObject.Instantiate(pests[randomNum], point.position, new Quaternion(0, 0, 0, 0), point.parent);
        pest.transform.SetParent(pm.playArea.transform);
        pest.GetComponent<PestLogic>().originalPest = false;
        LevelProperties.Instance.GetComponent<StatsScript>().numOfPests++;
    }
}
