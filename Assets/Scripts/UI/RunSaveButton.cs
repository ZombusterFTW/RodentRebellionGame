using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class RunSaveButton : MonoBehaviour, IPointerClickHandler
{
    public UnityEvent leftClick;
    public UnityEvent middleClick;
    public UnityEvent rightClick;
    public RunsDataContainer runsData;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            leftClick.Invoke();
        else if (eventData.button == PointerEventData.InputButton.Middle)
            middleClick.Invoke();
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            rightClick.Invoke();
            Debug.Log("Right Click Detected");
        }
            
    }


    public void SendDataToRunSaver()
    {
        RunSaverLogic runSaver = FindObjectOfType<RunSaverLogic>();
        if (runSaver != null)
        {
            Debug.Log("Begin Save");
            runSaver.SaveRunLogic(runsData);
        }
    }

    public void DeleteSingleRun()
    {
        RunSaverLogic runSaver = FindObjectOfType<RunSaverLogic>();
        if (runSaver != null)
        {
            Debug.Log("Begin Save");
            runSaver.DeleteSingleRunPrompt(runsData, gameObject);
        }
    }
}
