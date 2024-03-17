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
        if (SaveData.instance.playerSaveData.currentLevel != "")
        {
            currentLevelText.text = "Current Level: " + SaveData.instance.playerSaveData.currentLevel;
            continueButton.interactable = true;
        }
        else
        {
            currentLevelText.text = "";
            continueButton.interactable = false;
        }
    }

   
}
