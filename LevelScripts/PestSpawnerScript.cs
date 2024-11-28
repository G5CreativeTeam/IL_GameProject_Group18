using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PestSpawnerScript : MonoBehaviour
{
    // Start is called before the first frame update
    private float pestTimer = 0;
    private float realTime = 0;
    private int pestMax;
    private float pestRate;
    private int realTimeTimer;
    

    public GameObject eventSystem;
    public GameObject[] pests;
    public GameObject[] spawnPoints;

    private int randomPoint ;

    void Start()
    {
        
        pestMax = eventSystem.GetComponent<LevelProperties>().pestMax;
        pestRate = eventSystem.GetComponent<LevelProperties>().pestSpawnRate;
        
    }

    // Update is called once per frame
    void Update()
    {
        pestTimer += Time.deltaTime;
        realTime += Time.deltaTime;
        
        if (pestTimer >= pestRate && eventSystem.GetComponent<StatsScript>().numOfPests < pestMax && eventSystem.GetComponent<StatsScript>().numOfPlants > 0)
        {
            for (int i = 0; i < eventSystem.GetComponent<LevelProperties>().pestEachSwarm; i++) {
                randomPoint = Random.Range(0, spawnPoints.Length);
                SpawnPest(spawnPoints[randomPoint].transform);
            }
            
            pestTimer = 0;
        }
        if (realTime > eventSystem.GetComponent<LevelProperties>().multiplyAt)
        {
            pestRate = pestRate / eventSystem.GetComponent<LevelProperties>().pestMultiply;
            realTime = 0;
        }
    }

    void SpawnPest(Transform point)
    {
        int randomNum = Random.Range(0, pests.Length);
        GameObject pest = Instantiate(pests[randomNum],point.position,new Quaternion(0,0,0,0),point.parent);
        pest.GetComponent<PestScript>().originalPest = false;
        eventSystem.GetComponent<StatsScript>().numOfPests++;
    }
}
