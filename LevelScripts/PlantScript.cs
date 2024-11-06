using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlantScript : MonoBehaviour, IPointerClickHandler
{
    public float growthTime = 10.0f;
    public Sprite[] plantSprite;
    public bool originalPlant = true;

    public bool isWatered = true;
    public bool isFertilized = true;
    public float timeUntilWater = 5.0f;
    public float timeWithoutWater = 30.0f;

    public int health = 1000;
    public int sellPrice = 200;
    public int scoreValue = 20;
    
    
    public GameObject eventSystem;


    private int currentGrowthPhase = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (!originalPlant) {
            InvokeRepeating("GrowNext", growthTime, growthTime);
            InvokeRepeating("WaterPhase", timeUntilWater, timeUntilWater);
            //InvokeRepeating("FerilizerPhase")
            
        }
    }

    public void GrowNext()
    {

        // Pengondisian apabila terlalu lama dibiarkan saat sudah matang/tahap terakhir
        if (currentGrowthPhase == plantSprite.Length - 1)
        {
            PlotScript plot = gameObject.GetComponentInParent<PlotScript>();
            
            plot.hasPlant = false;
            Destroy(gameObject);
            eventSystem.GetComponent<StatsScript>().plantsLost = eventSystem.GetComponent<StatsScript>().addAmount(1, eventSystem.GetComponent<StatsScript>().plantsLost);

        } 
        if (currentGrowthPhase < plantSprite.Length-1) //Pengondisian untuk mengecek tanaman belum sampai matang/tahap terakhir
        {
            SpriteRenderer spriteRender = GetComponent<SpriteRenderer>();
            spriteRender.sprite = plantSprite[currentGrowthPhase+1];
            currentGrowthPhase++;
            
        }
        
    }
    public void HarvestPhase()
    {
        
    }

    public void WaterPhase()
    {
        SpriteRenderer spriteInfo = GetComponent<SpriteRenderer>();
        spriteInfo.color = new Color(0.4F,0.4F,0.4F,1);
        if (!isWatered)
        {
            gameObject.GetComponentInParent<PlotScript>().hasPlant = false;
            Destroy(gameObject);
            eventSystem.GetComponent<StatsScript>().plantsLost = eventSystem.GetComponent<StatsScript>().addAmount(1, eventSystem.GetComponent<StatsScript>().plantsLost);
        } else
        {
            Debug.Log("I Need Water!");
            
            isWatered = false;
        }
        
    }

    public void FertilizerPhase()
    {
        SpriteRenderer spriteInfo = GetComponent<SpriteRenderer>();
        spriteInfo.color = new Color(0.4F, 0.4F, 0.4F, 1);
        if (!isFertilized)
        {
            gameObject.GetComponentInParent<PlotScript>().hasPlant = false;
            Destroy(gameObject);
            eventSystem.GetComponent<StatsScript>().plantsLost = eventSystem.GetComponent<StatsScript>().addAmount(1, eventSystem.GetComponent<StatsScript>().plantsLost);
        }
        else
        {
            Debug.Log("I Need Pupuk bejirrrrrr plis lah bro");
            if (spriteInfo.color == new Color(61, 61, 61))
            {
                Debug.Log(spriteInfo.color);
            }
            isFertilized = false;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Sprite sprite = GetComponent<SpriteRenderer>().sprite;
        bool harvestPhase = sprite == plantSprite[plantSprite.Length - 1];
       
        if (harvestPhase) {
            eventSystem.GetComponent<StatsScript>().moneyAvailable = eventSystem.GetComponent<StatsScript>().addAmount(sellPrice, eventSystem.GetComponent<StatsScript>().moneyAvailable);
            eventSystem.GetComponent<StatsScript>().score = eventSystem.GetComponent<StatsScript>().addAmount(scoreValue*2, eventSystem.GetComponent<StatsScript>().score);
            eventSystem.GetComponent<StatsScript>().plantsHarvested = eventSystem.GetComponent<StatsScript>().addAmount(1, eventSystem.GetComponent<StatsScript>().plantsHarvested);
            Destroy(gameObject);
            
        } 
    }

    public void TakeDamage(int amount)
    {
        health -= amount;
    }
}
