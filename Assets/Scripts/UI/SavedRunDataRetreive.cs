using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SavedRunDataRetreive : MonoBehaviour
{
    [SerializeField] GameObject runDataPrefab;
    [SerializeField] GameObject viewPortParent;
    [SerializeField] RunsDataManager runsDataManager;
    [SerializeField] List<GameObject> savedRuns = new List<GameObject>();
    [SerializeField] TMPro.TMP_Dropdown sortingOptions;
    [SerializeField] SavedRunsSortingOptions currentSortingOption = SavedRunsSortingOptions.BestRun;

    private void Start()
    {
        UpdateSavedRunDisplay();
    }

    IEnumerator RunsWaitForFrame()
    {
        yield return null;  
        foreach (GameObject run in savedRuns)
        {
            Destroy(run);
        }
        savedRuns.Clear();
        SaveData.instance.LoadFromJson();
        List<RunDataSet> runs = new List<RunDataSet>();
        runs = SaveData.instance.playerSaveData.playerSavedRuns;
        //Sort list by the defined variable
        runs = UpdateSortedList(runs);
        //variable to track current level speed.
        if (runs.Count == 0) yield break;
        int i = 0;
        foreach (RunDataSet runData in runs)
        {
            GameObject runDataContainer = Instantiate(runDataPrefab, viewPortParent.transform);
            savedRuns.Add(runDataContainer);
            RunsDataContainer runDataRef = runDataContainer.GetComponent<RunsDataContainer>();
            runDataRef.SetRunDataSet(runData);
            runDataRef.UpdateRunDisplay();
            for (int j = 0; j < runs[i].playerRunTimes.Length; j++)
            {
                //If the player is under the avg time we set the background behind the text to green
                if (runs[i].playerRunTimes[j] < runsDataManager.gameAvgTimes[j] && runs[i].playerRunTimes[j] != 0)
                {
                    //runDataRef.levelImages[j].color = Color.green;
                    runDataRef.levelSpeeds[j].outlineColor = Color.green;
                }
                else if (runs[i].playerRunTimes[j] == 0)
                {
                    // runDataRef.levelImages[j].color = Color.yellow;
                    runDataRef.levelSpeeds[j].outlineColor = Color.yellow;
                }
                //Else we set it to red
                else
                {
                    //runDataRef.levelImages[j].color = Color.red;
                    runDataRef.levelSpeeds[j].outlineColor = Color.red;
                }


                if (runData.runTotalTime > 0)
                {
                    if (runData.runTotalTime < runsDataManager.gameAvgTimes[8])
                    {
                        runDataRef.totalTime.outlineColor = Color.green;
                    }
                    //Else we set it to red
                    else
                    {
                        runDataRef.totalTime.outlineColor = Color.red;
                    }
                }
                else
                {
                    runDataRef.totalTime.outlineColor = Color.yellow;
                }
            }
            i++;
        }
    }

    public void UpdateSavedRunDisplay()
    {
        StartCoroutine(RunsWaitForFrame());
    }



    private List<RunDataSet> UpdateSortedList(List<RunDataSet> savedRunsList)
    {
        List<RunDataSet> sortedList = null;
        //Return a correctly sorted list
        switch (currentSortingOption) 
        {
            case SavedRunsSortingOptions.BestRun:
                sortedList = savedRunsList.OrderBy(o => o.runTotalTime).ToList();
                break;
            case SavedRunsSortingOptions.WorstRun:
                sortedList = savedRunsList.OrderByDescending(o => o.runTotalTime).ToList();
                break;
            case SavedRunsSortingOptions.DateCreated:
                sortedList = savedRunsList.OrderBy(o => o.runSaveDate).ToList();
                break;
            case SavedRunsSortingOptions.Alphabetical:
                sortedList = savedRunsList.OrderBy(o=>o.runName).ToList();
                break;
        }
        return sortedList;
    }

    public void UpdateSortType()
    {
        Debug.Log("Attempted sort");
        currentSortingOption = (SavedRunsSortingOptions)sortingOptions.value;
        UpdateSavedRunDisplay();
    }
}

public enum SavedRunsSortingOptions
{
    BestRun = 0, WorstRun = 1, DateCreated = 2, Alphabetical = 3
}