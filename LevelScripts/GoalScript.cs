using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalScript : MonoBehaviour
{
    public ScoreScript score;
    public MoneyScript money;
    public int goal;
    private bool completeStatus;
    // Start is called before the first frame update
    
    public void checkMoneyResults(MoneyScript money)
    {
        completeStatus = money.moneyAvailable == goal;

    }

    public void checkPlants(int plantNum)
    {
        completeStatus = plantNum == goal;
    }

    public void underTime(int time)
    {
        completeStatus = time <= goal;
    }


}
