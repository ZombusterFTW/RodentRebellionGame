using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunSaverLogic : MonoBehaviour
{
    bool isActive = false;
    SaveData saveData;
    Canvas confirmSaveCanvas;
    //Runs
    private void Start()
    {
        saveData = SaveData.instance;
    }

    public void SaveRunLogic(RunsDataContainer container)
    {

    }
}
