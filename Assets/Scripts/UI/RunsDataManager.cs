using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RunsDataManager : MonoBehaviour
{
    [SerializeField] RunsDataContainer playerBestRun;
    [SerializeField] RunsDataContainer[] runsDataHolders;
    public float[] gameAvgTimes;

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
        float bestRunTotal = 0;
        //Best run
        //Loop over the recorded level completion speeds
        for (int i = 0; i < playerBestRun.levelSpeeds.Length; i++)
        {
            //Convert each float to a timespan and display it
            bestRunTotal += playerData.playerBestRun[i];
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
                playerBestRun.levelSpeeds[i].text = "     -";
            }
            //Else we set it to red
            else
            {
                playerBestRun.levelImages[i].color = Color.red;
            }
        }
        
        if (bestRunTotal > 0)
        {
            TimeSpan totalTimeT = TimeSpan.FromSeconds(bestRunTotal);
            playerBestRun.totalTime.text = totalTimeT.ToString("mm") + ":" + totalTimeT.ToString("ss") + ":" + totalTimeT.ToString("ff");
        }
        else playerBestRun.totalTime.text = "     -";

        //Set the current run name
        runsDataHolders[0].runName.text = "Run: " + (SaveData.instance.playerSaveData.currentRunCount + 1);
        float totalCurrentRunTime = 0;
        //Present data from the current player run
        for (int i = 0; i < runsDataHolders[0].levelSpeeds.Length; i++)
        {
            //Convert each float to a timespan and display it
            TimeSpan ts = TimeSpan.FromSeconds(playerData.playerCurrentRuns[SaveData.instance.playerSaveData.currentRunCount][i]);
            totalCurrentRunTime += playerData.playerCurrentRuns[SaveData.instance.playerSaveData.currentRunCount][i];
            //RunDataSet runData = runsDataHolders[i].ConvertDataToRunDataset(playerData.playerCurrentRuns[SaveData.instance.playerSaveData.currentRunCount][i], "Run: " + (i + 1), i);

            runsDataHolders[0].levelSpeeds[i].text = ts.ToString("mm") + ":" + ts.ToString("ss") + ":" + ts.ToString("ff");
            //If the player is under the avg time we set the background behind the text to green
            if (playerData.playerCurrentRuns[0][i] <= gameAvgTimes[i] && playerData.playerCurrentRuns[SaveData.instance.playerSaveData.currentRunCount][i] != 0)
            {
                runsDataHolders[0].levelImages[i].color = Color.green;
            }
            else if (playerData.playerCurrentRuns[0][i] == 0)
            {
                runsDataHolders[0].levelImages[i].color = Color.yellow;
                runsDataHolders[0].levelSpeeds[i].text = "     -";
            }
            //Else we set it to red
            else
            {
                runsDataHolders[0].levelImages[i].color = Color.red;
            }
            
        }
        if (totalCurrentRunTime > 0)
        {
            TimeSpan totalTimeT = TimeSpan.FromSeconds(totalCurrentRunTime);
            runsDataHolders[0].totalTime.text = totalTimeT.ToString("mm") + ":" + totalTimeT.ToString("ss") + ":" + totalTimeT.ToString("ff");
        }
        else runsDataHolders[0].totalTime.text = "     -";
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
                playerBestRun.levelSpeeds[i].text = "     -";
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
            runsDataHolders[j].runName.text = "Run: " + (j + 1);
            //If the run is in progress we show an identifier
            if (playerData.currentRunCount == j)
            {
                runsDataHolders[j].inProgressRunMarker.color = Color.cyan;
            }
            else runsDataHolders[j].inProgressRunMarker.color = Color.clear;
            //Loop over the recorded level completion speeds
            for (int i = 0; i < runsDataHolders[j].levelSpeeds.Length; i++)
            {
                //Convert data to object for easy saving of it later.
                RunDataSet runData = runsDataHolders[j].ConvertDataToRunDataset(playerData.playerCurrentRuns[j], "Run: " + (j + 1), j);
                //Convert each float to a timespan and display it
                TimeSpan ts = TimeSpan.FromSeconds(playerData.playerCurrentRuns[j][i]);
                //Debug.Log(playerData.playerCurrentRuns[j][i]);
                runsDataHolders[j].levelSpeeds[i].text = ts.ToString("mm") + ":" + ts.ToString("ss") + ":" + ts.ToString("ff");
                //If the player is under the avg time we set the background behind the text to green
                if (playerData.playerCurrentRuns[j][i] <= gameAvgTimes[i] && playerData.playerCurrentRuns[j][i] != 0)
                {
                    runsDataHolders[j].levelImages[i].color = Color.green;
                }
                else if (playerData.playerCurrentRuns[j][i] == 0)
                {
                    runsDataHolders[j].levelImages[i].color = Color.yellow;
                    runsDataHolders[j].levelSpeeds[i].text = "     -";
                }
                //Else we set it to red
                else
                {
                    runsDataHolders[j].levelImages[i].color = Color.red;
                }

                //Enable button if we have data that suggests the player has completed atleast level 1. Can add logic here to only allow a save where the player completed the game to be saved.
                if(runsDataHolders[j].runsButton != null)
                {
                    if (playerData.playerCurrentRuns[j][0] > 0 && playerData.playerCurrentRuns[j][7] > 0)
                    {
                        runsDataHolders[j].runsButton.gameObject.SetActive(true);
                    }
                    else runsDataHolders[j].runsButton.gameObject.SetActive(false);
                }
                if(runData.runTotalTime > 0)
                {
                    TimeSpan totalTimeT = TimeSpan.FromSeconds(runData.runTotalTime);
                    runsDataHolders[j].totalTime.text = totalTimeT.ToString("mm") + ":" + totalTimeT.ToString("ss") + ":" + totalTimeT.ToString("ff");
                }
                else runsDataHolders[j].totalTime.text = "     -";
                Debug.Log(Application.persistentDataPath);
            }
        }
       
        //TimeSpan.FromSeconds(elapsedTime);
    }
}
