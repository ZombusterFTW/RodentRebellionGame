using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using System.IO;
using Environment = System.Environment;
using System;
using System.Text.RegularExpressions; // needed for Regex

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
    [SerializeField] Canvas confirmRunSaveToCSV;
    [SerializeField] TextMeshProUGUI confirmRunSaveToCSVText;
    [SerializeField] TextMeshProUGUI deleteSingleRunText;
    [SerializeField] Canvas confirmDeleteAllRunsCanvas;
    string[] levelNames = new string[8] { "Tutorial", "Labyrinth 1", "Radioactive Cave", "Labyrinth 2", "The Labs", "Labyrinth 3", "The Surface", "Final Boss" };
    public GameObject upgradeHintObject;

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
            confirmRunSaveToCSV.enabled = false;
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
            GameObject hintText = Instantiate(upgradeHintObject);
            hintText.GetComponentInChildren<TextMeshProUGUI>().text = "Successfuly Saved Run!";
            hintText.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
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
        GameObject hintText = Instantiate(upgradeHintObject);
        hintText.GetComponentInChildren<TextMeshProUGUI>().text = "Successfuly Deleted All Saved Runs!";
        hintText.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
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
        //SaveData.instance.LoadFromJson();
        for (int i = 0; i < SaveData.instance.playerSaveData.playerSavedRuns.Count;  i++)
        {
            if (SaveData.instance.playerSaveData.playerSavedRuns[i].runIndex == runDataSet.runIndex)
            {
                Debug.Log("FOUND");
                SaveData.instance.playerSaveData.playerSavedRuns[i].ToString();
                SaveData.instance.playerSaveData.playerSavedRuns.RemoveAt(i);
                break;
            }
        }
        Destroy(dataHolderObj);
        SaveData.instance.playerSaveData.playerSavedRuns = ReindexSavedRuns(SaveData.instance.playerSaveData.playerSavedRuns);
        SaveData.instance.SaveIntoJson();
        runSaveRetrieve.UpdateSavedRunDisplay();
        CancelSave();
        GameObject hintText = Instantiate(upgradeHintObject);
        hintText.GetComponentInChildren<TextMeshProUGUI>().text = "Successfuly Deleted Run!";
        hintText.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
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



    public void SaveRunCSV(RunsDataContainer container)
    {
        if (!isActive)
        {
            runDataSet = container.GetRunDataSet();

            confirmRunSaveToCSV.enabled = true;
            confirmRunSaveToCSVText.text = "Are you sure you want to export " + runDataSet.runName + " as a .CSV file? .CSV files are saved at MyDocuments/My Games/RodentRebellion.";
            isActive = true;
        }
    }


    public void SaveRunToCSVFile()
    {
        string filePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/RodentRebellion/";
        DirectoryInfo info = Directory.CreateDirectory(filePath);
        filePath += runDataSet.runName + ".csv";
        StreamWriter writer = new StreamWriter(filePath);
        writer.WriteLine("Level,Time");

        for(int i = 0; i < runDataSet.playerRunTimes.Length; i++) 
        {
            TimeSpan ts = TimeSpan.FromSeconds(runDataSet.playerRunTimes[i]);
            writer.WriteLine(levelNames[i] + "," + ts.ToString("mm") + ":" + ts.ToString("ss") + ":" + ts.ToString("ff"));
        }
        TimeSpan ts2 = TimeSpan.FromSeconds(runDataSet.runTotalTime);
        writer.WriteLine("Run Total" + "," + ts2.ToString("mm") + ":" + ts2.ToString("ss") + ":" + ts2.ToString("ff"));
        writer.WriteLine("Run Creation Date" + "," + runDataSet.runSaveDate.ToString());
        writer.Flush();
        writer.Close();
        Debug.Log("Saved run to csv file.");
        CancelSave();
        GameObject hintText = Instantiate(upgradeHintObject);
        hintText.GetComponentInChildren<TextMeshProUGUI>().text = "Successfuly Exported Run!";
        hintText.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
    }


    public void EnsureStringIsLegal()
    {
        string runNameInProg = runNameField.text;
        runNameInProg = Regex.Replace(runNameInProg, @"[^a-zA-Z0-9 ]", "");
        runNameField.text = runNameInProg;
    }
}
