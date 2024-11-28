using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
[System.Serializable]
public class PlantScript : MonoBehaviour, IPointerClickHandler
{

    [HideInInspector] public bool originalPlant = true;
    [HideInInspector] public bool isWatered;
    [HideInInspector] public bool isFertilized;
    [HideInInspector] public bool currentlyWP;
    [HideInInspector] public bool currentlyFP;
    [HideInInspector] public float growthTimer;

    [Header("Growth Setting")]
    public float firstGrowthTime = 10.0f;
    public float secondGrowthTime = 10.0f;
    public float witherTime = 30.0f;

    [Header("Attributes")]
    public int health = 30;
    public int sellPrice = 200;
    public int scoreValue = 20;
    public Sprite[] plantSprites;

    [Header("Central Logic")]
    public GameObject eventSystem;

    [Header("Indicators")]
    public GameObject waterIndicator;
    public GameObject fertilizeIndicator;

    [Header("Audio")]
    public GameObject growAudio;
    public GameObject sellAudio;
    public GameObject witherAudio;

    private string[] growthPhases = new string[] { "sprout", "growing", "ripe", "withered", "broken-growing", "broken-ripe" };
    private float[] growthTimerSet  ;
    private int currentGrowthPhase = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        growthTimerSet = new float[] { firstGrowthTime, secondGrowthTime, witherTime };
        growthTimer = growthTimerSet[currentGrowthPhase];

        isWatered = false;
        isFertilized = false;
        currentlyWP = false;
        currentlyFP = false;

        if (!originalPlant)
        {
            if (!isWatered || !isFertilized)
            {
                gameObject.GetComponent<SpriteRenderer>().color = new Color(0.4f, 0.4f, 0.4f, 1);
            }
            else
            {
                gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
            }

            if (!isWatered && !currentlyWP )
            {
                WaterPhase();
            }
            if (!isFertilized && !currentlyFP)
            {
                FertilizerPhase();
            }

        }
    }

    private void Update()
    {
        
       // Debug.Log(growthTimer);

        if (!originalPlant && currentGrowthPhase != 3) {

            if (!isWatered || !isFertilized)
            {
                gameObject.GetComponent<SpriteRenderer>().color = new Color(0.4f, 0.4f, 0.4f, 1);
            } else
            {
                gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
            }
            //Debug.Log(!isWatered && !currentlyWP);
            if (!isWatered && !currentlyWP && !HarvestPhase())
            {
                WaterPhase();
            }
            if (!isFertilized && !currentlyFP)
            {
                FertilizerPhase();
            }
            if (growthTimer <= 0 && isWatered && isFertilized)
            {
                GrowNext();

            }
            if (isWatered && isFertilized)
            {
                growthTimer -= Time.deltaTime;
            }
        }
        if (health <= 0)
        {
            gameObject.GetComponentInParent<PlotScript>().hasPlant = false;
            Destroy(gameObject);
        }
    }

    public void GrowNext()
    {

        if (NotLastPhase()) //Pengondisian untuk mengecek tanaman belum sampai matang/tahap terakhir
        {
            SpriteRenderer spriteRender = GetComponent<SpriteRenderer>();
            spriteRender.sprite = plantSprites[currentGrowthPhase + 1];
            currentGrowthPhase++;
            
            growAudio.GetComponent<AudioSource>().Play();
            if (NotLastPhase()) {
                growthTimer = growthTimerSet[currentGrowthPhase];
            }
            

            if (!HarvestPhase())
            {
                isWatered = false;
            }
        }

    }

    public void WaterPhase()
    {
        SpriteRenderer spriteInfo = GetComponent<SpriteRenderer>();
        //spriteInfo.color = new Color(0.4F,0.4F,0.4F,1);
        GameObject indicator = Instantiate(waterIndicator, transform);
        indicator.transform.SetParent(gameObject.transform);
        currentlyWP = true;
        isWatered = false;
    }

    public void FertilizerPhase()
    {
        SpriteRenderer spriteInfo = GetComponent<SpriteRenderer>();

        
        GameObject indicator = Instantiate(fertilizeIndicator, transform);
        indicator.transform.SetParent(gameObject.transform);
        currentlyFP = true;
        isFertilized = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {

        if (HarvestPhase()) {
            
            eventSystem.GetComponent<StatsScript>().moneyAvailable = eventSystem.GetComponent<StatsScript>().AddAmount(sellPrice, eventSystem.GetComponent<StatsScript>().moneyAvailable);
            eventSystem.GetComponent<StatsScript>().score = eventSystem.GetComponent<StatsScript>().AddAmount(scoreValue * 2, eventSystem.GetComponent<StatsScript>().score);
            eventSystem.GetComponent<StatsScript>().plantsHarvested = eventSystem.GetComponent<StatsScript>().AddAmount(1, eventSystem.GetComponent<StatsScript>().plantsHarvested);
            sellAudio.GetComponent<AudioSource>().Play();
            Destroy(gameObject);

        }
    }

    public void BlinkEffect()
    {
        gameObject.GetComponent<SpriteRenderer>().color = new Color(0.3F, 0.3F, 0.3f, 0.8f);
    }
    public void TakeDamage(int amount)
    {
        health -= amount;
        Debug.Log($"OUCH!{health}");
    }

    public bool NotLastPhase()
    {
        return currentGrowthPhase < plantSprites.Length - 1;
    }

    public bool HarvestPhase() 
    {
        return currentGrowthPhase == 2;
    }
}
