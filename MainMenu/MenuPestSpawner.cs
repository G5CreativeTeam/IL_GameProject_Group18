using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MenuPestSpawner : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Spawning Properties")]
    public float pestSpawnRate = 5;
    public int pestSwarm = 1;
    public int pestMax;

    [SerializeField] private float pestTimer = 0;
    private float realTime = 0;
    private float pestRate;

    public int numOfPests;
    

    //public GameObject eventSystem;
    public GameObject[] pests;
    public GameObject[] spawnPoints;
    public GameObject PlayArea;

    private int randomPoint;


    // Update is called once per frame
    void Update()
    {


            pestTimer += Time.deltaTime;

            if (pestTimer >= pestSpawnRate &&
                numOfPests < pestMax 
                )
            {
                for (int i = 0; i < pestSwarm; i++)
                {
                    randomPoint = Random.Range(0, spawnPoints.Length);
                    SpawnPest(spawnPoints[randomPoint].transform);
                    if (numOfPests >= pestMax)
                    {
                        break;
                    }
                }

                pestTimer = 0;
            }
        
        realTime += Time.deltaTime;

    }

    public void SpawnPest(Transform point)
    {
        int randomNum = Random.Range(0, pests.Length);
        GameObject pest = Instantiate(pests[randomNum], point.position, new Quaternion(0, 0, 0, 0), point.parent);
        pest.transform.SetParent(PlayArea.transform);

        pest.GetComponent<Collider2D>().enabled = false; // Temporarily disable
        pest.GetComponent<Collider2D>().enabled = true;  // Re-enable to refresh bounds

        pest.GetComponent<PestLogic>().originalPest = false;
        numOfPests++;
    }
}
