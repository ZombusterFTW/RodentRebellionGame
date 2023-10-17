using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgrade : MonoBehaviour
{
    [SerializeField] float damageCount = 12f;
    [SerializeField] float damageCountCurrent = 0;
    [SerializeField] PlayerController playerController;
    private PlayerUI playerUIManager;
    private List<Upgrade> upgradeList;
    // Start is called before the first frame update
    void Start()
    {
        playerController = GetComponent<PlayerController>();
        playerUIManager = playerController.GetPlayerUI();
    }

    // Update is called once per frame
    void Update()
    {
        
    }




    public float GetAttackDamage(PlayerAttackType attackType)
    {
        switch(attackType) 
        {
            default: return damageCount / 3;




        }
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