using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RunsDataManager : MonoBehaviour
{
    [SerializeField] RunsDataContainer playerBestRun;
    [SerializeField] RunsDataContainer[] runsDataHolders;
    public float[] gameAvgTimes { get; private set; } =  new float[9]
    {307f, 344f, 613f, 398f, 394f, 436f, 498f, 511f, 3535f};

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
                //playerBestRun.levelImages[i].color = Color.green;
                playerBestRun.levelSpeeds[i].outlineColor = Color.green;
            }
            else if (playerData.playerBestRun[i] == 0)
            {
                //playerBestRun.levelImages[i].color = Color.yellow;
                playerBestRun.levelSpeeds[i].outlineColor = Color.yellow;
                playerBestRun.levelSpeeds[i].text = "     -";
            }
            //Else we set it to red
            else
            {
                playerBestRun.levelSpeeds[i].outlineColor = Color.red;
                //playerBestRun.levelImages[i].color = Color.red;
            }
        }

        if (bestRunTotal > 0)
        {
            TimeSpan totalTimeT = TimeSpan.FromSeconds(bestRunTotal);
            playerBestRun.totalTime.text = totalTimeT.ToString("hh") + ":" + totalTimeT.ToString("mm") + ":" + totalTimeT.ToString("ss");
            if (bestRunTotal < gameAvgTimes[8])
            {
                playerBestRun.totalTime.outlineColor = Color.green;
            }
            //Else we set it to red
            else
            {
                playerBestRun.totalTime.outlineColor = Color.red;
            }
        }
        else
        {
            playerBestRun.totalTime.text = "     -";
            playerBestRun.totalTime.outlineColor = Color.yellow;
        }

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
                //runsDataHolders[0].levelImages[i].color = Color.green;
                runsDataHolders[0].levelSpeeds[i].outlineColor = Color.green;
            }
            else if (playerData.playerCurrentRuns[0][i] == 0)
            {
                //runsDataHolders[0].levelImages[i].color = Color.yellow;
                runsDataHolders[0].levelSpeeds[i].outlineColor = Color.yellow;
                runsDataHolders[0].levelSpeeds[i].text = "     -";
            }
            //Else we set it to red
            else
            {
                runsDataHolders[0].levelSpeeds[i].outlineColor = Color.red;
                //runsDataHolders[0].levelImages[i].color = Color.red;
            }

        }



        //COLOR THIS TEXT WITH AVERAGES FOUND.
        if (totalCurrentRunTime > 0)
        {
            TimeSpan totalTimeT = TimeSpan.FromSeconds(totalCurrentRunTime);
            runsDataHolders[0].totalTime.text = totalTimeT.ToString("hh") + ":" + totalTimeT.ToString("mm") + ":" + totalTimeT.ToString("ss");

            if (totalCurrentRunTime < gameAvgTimes[8])
            {
                runsDataHolders[0].totalTime.outlineColor = Color.green;
            }
            //Else we set it to red
            else
            {
                runsDataHolders[0].totalTime.outlineColor = Color.red;
            }
        }
        else
        {
            runsDataHolders[0].totalTime.text = "     -";
            runsDataHolders[0].totalTime.outlineColor = Color.yellow;
        }
    }




    void PresentPlayerData()
    {
        float bestRunTimeTotal = 0;
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
                //playerBestRun.levelImages[i].color = Color.green;
                playerBestRun.levelSpeeds[i].outlineColor = Color.green;
            }
            else if (playerData.playerBestRun[i] == 0)
            {
                //playerBestRun.levelImages[i].color = Color.yellow;
                playerBestRun.levelSpeeds[i].outlineColor = Color.yellow;
                playerBestRun.levelSpeeds[i].text = "     -";
            }
            //Else we set it to red
            else
            {
                playerBestRun.levelSpeeds[i].outlineColor = Color.red;
                //playerBestRun.levelImages[i].color = Color.red;
            }
            bestRunTimeTotal += playerData.playerBestRun[i];
        }

        if (bestRunTimeTotal > 0)
        {
            TimeSpan totalTimeT = TimeSpan.FromSeconds(bestRunTimeTotal);
            playerBestRun.totalTime.text = totalTimeT.ToString("hh") + ":" + totalTimeT.ToString("mm") + ":" + totalTimeT.ToString("ss");
            if (bestRunTimeTotal < gameAvgTimes[8])
            {
                playerBestRun.totalTime.outlineColor = Color.green;
            }
            //Else we set it to red
            else
            {
                playerBestRun.totalTime.outlineColor = Color.red;
            }
        }
        else
        {
            playerBestRun.totalTime.text = "     -";
            playerBestRun.totalTime.outlineColor = Color.yellow;
        }


        //Other runs
        //Loop over jagged array
        for (int j = 0; j < playerData.playerCurrentRuns.Length; j++) 
        {
            runsDataHolders[j].runName.text = "Run: " + (j + 1);
            //If the run is in progress we show an identifier
            if (playerData.currentRunCount == j)
            {
                runsDataHolders[j].inProgressRunMarker.color = Color.white;
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
                    //runsDataHolders[j].levelImages[i].color = Color.green;
                    runsDataHolders[j].levelSpeeds[i].outlineColor = Color.green;
                }
                else if (playerData.playerCurrentRuns[j][i] == 0)
                {
                    //runsDataHolders[j].levelImages[i].color = Color.yellow;
                    runsDataHolders[j].levelSpeeds[i].outlineColor = Color.yellow;
                    runsDataHolders[j].levelSpeeds[i].text = "     -";
                }
                //Else we set it to red
                else
                {
                    //runsDataHolders[j].levelImages[i].color = Color.red;
                    runsDataHolders[j].levelSpeeds[i].outlineColor = Color.red;
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


                if (runData.runTotalTime > 0)
                {
                    TimeSpan totalTimeT = TimeSpan.FromSeconds(runData.runTotalTime);
                    runsDataHolders[j].totalTime.text = totalTimeT.ToString("hh") + ":" + totalTimeT.ToString("mm") + ":" + totalTimeT.ToString("ss");
                    if (runData.runTotalTime < gameAvgTimes[8])
                    {
                        runsDataHolders[j].totalTime.outlineColor = Color.green;
                    }
                    //Else we set it to red
                    else
                    {
                        runsDataHolders[j].totalTime.outlineColor = Color.red;
                    }
                }
                else
                {
                    runsDataHolders[j].totalTime.text = "     -";
                    runsDataHolders[j].totalTime.outlineColor = Color.yellow;
                }
                Debug.Log(Application.persistentDataPath);
            }
        }
       
        //TimeSpan.FromSeconds(elapsedTime);
    }
}
