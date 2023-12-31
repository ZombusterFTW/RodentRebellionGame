using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [Tooltip("How much health the player starts with")][SerializeField] private float healthCount = 100f;
    [SerializeField] private float healthCountCurrent;
    [SerializeField] private ControlledCharacter playerController;
    [SerializeField] private bool isPlayer = false;
    [SerializeField] private PlayerUI playerUIManager;

    // Start is called before the first frame update
    void Start()
    {
        healthCountCurrent = healthCount;
        playerController = GetComponent<ControlledCharacter>();
        playerUIManager = playerController.GetPlayerUI();
    }

    
    public float SubtractFromHealth(float healthToLose)
    {
        //Damaged taken during frenzy mode is halved
        if(isPlayer && playerController.GetPlayerController().frenzyActivated) 
        {
            healthToLose /= 2;
        }
        healthCountCurrent = Mathf.Clamp(healthCountCurrent-healthToLose, 0, healthCount);
        if (healthCountCurrent == 0)
        {
            playerController.RespawnPlayer();
        }
        else playerController.PlayDamagedAnim();
        playerUIManager.UpdateHealthBar(healthCountCurrent, healthCount);
        return healthCountCurrent;
    }


    public float AddToHealth(float healthToGain)
    {
        healthCountCurrent = Mathf.Clamp(healthCountCurrent + healthToGain, 0, healthCount);
        playerUIManager.UpdateHealthBar(healthCountCurrent, healthCount);
        return healthCountCurrent;
    }


    public void IncreaseHealthCap(float healthCapIncreasePercentage) 
    {
        healthCount += healthCount * healthCapIncreasePercentage;
        healthCountCurrent = healthCount;
        playerUIManager.UpdateHealthBar(healthCountCurrent, healthCount);
    }

    public float GetCurrentHealth()
    {
        return healthCountCurrent;
    }

    public void HealthToMax()
    {
        healthCountCurrent = healthCount;
        if(playerUIManager != null) playerUIManager.UpdateHealthBar(healthCountCurrent, healthCount);
    }


}
