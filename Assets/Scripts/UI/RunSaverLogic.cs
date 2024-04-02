using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class RunSaverLogic : MonoBehaviour
{
    bool isActive = false;
    [SerializeField] Canvas confirmSaveCanvas;
    [SerializeField] RunDataSet runDataSet;
    [SerializeField] SavedRunDataRetreive runSaveRetrieve;
    [SerializeField] RunsDataManager runsDataManager;
    [SerializeField] TMP_InputField runNameField;
    [SerializeField] GameObject dataHolderObj;

    [SerializeField] Canvas confirmSingleRunDeleteCanvas;
    [SerializeField] TextMeshProUGUI deleteSingleRunText;
    [SerializeField] Canvas confirmDeleteAllRunsCanvas;



    public void SaveRunLogic(RunsDataContainer container)
    {
        if (!isActive) 
        {
            runDataSet = container.GetRunDataSet();
            confirmSaveCanvas.enabled = true;
            isActive = true;
        }
    }

    public void CancelSave()
    {
        if(isActive)
        {
            isActive = false;
            runDataSet = new RunDataSet();
            confirmSaveCanvas.enabled = false;
            confirmSingleRunDeleteCanvas.enabled = false;
            confirmDeleteAllRunsCanvas.enabled = false;
            //Wipe field so its blank the next time we use it.
            if (runNameField != null) runNameField.text = string.Empty;
        }
    }


    public void SaveRunDataIntoMemory()
    {
        if(runNameField.text != string.Empty)
        {
            runDataSet.runName = runNameField.text;
            runDataSet.runSaveDate = System.DateTime.Now;
            //This commented to show how multiple runs can be saved. We want to wipe any current runs that are saved.
            SaveData.instance.playerSaveData.playerCurrentRuns[runDataSet.runIndex] = new float[8];
            runDataSet.runIndex = SaveData.instance.playerSaveData.playerSavedRuns.Count - 1;
            SaveData.instance.playerSaveData.playerSavedRuns.Add(runDataSet);
            CancelSave();
            SaveData.instance.playerSaveData.playerSavedRuns = ReindexSavedRuns(SaveData.instance.playerSaveData.playerSavedRuns);
            SaveData.instance.SaveIntoJson();
            runsDataManager.UpdateTimes();
            runSaveRetrieve.UpdateSavedRunDisplay();
        }
    }

    public void DeleteAllSavedRuns()
    {
        SaveData.instance.LoadFromJson();
        SaveData.instance.playerSaveData.playerSavedRuns.Clear();
        SaveData.instance.SaveIntoJson();
        runsDataManager.UpdateTimes();
        runSaveRetrieve.UpdateSavedRunDisplay();
        CancelSave();
    }



    public void DeleteAllRunsPrompt()
    {
        if (!isActive)
        {
            confirmDeleteAllRunsCanvas.enabled = true;
            isActive = true;
        }
    }



    public void DeleteRunFromMemory()
    {
        SaveData.instance.LoadFromJson();
        Destroy(dataHolderObj);
        SaveData.instance.playerSaveData.playerSavedRuns.RemoveAt(runDataSet.runIndex);
        SaveData.instance.playerSaveData.playerSavedRuns = ReindexSavedRuns(SaveData.instance.playerSaveData.playerSavedRuns);
        SaveData.instance.SaveIntoJson();
        runSaveRetrieve.UpdateSavedRunDisplay();
        CancelSave();
        Debug.Log("deleted run");
    }

    public void DeleteSingleRunPrompt(RunsDataContainer container, GameObject dataSetHolder)
    {
        if (!isActive)
        {
            dataHolderObj = dataSetHolder;
            runDataSet = container.GetRunDataSet();
            deleteSingleRunText.text = "Are you sure you want to delete " + runDataSet.runName + "?";
            confirmSingleRunDeleteCanvas.enabled = true;
            isActive = true;
        }
    }


    private List<RunDataSet> ReindexSavedRuns(List<RunDataSet> inData)
    {
        for(int i = 0; i < inData.Count; i++) 
        {
            inData[i].runIndex = i;
        }
        return inData;
    }
}
