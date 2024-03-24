using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            //Grab players best time from that level. If this number is zero, we attempt to load the players best run from that stage.
            if (SaveData.instance.practiceModeLevelSettings.warpModeLevelTimesPlayer[i] != 0) timeWarpButtons[i].bestPlayerTime.text = System.TimeSpan.FromSeconds(SaveData.instance.practiceModeLevelSettings.warpModeLevelTimesPlayer[i]).ToString();
            else timeWarpButtons[i].bestPlayerTime.text = "Best Time: " + System.TimeSpan.FromSeconds(SaveData.instance.playerSaveData.playerBestRun[i]).ToString();
            //Load the dev times
            timeWarpButtons[i].devTime.text = "Dev Time: " + System.TimeSpan.FromSeconds(SaveData.instance.practiceModeLevelSettings.warpModeLevelTimes[i]).ToString();
            //Load collectibles data. If its zero we attempt to pull from previously saved data.
            if (SaveData.instance.practiceModeLevelSettings.warpModeCollectibleCountPlayer[i] != 0) timeWarpButtons[i].collectibleCount.text = "Collectibles: " + SaveData.instance.practiceModeLevelSettings.warpModeCollectibleCountPlayer[i] + "/" + SaveData.instance.practiceModeLevelSettings.warpModeCollectibleCount[i];
            else timeWarpButtons[i].collectibleCount.text = "Collectibles: " + SaveData.instance.playerSaveData.playerCollectiblesTracker[i] + "/" + SaveData.instance.practiceModeLevelSettings.warpModeCollectibleCount[i];

            //Color the text if the player is a above or below dev time
            if (SaveData.instance.practiceModeLevelSettings.warpModeLevelTimesPlayer[i] <= SaveData.instance.practiceModeLevelSettings.warpModeLevelTimes[i])
            {
                timeWarpButtons[i].bestPlayerTime.color = Color.green;
            }
            else
            {
                timeWarpButtons[i].bestPlayerTime.color = Color.red;
            }
        }
    }
}
