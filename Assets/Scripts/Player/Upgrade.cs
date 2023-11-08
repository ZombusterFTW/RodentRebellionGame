using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgrade : MonoBehaviour
{
    [SerializeField] float damageCount = 12f;
    [SerializeField] float damageCountCurrent = 0;
    [SerializeField] float frenzyMultiplier = 2f;
    [SerializeField] PlayerController playerController;
    private PlayerUI playerUIManager;
    private List<Upgrade> upgradeList;
    // Start is called before the first frame update
    void Start()
    {
        playerController = GetComponent<PlayerController>();
        playerUIManager = playerController.GetPlayerUI();

        damageCountCurrent = damageCount;

    }

    // Update is called once per frame
    void Update()
    {
        
    }




    public float GetAttackDamage(PlayerAttackType attackType)
    {
        float temp;
        switch(attackType) 
        {
            default: return damageCount / 3;
            case PlayerAttackType.DaggerStrike: temp = damageCountCurrent / 2;
                break;
            case PlayerAttackType.GroundPound: temp =  damageCountCurrent;
                break;
            case PlayerAttackType.StandardAttack: temp =  damageCountCurrent / 3;
                break;
            case PlayerAttackType.LaserBlast: temp =  damageCountCurrent / 5;
                break;
        }

        if (playerController.frenzyActivated) temp *= frenzyMultiplier;

        return temp;

    }

    public void UnlockAbility(UpgradeType upgrade)
    {
        //Give player the upgrade and add it to the upgrade list. Run function on the player that will set bools and allow for ability usage.
        switch (upgrade) 
        {
            default: return;
            

        }
    }

    public void UpgradeAttackDamage(float damageIncreasePercentage)
    {
        damageCount += damageCount * damageIncreasePercentage;
        damageCountCurrent = damageCount;
    }





}

public enum PlayerAttackType
{
    DaggerStrike,
    AirFlip,
    GroundPound,
    StandardAttack,
    LaserBlast
}