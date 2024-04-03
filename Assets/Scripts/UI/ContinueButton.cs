using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ContinueButton : MonoBehaviour
{
    [SerializeField] Button continueButton;
    [SerializeField] TextMeshProUGUI currentLevelText;
    void Start()
    {
        SaveData.instance.LoadFromJson();
        //Allow for continue button functionality if the player has save progress.
        if (SaveData.instance.playerSaveData.currentLevel != "" && SaveData.instance.playerSaveData.currentLevel != "MainMenu")
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
