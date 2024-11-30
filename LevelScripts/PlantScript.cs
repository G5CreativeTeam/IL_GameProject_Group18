using System.Collections;
using System.Collections.Generic;
using System.Data;
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
    [SerializeField] private PlantType plant;
    public Sprite[] plantSprites;
    public PlantPhases[] phases ;

    [Header("Central Logic")]
    public GameObject levelProperties;

    [Header("Indicators")]
    public GameObject waterIndicator;
    public GameObject fertilizeIndicator;
    public GameObject harvestIndicator;
    public GameObject attackedIndicator;

    [Header("Audio")]
    public GameObject growAudio;
    public GameObject sellAudio;
    public GameObject witherAudio;

    [Header("Animation")]
    public float animSpeed = 0.03f;
    public float interlude = 0.005f;
    private int index = 0;

    //private string[] growthPhases = new string[] { "sprout", "growing", "ripe", "withered"};
    private float[] growthTimerSet  ;
    private int currentGrowthPhase = 0;
    private bool isAttacked = false;
    
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
            StartCoroutine(Animate());
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
            if (!isWatered && !currentlyWP && currentGrowthPhase != 2)
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
            gameObject.GetComponentInParent<SpriteRenderer>().sprite = gameObject.GetComponentInParent<PlotScript>().DryLand;
            Destroy(gameObject);
            LevelProperties.Instance.GetComponent<StatsScript>().numOfPlants--;
        }
    }

    public void GrowNext()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        if (NotLastPhase()) //Pengondisian untuk mengecek tanaman belum sampai matang/tahap terakhir
        {
            StopCoroutine(Animate());

            SpriteRenderer spriteRender = GetComponent<SpriteRenderer>();
            spriteRender.sprite = plantSprites[currentGrowthPhase + 1];
            currentGrowthPhase++;
            
            growAudio.GetComponent<AudioSource>().Play();
            if (NotLastPhase()) {
                growthTimer = growthTimerSet[currentGrowthPhase];
            }

            isWatered = false;
            if (currentGrowthPhase == 2)
            {
                HarvestPhase();
            }
            StartCoroutine(Animate());
        } else
        {
            
        }

    }

    public void WaterPhase()
    {
        //SpriteRenderer spriteInfo = GetComponent<SpriteRenderer>();
        //spriteInfo.color = new Color(0.4F,0.4F,0.4F,1);
        GameObject indicator = Instantiate(waterIndicator, transform);
        indicator.transform.SetParent(gameObject.transform);
        currentlyWP = true;
        isWatered = false;
    }

    public void FertilizerPhase()
    {
        //SpriteRenderer spriteInfo = GetComponent<SpriteRenderer>();

        
        GameObject indicator = Instantiate(fertilizeIndicator, transform);
        indicator.transform.SetParent(gameObject.transform);
        currentlyFP = true;
        isFertilized = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {

        if (currentGrowthPhase == 2) {
            
            levelProperties.GetComponent<StatsScript>().moneyAvailable = levelProperties.GetComponent<StatsScript>().AddAmount(sellPrice, levelProperties.GetComponent<StatsScript>().moneyAvailable);
            levelProperties.GetComponent<StatsScript>().score = levelProperties.GetComponent<StatsScript>().AddAmount(scoreValue * 2, levelProperties.GetComponent<StatsScript>().score);
            levelProperties.GetComponent<StatsScript>().plantsHarvested = levelProperties.GetComponent<StatsScript>().AddAmount(1, levelProperties.GetComponent<StatsScript>().plantsHarvested);
            sellAudio.GetComponent<AudioSource>().Play();
            PlantAdd(plant);
            isWatered = false;
            Destroy(gameObject);
            

        }
    }

    public void BlinkEffect()
    {
        gameObject.GetComponent<SpriteRenderer>().color = new Color(0.3F, 0.3F, 0.3f, 0.8f);
    }

    public IEnumerator TakeDamage(int amount)
    {
        health -= amount;
        
        if (!isAttacked)
        {
            isAttacked = true;
            GameObject indicator = Instantiate(attackedIndicator, transform);
            indicator.transform.SetParent(gameObject.transform);
            yield return new WaitForSeconds(1.5f);
            Destroy(indicator);
            isAttacked = false;
        } else
        {
            yield return new WaitForSeconds(1.5f);
        }
        
        StopCoroutine(TakeDamage(amount));
    }

    public bool NotLastPhase()
    {
        return currentGrowthPhase < plantSprites.Length - 1;
    }

    public void HarvestPhase() 
    {
        
        isWatered = true;
        GameObject indicator = Instantiate(harvestIndicator, transform);
        indicator.transform.SetParent(gameObject.transform);
        
    }

    public void PlantAdd(PlantType plant)
    {
        switch (plant)
        {
            case PlantType.carrot:
                LevelProperties.Instance.GetComponent<StatsScript>().carrotsHarvested++;
                return ;
            case PlantType.potato:
                LevelProperties.Instance.GetComponent<StatsScript>().potatoHarvested++;
                return;
            case PlantType.yam:
                LevelProperties.Instance.GetComponent<StatsScript>().yamHarvested++;
                return;
        }
    }

    public IEnumerator Animate()
    {
        while (true)
        {
            while (index < phases[currentGrowthPhase].plantSprites.Length)
            {
                gameObject.GetComponent<SpriteRenderer>().sprite = phases[currentGrowthPhase].plantSprites[index];

                yield return new WaitForSeconds(animSpeed);
                index++;
            }
            index = 0;
        }

    }
}

public enum PlantType
{
    carrot,
    potato,
    yam
}

[System.Serializable]
public class PlantPhases
{
    public Sprite[] plantSprites;
}