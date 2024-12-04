using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PestSpawnerScript : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Spawning Properties")]
    public float pestSpawnRate = 5;
    public int pestSwarm = 1;

    private float pestTimer = 0;
    private float realTime = 0;
    private float pestRate;

    

    //public GameObject eventSystem;
    public GameObject[] pests;
    public GameObject[] spawnPoints;
    public GameObject PlayArea;

    private int randomPoint ;


    // Update is called once per frame
    void Update()
    {
        if (LevelProperties.Instance.GetComponent<StatsScript>().numOfPlants > 0)
        {
            pestTimer += Time.deltaTime;
        }
        
        realTime += Time.deltaTime;
        
        if (pestTimer >= pestSpawnRate && 
            LevelProperties.Instance.GetComponent<StatsScript>().numOfPests < LevelProperties.Instance.pestMax && 
            LevelProperties.Instance.GetComponent<StatsScript>().numOfPlants > 0)
        {
            for (int i = 0; i < pestSwarm; i++) {
                randomPoint = Random.Range(0, spawnPoints.Length);
                SpawnPest(spawnPoints[randomPoint].transform);
                if (LevelProperties.Instance.GetComponent<StatsScript>().numOfPests >= LevelProperties.Instance.pestMax)
                {
                    break;
                }
            }
            
            pestTimer = 0;
        }
        if (realTime > LevelProperties.Instance.multiplyAt)
        {
            pestRate = pestRate / LevelProperties.Instance.pestMultiply;
            realTime = 0;
        }
    }

    void SpawnPest(Transform point)
    {
        int randomNum = Random.Range(0, pests.Length);
        GameObject pest = Instantiate(pests[randomNum],point.position,new Quaternion(0,0,0,0),point.parent);
        pest.transform.parent = PlayArea.transform;
        pest.GetComponent<PestScript>().originalPest = false;
        LevelProperties.Instance.GetComponent<StatsScript>().numOfPests++;
    }
}
