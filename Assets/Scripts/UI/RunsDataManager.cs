using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RunsDataManager : MonoBehaviour
{
    [SerializeField] RunsDataContainer playerBestRun;
    [SerializeField] RunsDataContainer[] runsDataHolders;
    [SerializeField] float[] gameAvgTimes;

    private PlayerSaveData playerData;
    

    // Start is called before the first frame update
    void Start()
    {
        if(SaveData.instance != null)
        {
            SaveData.instance.LoadFromJson();
            playerData = SaveData.instance.playerSaveData; 
            if(runsDataHolders.Length == 1) PresentDataLoadingScreen();
            else PresentPlayerData();
        }
    }

    public void UpdateTimes()
    {
        if (SaveData.instance != null)
        {
            playerData = SaveData.instance.playerSaveData;
            if (runsDataHolders.Length == 1) PresentDataLoadingScreen();
            else PresentPlayerData();
        }
    }

    void PresentDataLoadingScreen()
    {
        //Best run
        //Loop over the recorded level completion speeds
        for (int i = 0; i < playerBestRun.levelSpeeds.Length; i++)
        {
            //Convert each float to a timespan and display it
            TimeSpan ts = TimeSpan.FromSeconds(playerData.playerBestRun[i]);
            playerBestRun.levelSpeeds[i].text = ts.ToString("mm") + ":" + ts.ToString("ss") + ":" + ts.ToString("ff");

            //If the player is under the avg time we set the background behind the text to green
            if (playerData.playerBestRun[i] < gameAvgTimes[i] && playerData.playerBestRun[i] != 0)
            {
                playerBestRun.levelImages[i].color = Color.green;
            }
            else if (playerData.playerBestRun[i] == 0)
            {
                playerBestRun.levelImages[i].color = Color.yellow;
            }
            //Else we set it to red
            else
            {
                playerBestRun.levelImages[i].color = Color.red;
            }
        }
        //Set the current run name
        runsDataHolders[0].runName.text = "Run: " + (SaveData.instance.playerSaveData.currentRunCount + 1);
        //Present data from the current player run
        for (int i = 0; i < runsDataHolders[0].levelSpeeds.Length; i++)
        {
            //Convert each float to a timespan and display it
            TimeSpan ts = TimeSpan.FromSeconds(playerData.playerCurrentRuns[SaveData.instance.playerSaveData.currentRunCount][i]);
            runsDataHolders[SaveData.instance.playerSaveData.currentRunCount].levelSpeeds[i].text = ts.ToString("mm") + ":" + ts.ToString("ss") + ":" + ts.ToString("ff");
            //If the player is under the avg time we set the background behind the text to green
            if (playerData.playerCurrentRuns[SaveData.instance.playerSaveData.currentRunCount][i] <= gameAvgTimes[i] && playerData.playerCurrentRuns[SaveData.instance.playerSaveData.currentRunCount][i] != 0)
            {
                runsDataHolders[SaveData.instance.playerSaveData.currentRunCount].levelImages[i].color = Color.green;
            }
            else if (playerData.playerCurrentRuns[SaveData.instance.playerSaveData.currentRunCount][i] == 0)
            {
                runsDataHolders[SaveData.instance.playerSaveData.currentRunCount].levelImages[i].color = Color.yellow;
            }
            //Else we set it to red
            else
            {
                runsDataHolders[SaveData.instance.playerSaveData.currentRunCount].levelImages[i].color = Color.red;
            }
        }
    }




    void PresentPlayerData()
    {
        //Best run
        //Loop over the recorded level completion speeds
        for(int i = 0; i < playerBestRun.levelSpeeds.Length; i++)
        {
            //Convert each float to a timespan and display it
            TimeSpan ts = TimeSpan.FromSeconds(playerData.playerBestRun[i]);
            playerBestRun.levelSpeeds[i].text = ts.ToString("mm") + ":" + ts.ToString("ss") + ":" + ts.ToString("ff");

            //If the player is under the avg time we set the background behind the text to green
            if (playerData.playerBestRun[i] < gameAvgTimes[i] && playerData.playerBestRun[i] != 0)
            {
                playerBestRun.levelImages[i].color = Color.green;
            }
            else if (playerData.playerBestRun[i] == 0)
            {
                playerBestRun.levelImages[i].color = Color.yellow;
            }
            //Else we set it to red
            else
            {
                playerBestRun.levelImages[i].color = Color.red;
            }
        }
        
        //Other runs
        //Loop over jagged array
        for(int j = 0; j < playerData.playerCurrentRuns.Length; j++) 
        {
            //If the run is in progress we show an identifier
            if (playerData.currentRunCount == j)
            {
                runsDataHolders[j].inProgressRunMarker.color = Color.cyan;
            }
            else runsDataHolders[j].inProgressRunMarker.color = Color.clear;
            //Loop over the recorded level completion speeds
            for (int i = 0; i < runsDataHolders[j].levelSpeeds.Length; i++)
            {
                //Convert each float to a timespan and display it
                TimeSpan ts = TimeSpan.FromSeconds(playerData.playerCurrentRuns[j][i]);
                Debug.Log(playerData.playerCurrentRuns[j][i]);
                runsDataHolders[j].levelSpeeds[i].text = ts.ToString("mm") + ":" + ts.ToString("ss") + ":" + ts.ToString("ff");
                //If the player is under the avg time we set the background behind the text to green
                if (playerData.playerCurrentRuns[j][i] <= gameAvgTimes[i] && playerData.playerCurrentRuns[j][i] != 0)
                {
                    runsDataHolders[j].levelImages[i].color = Color.green;
                }
                else if (playerData.playerCurrentRuns[j][i] == 0)
                {
                    runsDataHolders[j].levelImages[i].color = Color.yellow;
                }
                //Else we set it to red
                else
                {
                    runsDataHolders[j].levelImages[i].color = Color.red;
                }
            }
        }
       
        //TimeSpan.FromSeconds(elapsedTime);
    }
}
