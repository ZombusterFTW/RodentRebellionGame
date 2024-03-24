using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MatchTimer : MonoBehaviour
{
    public TextMeshProUGUI timerText1;
    public TextMeshProUGUI timerText2;
    public TextMeshProUGUI timerText3;
    public TextMeshProUGUI levelName;
    public GameObject matchTimerParent;
    private float elapsedTime;
    DialogueManager dialogueManager;

    // Start is called before the first frame update
    void Start()
    {
        dialogueManager = DialogueManager.GetInstance();
        SceneManager.sceneLoaded += OnSceneLoaded;
        levelName.text = SceneManager.GetActiveScene().name;
        if (SaveData.instance.playerSettingsConfig.isSpeedrunModeEnabled || SaveData.instance.playerSettingsConfig.playerInTimeWarpMode)
        {
            matchTimerParent.SetActive(true);
        }
        else
        {
            matchTimerParent.SetActive(false);
        }
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if(this != null)
        {
            elapsedTime = 0;
            //Set level name to the new scene
            levelName.text = arg0.name;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        //Time will no longer accrue on the timer during dialouge
        if (!GameObject.ReferenceEquals(dialogueManager, null) && !dialogueManager.dialogueIsPlaying)
        {
            elapsedTime += Time.deltaTime;
            if(SaveData.instance.playerSettingsConfig.isSpeedrunModeEnabled || SaveData.instance.playerSettingsConfig.playerInTimeWarpMode)
            {
                UpdateTimer();
            }
        }
    }


    private void UpdateTimer()
    {
        var ts = TimeSpan.FromSeconds(elapsedTime);
        timerText1.text = "<mspace=0.8em>" + ts.ToString("mm");
        timerText2.text = "<mspace=0.8em>" + ts.ToString("ss");
        timerText3.text = "<mspace=0.8em>" + ts.ToString("ff");
    }

    public void LapTime()
    {


        if (SaveData.instance != null)
        {
            //Get level scene name. Save time
            //currentRunCount will increment when the player presses new game.
            int currentRunCount = SaveData.instance.playerSaveData.currentRunCount;

            //Need to set current level to nothing on completion of the final boss
            switch (SceneManager.GetActiveScene().name)
            {
                case "TestLevel":
                    {
                        SaveData.instance.playerSaveData.playerCurrentRuns[currentRunCount][0] = elapsedTime;
                        //SaveData.instance.playerSaveData.playerBestRun[0] = elapsedTime;
                        CalculateBestRun();
                        break;
                    }
                case "0Tutorial":
                    {
                        SaveData.instance.playerSaveData.playerCurrentRuns[currentRunCount][0] = elapsedTime;
                        //SaveData.instance.playerSaveData.playerBestRun[0] = elapsedTime;
                        break;
                    }
                case "Labyrinth1":
                    {
                        SaveData.instance.playerSaveData.playerCurrentRuns[currentRunCount][1] = elapsedTime;
                        //SaveData.instance.playerSaveData.playerBestRun[1] = elapsedTime;
                        break;
                    }
                case "RadioactiveCave":
                    {
                        SaveData.instance.playerSaveData.playerCurrentRuns[currentRunCount][2] = elapsedTime;
                        //SaveData.instance.playerSaveData.playerBestRun[2] = elapsedTime;
                        break;
                    }
                case "Labyrinth2":
                    {
                        SaveData.instance.playerSaveData.playerCurrentRuns[currentRunCount][3] = elapsedTime;
                        //SaveData.instance.playerSaveData.playerBestRun[3] = elapsedTime;
                        break;
                    }
                case "LabLevel":
                    {
                        SaveData.instance.playerSaveData.playerCurrentRuns[currentRunCount][4] = elapsedTime;
                        //SaveData.instance.playerSaveData.playerBestRun[4] = elapsedTime;
                        break;
                    }
                case "Labyrinth3":
                    {
                        SaveData.instance.playerSaveData.playerCurrentRuns[currentRunCount][5] = elapsedTime;
                        //SaveData.instance.playerSaveData.playerBestRun[5] = elapsedTime;
                        break;
                    }
                case "Surface":
                    {
                        SaveData.instance.playerSaveData.playerCurrentRuns[currentRunCount][6] = elapsedTime;
                        //SaveData.instance.playerSaveData.playerBestRun[6] = elapsedTime;
                        break;
                    }
                case "FinalBossTest":
                    {
                        SaveData.instance.playerSaveData.playerCurrentRuns[currentRunCount][7] = elapsedTime;
                        //SaveData.instance.playerSaveData.playerBestRun[7] = elapsedTime;
                        //The game has officially ended so we check if the player got their new best completion time.
                        CalculateBestRun();
                        break;
                    }

            }
            SaveData.instance.SaveIntoJson();
            Debug.Log("Saved Lap: " + SceneManager.GetActiveScene().name);
            Debug.Log(TimeSpan.FromSeconds(elapsedTime));
            if (SceneTransitionerManager.instance != null)
            {
                SceneTransitionerManager.instance.runsShowcase.UpdateTimes();
            }
        }
    }


    void CalculateBestRun()
    {
        float currentRunTimeSummed = 0;
        float bestTimeRunSummed = 0;
        //This function runs after the final boss is laped. It sums the current best run and ensures that it is greater than 0. If the current run's sum is less than this value then we save it as the new best run.
        for(int i = 0; i < SaveData.instance.playerSaveData.playerCurrentRuns[SaveData.instance.playerSaveData.currentRunCount].Length; i++) 
        {
            //Grab each float from the array and sum it into currentRunTimeSummed
            currentRunTimeSummed += SaveData.instance.playerSaveData.playerCurrentRuns[SaveData.instance.playerSaveData.currentRunCount][i];
            bestTimeRunSummed += SaveData.instance.playerSaveData.playerBestRun[i];

            //If the best time is zero we just replace it. Other wise the besttime is replaced when the summed value is less than the stored one
            if(currentRunTimeSummed < bestTimeRunSummed || bestTimeRunSummed == 0) 
            {
                //If both of these statements are true, then we save this current run as the new best run.
                SaveData.instance.playerSaveData.playerBestRun = SaveData.instance.playerSaveData.playerCurrentRuns[SaveData.instance.playerSaveData.currentRunCount];
            }

        }
        SaveData.instance.SaveIntoJson();
    }
}
