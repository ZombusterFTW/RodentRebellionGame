using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
            rightClick.Invoke();
    }


    public void SendDataToRunSaver()
    {
        RunSaverLogic runSaver = FindObjectOfType<RunSaverLogic>();
        if (runSaver != null)
        {
            runSaver.SaveRunLogic(runsData);
        }
    }
}
