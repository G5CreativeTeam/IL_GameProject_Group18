using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLogicScript : MonoBehaviour
{
    public int level;
    public int difficultyLevel = 1;
    public int defaultTime = 540;

    public struct levelGoals {
        public bool moneyBased;
        public bool plantNumBased;
        public bool guardPlantBased;
    }
   

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool LevelGoal(int[] goals)
    {
        return true;
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
    }
}
