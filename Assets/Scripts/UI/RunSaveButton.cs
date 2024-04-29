using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;

public class RunSaveButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public UnityEvent leftClick;
    public UnityEvent middleClick;
    public UnityEvent rightClick;
    [SerializeField] private RunsDataContainer runsData;
    public TextMeshProUGUI promptText;


    private void Awake()
    {
        promptText.gameObject.SetActive(false);
    }


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




    public void SaveRunAsCSV()
    {
        RunSaverLogic runSaver = FindObjectOfType<RunSaverLogic>();
        if (runSaver != null)
        {
            Debug.Log("Begin Save");
            runSaver.SaveRunCSV(runsData);
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
            runSaver.DeleteSingleRunPrompt(runsData, transform.parent.gameObject);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        promptText.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        promptText.gameObject.SetActive(false);
    }
}
