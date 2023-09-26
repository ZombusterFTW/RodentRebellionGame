using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [Tooltip("How much health the player starts with")][SerializeField] private float healthCount = 100f;
    private float healthCountCurrent;
    [SerializeField] private PlayerController playerController;



    // Start is called before the first frame update
    void Start()
    {
        healthCountCurrent = healthCount;
        playerController = GetComponent<PlayerController>();
    }

    
    public float SubtractFromHealth(float healthToLose)
    {
        healthCountCurrent = Mathf.Clamp(healthCountCurrent-healthToLose, 0, healthCount);  
        if(healthCountCurrent == 0 ) playerController.RespawnPlayer();
        return healthCountCurrent;
    }


    public float AddToHealth(float healthToGain)
    {
        healthCountCurrent = Mathf.Clamp(healthCountCurrent + healthToGain, 0, healthCount);
        return healthCountCurrent;
    }


    public void IncreaseHealthCap(float healthCapIncreasePercentage) 
    {
        healthCount += healthCount * healthCapIncreasePercentage;
        healthCountCurrent = healthCount;
    }

    public float GetCurrentHealth()
    {
        return healthCountCurrent;
    }

    public void HealthToMax()
    {
        healthCountCurrent = healthCount; 
    }


}
