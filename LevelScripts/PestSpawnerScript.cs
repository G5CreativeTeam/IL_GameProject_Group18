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
    
    void Start()
    {
        pestMax = eventSystem.GetComponent<EventSystem>().pestMax;
        pestRate = eventSystem.GetComponent<EventSystem>().pestSpawnRate;
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= pestRate && eventSystem.GetComponent<EventSystem>().numOfPests < pestMax && eventSystem.GetComponent<EventSystem>().numOfPlants > 0)
        {
            SpawnPest();
            timer = 0;
        }

    }

    void SpawnPest()
    {
        int randomNum = Random.Range(0, pests.Length);
        GameObject pest = Instantiate(pests[randomNum],transform.position,new Quaternion(0,0,0,0),transform.parent);
        pest.GetComponent<PestScript>().originalPest = false;
        eventSystem.GetComponent<EventSystem>().numOfPests++;
    }
}
