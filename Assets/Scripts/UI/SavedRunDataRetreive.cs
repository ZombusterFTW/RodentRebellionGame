using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavedRunDataRetreive : MonoBehaviour
{
    [SerializeField] GameObject runDataPrefab;
    [SerializeField] GameObject viewPortParent;
    [SerializeField] RunsDataManager runsDataManager;
    [SerializeField] List<GameObject> savedRuns = new List<GameObject>();

    private void Start()
    {
        UpdateSavedRunDisplay();
    }

    public void UpdateSavedRunDisplay()
    {

        foreach(GameObject run in savedRuns) 
        {
            Destroy(run);
        }
        SaveData.instance.LoadFromJson();
        List<RunDataSet> runs = new List<RunDataSet>();
        runs = SaveData.instance.playerSaveData.playerSavedRuns;
        //variable to track current level speed.
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
                    runDataRef.levelImages[j].color = Color.green;
                }
                else if (runs[i].playerRunTimes[j] == 0)
                {
                    runDataRef.levelImages[j].color = Color.yellow;
                }
                //Else we set it to red
                else
                {
                    runDataRef.levelImages[j].color = Color.red;
                }
            }
            i++;
        }
    }
}
