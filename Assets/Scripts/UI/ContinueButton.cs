using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ContinueButton : MonoBehaviour
{
    [SerializeField] Button continueButton;
    [SerializeField] TextMeshProUGUI currentLevelText;
    [SerializeField] TextMeshProUGUI completeGameHint;
    [SerializeField] Button RunsButton;
    [SerializeField] Button TimeWarpButton;


    void Start()
    {
        SaveData.instance.LoadFromJson();


        if(continueButton != null)
        {
            //Allow for continue button functionality if the player has save progress.
            if (SaveData.instance.playerSaveData.currentLevel != "" && SaveData.instance.playerSaveData.currentLevel != "Credits")
            {
                currentLevelText.text = "Current Level: " + GetLevelNamePretty(SaveData.instance.playerSaveData.currentLevel);
                continueButton.interactable = true;
            }
            else
            {
                currentLevelText.text = "";
                continueButton.interactable = false;
            }
        }

        //Prevent player from accessing menus
        if(RunsButton != null && TimeWarpButton != null  && SaveData.instance.playerSaveData.isPracticeModeUnlocked) 
        {
            RunsButton.interactable = true;
            TimeWarpButton.interactable = true;
            completeGameHint.text = "Thanks for completing the Rodent Rebellion Beta! Sincerely, Rubber Room Studios.";
        }
        else
        {
            RunsButton.interactable = false;
            TimeWarpButton.interactable = false;
            completeGameHint.gameObject.SetActive(true);
        }
    }

    private string GetLevelNamePretty(string inLevelName)
    {
        switch(inLevelName) 
        {
            default:
                return "Error :(";
            case "0Tutorial":
                return "Tutorial";
            case "Labyrinth3":
                return "Labyrinth α";
            case "Labyrinth2":
                return "Labyrinth δ";
            case "Labyrinth1":
                return "Labyrinth Ω";
            case "RadioactiveCave":
                return "Radioactive Cave";
            case "LabLevel":
                return "The Labs";
            case "FinalBossTest":
                return "Final Boss";
            case "Surface":
                return "The Surface";
        }
    }





}
