using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RunSaverLogic : MonoBehaviour
{
    bool isActive = false;
    [SerializeField] Canvas confirmSaveCanvas;
    RunDataSet runDataSet;
    [SerializeField] SavedRunDataRetreive runSaveRetrieve;
    [SerializeField] RunsDataManager runsDataManager;
    [SerializeField] TMP_InputField runNameField;



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
        }
    }


    public void SaveRunDataIntoMemory()
    {
        if(runNameField.text != string.Empty)
        {
            runDataSet.runName = runNameField.text;
            SaveData.instance.playerSaveData.playerSavedRuns.Add(runDataSet);
           // SaveData.instance.playerSaveData.playerCurrentRuns[SaveData.instance.playerSaveData.currentRunCount] = new float[8];
            CancelSave();
            SaveData.instance.SaveIntoJson();
            runsDataManager.UpdateTimes();
            runSaveRetrieve.UpdateSavedRunDisplay();
        }
    }

    public void DeleteAllSavedRuns()
    {
        SaveData.instance.playerSaveData.playerSavedRuns.Clear();
        SaveData.instance.SaveIntoJson();
        runsDataManager.UpdateTimes();
        runSaveRetrieve.UpdateSavedRunDisplay();
    }
}
