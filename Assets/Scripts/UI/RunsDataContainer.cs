using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RunsDataContainer : MonoBehaviour
{
    public TextMeshProUGUI[] levelSpeeds;
    public Image[] levelImages;
    public Image inProgressRunMarker;
    public TextMeshProUGUI runName;
    [SerializeField] private RunDataSet runDataSet;
    public Button runsButton;


    public void ConvertDataToRunDataset(float[] playerRunTimes, string runName, int runIndex)
    {
        if (playerRunTimes.Length == 8)
        {
            runDataSet.playerRunTimes = playerRunTimes;
            runDataSet.runName = runName;
            runDataSet.runIndex = runIndex;
            //Add each time into the total time, so this data can be easily sorted.
            float temp = 0;
            for (int i = 0; i < playerRunTimes.Length; i++) 
            {
                temp += playerRunTimes[i];
            }
            runDataSet.runTotalTime = temp;
        }
    }

    public RunDataSet GetRunDataSet() { return runDataSet; }

    public void SetRunDataSet(RunDataSet runDataSetIn) 
    { runDataSet = runDataSetIn; }

    public void UpdateRunDisplay()
    {
        runName.text = runDataSet.runName;  

        for (int i = 0; i < levelSpeeds.Length; i++)
        {
            if(runDataSet.playerRunTimes[i] == 0)
            {
                levelSpeeds[i].text = "     -";
            }
            else
            {
                TimeSpan ts = TimeSpan.FromSeconds(runDataSet.playerRunTimes[i]);
                levelSpeeds[i].text = ts.ToString("mm") + ":" + ts.ToString("ss") + ":" + ts.ToString("ff");
            }
            
        }
    }


}

