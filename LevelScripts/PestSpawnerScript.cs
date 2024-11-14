using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PestSpawnerScript : MonoBehaviour
{
    // Start is called before the first frame update
    private float timer = 0;
    private int pestMax;
    private int pestRate;
    

    public GameObject eventSystem;
    public GameObject[] pests;
    public GameObject[] spawnPoints;

    private int randomPoint ;

    void Start()
    {
        
        pestMax = eventSystem.GetComponent<EventSystem>().pestMax;
        pestRate = eventSystem.GetComponent<EventSystem>().pestSpawnRate;
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        randomPoint = Random.Range(0, spawnPoints.Length);
        
        if (timer >= pestRate && eventSystem.GetComponent<EventSystem>().numOfPests < pestMax && eventSystem.GetComponent<EventSystem>().numOfPlants > 0)
        {
            SpawnPest(spawnPoints[randomPoint].transform);
            timer = 0;
        }

    }

    void SpawnPest(Transform point)
    {
        int randomNum = Random.Range(0, pests.Length);
        GameObject pest = Instantiate(pests[randomNum],point.position,new Quaternion(0,0,0,0),point.parent);
        pest.GetComponent<PestScript>().originalPest = false;
        eventSystem.GetComponent<EventSystem>().numOfPests++;
    }
}
