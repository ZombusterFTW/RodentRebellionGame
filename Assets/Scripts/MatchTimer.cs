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
    private float elapsedTime;
    DialogueManager dialogueManager;

    // Start is called before the first frame update
    void Start()
    {
        dialogueManager = DialogueManager.GetInstance();
        SceneManager.sceneLoaded += OnSceneLoaded;
        levelName.text = SceneManager.GetActiveScene().name;
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if(this != null)
        {
            //Save time
            //We enter a new scene so we reset the time.
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
            UpdateTimer();
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
        //Get level scene name. Save time
        //currentRunCount will increment when the player presses new game.
        int currentRunCount = SaveData.instance.playerSaveData.currentRunCount;

        //Need to set current level to nothing on completion of the final boss
        switch(SceneManager.GetActiveScene().name)
        {
            case "TestLevel":
                {
                    SaveData.instance.playerSaveData.playerCurrentRuns[currentRunCount][0] = elapsedTime;
                    SaveData.instance.playerSaveData.playerBestRun[0] = elapsedTime;
                    break;
                }
            case "0Tutorial":
                {
                    SaveData.instance.playerSaveData.playerCurrentRuns[currentRunCount][0] = elapsedTime;
                    SaveData.instance.playerSaveData.playerBestRun[0] = elapsedTime;
                    break;
                }
            case "Labyrinth1":
                {
                    SaveData.instance.playerSaveData.playerCurrentRuns[currentRunCount][1] = elapsedTime;
                    SaveData.instance.playerSaveData.playerBestRun[1] = elapsedTime;
                    break;
                }
            case "RadioactiveCave":
                {
                    SaveData.instance.playerSaveData.playerCurrentRuns[currentRunCount][2] = elapsedTime;
                    SaveData.instance.playerSaveData.playerBestRun[2] = elapsedTime;
                    break;
                }
            case "Labyrinth2":
                {
                    SaveData.instance.playerSaveData.playerCurrentRuns[currentRunCount][3] = elapsedTime;
                    SaveData.instance.playerSaveData.playerBestRun[3] = elapsedTime;
                    break;
                }
            case "LabLevel":
                {
                    SaveData.instance.playerSaveData.playerCurrentRuns[currentRunCount][4] = elapsedTime;
                    SaveData.instance.playerSaveData.playerBestRun[4] = elapsedTime;
                    break;
                }
            case "Labyrinth3":
                {
                    SaveData.instance.playerSaveData.playerCurrentRuns[currentRunCount][5] = elapsedTime;
                    SaveData.instance.playerSaveData.playerBestRun[5] = elapsedTime;
                    break;
                }
            case "Surface":
                {
                    SaveData.instance.playerSaveData.playerCurrentRuns[currentRunCount][6] = elapsedTime;
                    SaveData.instance.playerSaveData.playerBestRun[6] = elapsedTime;
                    break;
                }
            case "FinalBossTest":
                {
                    SaveData.instance.playerSaveData.playerCurrentRuns[currentRunCount][7] = elapsedTime;
                    SaveData.instance.playerSaveData.playerBestRun[7] = elapsedTime;
                    break;
                }

        }
        Debug.Log("Saved Lap");
        Debug.Log(TimeSpan.FromSeconds(elapsedTime));
    }
}
