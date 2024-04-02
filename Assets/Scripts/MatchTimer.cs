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
    Coroutine timeSaver = null;

    // Start is called before the first frame update
    void Start()
    {
        dialogueManager = DialogueManager.GetInstance();
        SceneManager.sceneLoaded += OnSceneLoaded;
        levelName.text = GetLevelNamePretty(SceneManager.GetActiveScene().name);
        if (SaveData.instance.playerSettingsConfig.isSpeedrunModeEnabled || SaveData.instance.playerSettingsConfig.playerInTimeWarpMode)
        {
            //time exploit fix commented for now
            //elapsedTime = SaveData.instance.practiceModeLevelSettings.lastLevelAccruedTime;
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
            levelName.text = GetLevelNamePretty(arg0.name);
            //time exploit fix commented for now
            //if (SaveData.instance.playerSettingsConfig.isSpeedrunModeEnabled) elapsedTime += SaveData.instance.practiceModeLevelSettings.lastLevelAccruedTime;
        }

    }


    private string GetLevelNamePretty(string inLevelName)
    {
        switch (inLevelName)
        {
            default:
                return "Error :(";
            case "0Tutorial":
                return "Tutorial";
            case "Labyrinth1":
                return "Labyrinth α";
            case "Labyrinth2":
                return "Labyrinth δ";
            case "Labyrinth3":
                return "Labyrinth Ω";
            case "RadioactiveCave":
                return "Radioactive Cave";
            case "LabLevel":
                return "The Labs";
            case "FinalBossTest":
                return "Final Boss";
            case "Surface":
                return "The Surface";
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
                /* Commented this code would theoretically prevent the player from resuming a level and restart their times.
                if(!SaveData.instance.playerSettingsConfig.playerInTimeWarpMode)
                {
                    if(timeSaver == null) timeSaver = StartCoroutine(SaveTimeInterval());
                }
                */
                UpdateTimer();
            }
        }
    }

    IEnumerator SaveTimeInterval()
    {
        while(true)
        {
            yield return new WaitForSecondsRealtime(1);
            SaveData.instance.practiceModeLevelSettings.lastLevelAccruedTime = elapsedTime;
            SaveData.instance.SaveLevelTime();
            LapTime();   
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
        if(timeSaver != null) StopCoroutine(timeSaver);
        if (SaveData.instance != null)
        {
            //Get level scene name. Save time
            //currentRunCount will increment when the player presses new game.
            int currentRunCount = SaveData.instance.playerSaveData.currentRunCount;
            bool playerInTimeWarpMode = SaveData.instance.playerSettingsConfig.playerInTimeWarpMode;
            int currentCoinCount = PlayerController.instance.GetComponentInChildren<CoinCollectibleManager>().GetCurrentCoinCount();
            int totalCoinCount = PlayerController.instance.GetComponentInChildren<CoinCollectibleManager>().GetTotalCoinCount();

            //Need to set current level to nothing on completion of the final boss
            switch (SceneManager.GetActiveScene().name)
            {
                case "TestLevel":
                    {
                        //Update the counter for collectibles per level.
                        SaveData.instance.practiceModeLevelSettings.warpModeCollectibleCount[0] = totalCoinCount;
                        if (playerInTimeWarpMode)
                        {
                            if (SaveData.instance.practiceModeLevelSettings.warpModeLevelTimesPlayer[0] == 0 || SaveData.instance.practiceModeLevelSettings.warpModeLevelTimesPlayer[0] > elapsedTime) SaveData.instance.practiceModeLevelSettings.warpModeLevelTimesPlayer[0] = elapsedTime;
                            SaveData.instance.practiceModeLevelSettings.warpModeCollectibleCountPlayer[0] = currentCoinCount;
                        }
                        else
                        {
                            SaveData.instance.playerSaveData.playerCurrentRuns[currentRunCount][0] = elapsedTime;
                            SaveData.instance.playerSaveData.playerCollectiblesTracker[0] = currentCoinCount;
                            //SaveData.instance.playerSaveData.playerBestRun[0] = elapsedTime;
                            CalculateBestRun();
                        }
                        break;
                    }
                case "0Tutorial":
                    {
                        SaveData.instance.practiceModeLevelSettings.warpModeCollectibleCount[0] = totalCoinCount;
                        if (playerInTimeWarpMode)
                        {
                            if (SaveData.instance.practiceModeLevelSettings.warpModeLevelTimesPlayer[0] == 0 || SaveData.instance.practiceModeLevelSettings.warpModeLevelTimesPlayer[0] > elapsedTime) SaveData.instance.practiceModeLevelSettings.warpModeLevelTimesPlayer[0] = elapsedTime;
                            SaveData.instance.practiceModeLevelSettings.warpModeCollectibleCountPlayer[0] = currentCoinCount;
                        }
                        else
                        {
                            SaveData.instance.playerSaveData.playerCurrentRuns[currentRunCount][0] = elapsedTime;
                            SaveData.instance.playerSaveData.playerCollectiblesTracker[0] = currentCoinCount;
                            //SaveData.instance.playerSaveData.playerBestRun[0] = elapsedTime;
                        }
                        break;
                    }
                case "Labyrinth1":
                    {
                        SaveData.instance.practiceModeLevelSettings.warpModeCollectibleCount[1] = totalCoinCount;
                        if (playerInTimeWarpMode)
                        {
                            if (SaveData.instance.practiceModeLevelSettings.warpModeLevelTimesPlayer[1] == 0 || SaveData.instance.practiceModeLevelSettings.warpModeLevelTimesPlayer[1] > elapsedTime) SaveData.instance.practiceModeLevelSettings.warpModeLevelTimesPlayer[1] = elapsedTime;
                            SaveData.instance.practiceModeLevelSettings.warpModeCollectibleCountPlayer[1] = currentCoinCount;
                        }
                        else
                        {
                            SaveData.instance.playerSaveData.playerCurrentRuns[currentRunCount][1] = elapsedTime;
                            SaveData.instance.playerSaveData.playerCollectiblesTracker[1] = currentCoinCount;
                            //SaveData.instance.playerSaveData.playerBestRun[1] = elapsedTime;
                        }
                        break;
                    }
                case "RadioactiveCave":
                    {
                        SaveData.instance.practiceModeLevelSettings.warpModeCollectibleCount[2] = totalCoinCount;
                        if (playerInTimeWarpMode)
                        {
                            if (SaveData.instance.practiceModeLevelSettings.warpModeLevelTimesPlayer[2] == 0 || SaveData.instance.practiceModeLevelSettings.warpModeLevelTimesPlayer[2] > elapsedTime) SaveData.instance.practiceModeLevelSettings.warpModeLevelTimesPlayer[2] = elapsedTime;
                            SaveData.instance.practiceModeLevelSettings.warpModeCollectibleCountPlayer[2] = currentCoinCount;
                        }
                        else
                        {
                            SaveData.instance.playerSaveData.playerCurrentRuns[currentRunCount][2] = elapsedTime;
                            SaveData.instance.playerSaveData.playerCollectiblesTracker[2] = currentCoinCount;
                            //SaveData.instance.playerSaveData.playerBestRun[2] = elapsedTime;
                        }
                        break;
                    }
                case "Labyrinth2":
                    {
                        SaveData.instance.practiceModeLevelSettings.warpModeCollectibleCount[3] = totalCoinCount;
                        if (playerInTimeWarpMode)
                        {
                            if (SaveData.instance.practiceModeLevelSettings.warpModeLevelTimesPlayer[3] == 0 || SaveData.instance.practiceModeLevelSettings.warpModeLevelTimesPlayer[3] > elapsedTime) SaveData.instance.practiceModeLevelSettings.warpModeLevelTimesPlayer[3] = elapsedTime;
                            SaveData.instance.practiceModeLevelSettings.warpModeCollectibleCountPlayer[3] = currentCoinCount;
                        }
                        else
                        {
                            SaveData.instance.playerSaveData.playerCurrentRuns[currentRunCount][3] = elapsedTime;
                            SaveData.instance.playerSaveData.playerCollectiblesTracker[3] = currentCoinCount;
                            //SaveData.instance.playerSaveData.playerBestRun[3] = elapsedTime;
                        }
                        break;
                    }
                case "LabLevel":
                    {
                        SaveData.instance.practiceModeLevelSettings.warpModeCollectibleCount[4] = totalCoinCount;
                        if (playerInTimeWarpMode)
                        {
                            if (SaveData.instance.practiceModeLevelSettings.warpModeLevelTimesPlayer[4] == 0 || SaveData.instance.practiceModeLevelSettings.warpModeLevelTimesPlayer[4] > elapsedTime) SaveData.instance.practiceModeLevelSettings.warpModeLevelTimesPlayer[4] = elapsedTime;
                            SaveData.instance.practiceModeLevelSettings.warpModeCollectibleCountPlayer[4] = currentCoinCount;
                        }
                        else
                        {
                            SaveData.instance.playerSaveData.playerCurrentRuns[currentRunCount][4] = elapsedTime;
                            SaveData.instance.playerSaveData.playerCollectiblesTracker[4] = currentCoinCount;
                            //SaveData.instance.playerSaveData.playerBestRun[4] = elapsedTime;
                        }
                        break;
                    }
                case "Labyrinth3":
                    {
                        SaveData.instance.practiceModeLevelSettings.warpModeCollectibleCount[5] = totalCoinCount;
                        if (playerInTimeWarpMode)
                        {
                            if (SaveData.instance.practiceModeLevelSettings.warpModeLevelTimesPlayer[5] == 0 || SaveData.instance.practiceModeLevelSettings.warpModeLevelTimesPlayer[5] > elapsedTime) SaveData.instance.practiceModeLevelSettings.warpModeLevelTimesPlayer[5] = elapsedTime;
                            SaveData.instance.practiceModeLevelSettings.warpModeCollectibleCountPlayer[5] = currentCoinCount;
                        }
                        else
                        {
                            SaveData.instance.playerSaveData.playerCurrentRuns[currentRunCount][5] = elapsedTime;
                            SaveData.instance.playerSaveData.playerCollectiblesTracker[5] = currentCoinCount;
                            //SaveData.instance.playerSaveData.playerBestRun[5] = elapsedTime;
                        }
                        break;
                    }
                case "Surface":
                    {
                        SaveData.instance.practiceModeLevelSettings.warpModeCollectibleCount[6] = totalCoinCount;
                        if (playerInTimeWarpMode)
                        {
                            if (SaveData.instance.practiceModeLevelSettings.warpModeLevelTimesPlayer[6] == 0 || SaveData.instance.practiceModeLevelSettings.warpModeLevelTimesPlayer[6] > elapsedTime) SaveData.instance.practiceModeLevelSettings.warpModeLevelTimesPlayer[6] = elapsedTime;
                            SaveData.instance.practiceModeLevelSettings.warpModeCollectibleCountPlayer[6] = currentCoinCount;
                        }
                        else
                        {
                            SaveData.instance.playerSaveData.playerCurrentRuns[currentRunCount][6] = elapsedTime;
                            SaveData.instance.playerSaveData.playerCollectiblesTracker[6] = currentCoinCount;
                            //SaveData.instance.playerSaveData.playerBestRun[6] = elapsedTime;
                        }
                        break;
                    }
                case "FinalBossTest":
                    {
                        SaveData.instance.practiceModeLevelSettings.warpModeCollectibleCount[7] = totalCoinCount;
                        if (playerInTimeWarpMode)
                        {
                            if(SaveData.instance.practiceModeLevelSettings.warpModeLevelTimesPlayer[7] == 0 || SaveData.instance.practiceModeLevelSettings.warpModeLevelTimesPlayer[7] > elapsedTime) SaveData.instance.practiceModeLevelSettings.warpModeLevelTimesPlayer[7] = elapsedTime;
                            SaveData.instance.practiceModeLevelSettings.warpModeCollectibleCountPlayer[7] = currentCoinCount;
                        }
                        else
                        {
                            SaveData.instance.playerSaveData.playerCurrentRuns[currentRunCount][7] = elapsedTime;
                            SaveData.instance.playerSaveData.playerCollectiblesTracker[7] = currentCoinCount;
                            //SaveData.instance.playerSaveData.playerBestRun[7] = elapsedTime;
                            //The game has officially ended so we check if the player got their new best completion time.
                            CalculateBestRun();
                        }
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
