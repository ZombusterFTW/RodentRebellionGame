using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgrade : MonoBehaviour
{
    [SerializeField] float damageCount = 12f;
    [SerializeField] float damageCountCurrent = 0;
    [SerializeField] float frenzyMultiplier = 2f;
    [SerializeField] PlayerController playerController;
    public PlayerWeaponType playerWeaponType = PlayerWeaponType.None;
    private PlayerUI playerUIManager;
    private List<PlayerWeaponType> weaponList;
    private int weaponIndex = 0;




    // Start is called before the first frame update
    void Start()
    {
        playerController = GetComponent<PlayerController>();
        playerUIManager = playerController.GetPlayerUI();
        damageCountCurrent = damageCount;
        weaponList = new List<PlayerWeaponType>();
        weaponList.Add(PlayerWeaponType.None);
        playerUIManager.weaponIndentifier.text = weaponList[weaponIndex].ToString();
    }

    public void SwapWeapon(bool swapUp = true)
    {
        int weaponListCount = weaponList.Count - 1;
        int tempIndex = weaponIndex;
        //Attempt to go to next weapon
        if(swapUp ) 
        {
            tempIndex++;
            //If we go over the length of the list of weapons the player has go back to the starting weapon
            if (tempIndex > weaponListCount)
            {
                tempIndex = 0;
            }
        }
        else
        {
            tempIndex--;
            //If we go under the length of the list of weapons the player has go back to the last weapon
            if (tempIndex < 0)
            {
                tempIndex = weaponListCount;
            }
        }
        //Swap the weapon.
        weaponIndex = tempIndex;
        playerWeaponType = weaponList[tempIndex];
        //Show weapon name.
        playerUIManager.weaponIndentifier.text = weaponList[tempIndex].ToString();
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

    public void UnlockWeapon(PlayerWeaponType weapon)
    {
        //Prevent the same weapon from being added to the list if its already there
        if(!weaponList.Contains(weapon)) 
        {
            //Add the weapon to the list so the player can now swap between them.
            weaponList.Add(weapon);
            //Automatically swap player to the new weapon.
            playerWeaponType = weapon;
            weaponIndex = weaponList.IndexOf(weapon);
            playerUIManager.weaponIndentifier.text = weaponList[weaponIndex].ToString();
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

public enum PlayerWeaponType
{
    None,
    Dagger,
    LaserGun
}