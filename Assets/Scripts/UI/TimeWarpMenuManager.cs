using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TimeWarpMenuManager : MonoBehaviour
{
    [SerializeField] private TimeWarpButtonContainer[] timeWarpButtons;


    void Start()
    {
        SaveData.instance.LoadFromJson();

        SaveData.instance.playerSettingsConfig.playerInTimeWarpMode = true;
        //If player is in timewarp mode we disregard speedrun mode. As these are seperate features.

        //Will add seperate saving behavior to the exit door script.

        //loop over all timewarpbuttonobjects

        for (int i = 0; i < timeWarpButtons.Length; i++)
        {
            TimeSpan tsWarp = System.TimeSpan.FromSeconds(SaveData.instance.practiceModeLevelSettings.warpModeLevelTimesPlayer[i]);
            TimeSpan normalLevel = System.TimeSpan.FromSeconds(SaveData.instance.playerSaveData.playerBestRun[i]);


            //Grab players best time from that level. If this number is zero, we attempt to load the players best run from that stage.
            if (SaveData.instance.practiceModeLevelSettings.warpModeLevelTimesPlayer[i] != 0) timeWarpButtons[i].bestPlayerTime.text = "Best Time: " + tsWarp.ToString("mm") + ":" + tsWarp.ToString("ss") + ":" + tsWarp.ToString("ff");
            else timeWarpButtons[i].bestPlayerTime.text = "Best Time: " + normalLevel.ToString("mm") + ":" + normalLevel.ToString("ss") + ":" + normalLevel.ToString("ff");
            //Load the dev times
            timeWarpButtons[i].devTime.text = "Dev Time: " + System.TimeSpan.FromSeconds(SaveData.instance.practiceModeLevelSettings.warpModeLevelTimes[i]).ToString();
            //Load collectibles data. If its zero we attempt to pull from previously saved data.
            if (SaveData.instance.practiceModeLevelSettings.warpModeCollectibleCountPlayer[i] != 0) timeWarpButtons[i].collectibleCount.text = "Collectibles: " + SaveData.instance.practiceModeLevelSettings.warpModeCollectibleCountPlayer[i] + "/";
            else timeWarpButtons[i].collectibleCount.text = "Collectibles: " + SaveData.instance.playerSaveData.playerCollectiblesTracker[i] + "/";
            if (SaveData.instance.practiceModeLevelSettings.warpModeCollectibleCount[i] == 0) timeWarpButtons[i].collectibleCount.text += "?";
            else timeWarpButtons[i].collectibleCount.text += SaveData.instance.practiceModeLevelSettings.warpModeCollectibleCount[i];



            //Color the text if the player is a above or below dev time
            if (SaveData.instance.practiceModeLevelSettings.warpModeLevelTimesPlayer[i] <= SaveData.instance.practiceModeLevelSettings.warpModeLevelTimes[i] && SaveData.instance.practiceModeLevelSettings.warpModeLevelTimesPlayer[i] != 0)
            {
                timeWarpButtons[i].bestPlayerTime.color = Color.green;
            }
            else if (SaveData.instance.practiceModeLevelSettings.warpModeLevelTimesPlayer[i] == 0)
            {
                timeWarpButtons[i].bestPlayerTime.color = Color.yellow;
            }
            else
            {
                timeWarpButtons[i].bestPlayerTime.color = Color.red;
            }
        }
    }


    public void TimewarpSaveVariables(string levelToLoad)
    {
        SaveData.instance.playerSettingsConfig.playerInTimeWarpMode = true;
        SaveData.instance.SaveIntoJson();
        PlayerSaveData presetDataForWarp = SaveData.instance.playerSaveData;
        //Here we edit the playerDatafile to give the player the items they should have on the specific level chosen
        //Reference for ability order.
        /*canWallClimb = saveData.currentAbilities[0];
        canDash = saveData.currentAbilities[1];
        canGroundPound = saveData.currentAbilities[2];
        canDoubleJump = saveData.currentAbilities[3];
        canWallJump = saveData.currentAbilities[4];
        canEnterRageMode = saveData.currentAbilities[5];
        canPhaseShift = saveData.currentAbilities[6];*/
        switch (levelToLoad) 
        {
            case "TestLevel":
                {
                    presetDataForWarp.currentWeapon = PlayerWeaponType.Dagger;
                    presetDataForWarp.currentAbilities = new bool[7] {true,true,true,true,true,true,true };
                    presetDataForWarp.currentPlayerWeapons = new List<PlayerWeaponType> { PlayerWeaponType.None, PlayerWeaponType.Dagger };
                    break;
                }
            case "0Tutorial":
                {
                    presetDataForWarp.currentWeapon = PlayerWeaponType.None;
                    presetDataForWarp.currentAbilities = new bool[7] { false, false, false, false, false, false, false };
                    presetDataForWarp.currentPlayerWeapons = new List<PlayerWeaponType> { PlayerWeaponType.None };
                    break;
                }
            case "Labyrinth1":
                {
                    presetDataForWarp.currentWeapon = PlayerWeaponType.None;
                    presetDataForWarp.currentAbilities = new bool[7] { false, false, false, false, false, true, true };
                    presetDataForWarp.currentPlayerWeapons = new List<PlayerWeaponType> { PlayerWeaponType.None };
                    break;
                }
            case "RadioactiveCave":
                {
                    presetDataForWarp.currentWeapon = PlayerWeaponType.None;
                    presetDataForWarp.currentAbilities = new bool[7] { false, false, false, false, false, true, true };
                    presetDataForWarp.currentPlayerWeapons = new List<PlayerWeaponType> { PlayerWeaponType.None };
                    break;
                }
            case "Labyrinth2":
                {
                    presetDataForWarp.currentWeapon = PlayerWeaponType.Dagger;
                    presetDataForWarp.currentAbilities = new bool[7] { false, true, false, false, false, true, true };
                    presetDataForWarp.currentPlayerWeapons = new List<PlayerWeaponType> { PlayerWeaponType.None, PlayerWeaponType.Dagger };
                    break;
                }
            case "LabLevel":
                {
                    presetDataForWarp.currentWeapon = PlayerWeaponType.Dagger;
                    presetDataForWarp.currentAbilities = new bool[7] { false, true, false, false, false, true, true };
                    presetDataForWarp.currentPlayerWeapons = new List<PlayerWeaponType> { PlayerWeaponType.None, PlayerWeaponType.Dagger };
                    break;
                }
            case "Labyrinth3":
                {
                    presetDataForWarp.currentWeapon = PlayerWeaponType.LaserGun;
                    presetDataForWarp.currentAbilities = new bool[7] { true, true, false, false, true, true, true };
                    presetDataForWarp.currentPlayerWeapons = new List<PlayerWeaponType> { PlayerWeaponType.None, PlayerWeaponType.Dagger, PlayerWeaponType.LaserGun };
                    break;
                }
            case "Surface":
                {
                    presetDataForWarp.currentWeapon = PlayerWeaponType.LaserGun;
                    presetDataForWarp.currentAbilities = new bool[7] { true, true, false, false, true, true, true };
                    presetDataForWarp.currentPlayerWeapons = new List<PlayerWeaponType> { PlayerWeaponType.None, PlayerWeaponType.Dagger, PlayerWeaponType.LaserGun };
                    break;
                }
            case "FinalBossTest":
                {
                    presetDataForWarp.currentWeapon = PlayerWeaponType.ChainWhip;
                    presetDataForWarp.currentAbilities = new bool[7] { true, true, true, true, true, true, true };
                    presetDataForWarp.currentPlayerWeapons = new List<PlayerWeaponType> { PlayerWeaponType.None, PlayerWeaponType.Dagger, PlayerWeaponType.LaserGun, PlayerWeaponType.ChainWhip };
                    break;
                }

        }
        //We save the data into the player save data so it can be temporarily stored. //DO NOT SAVE THIS OR PLAYER PROGRESSION WILL BE DESTROYED.
        SaveData.instance.playerSaveData = presetDataForWarp;
    }
}


