using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpeedrunModeButtonToggle : MonoBehaviour
{
    [SerializeField] Button button;
    [SerializeField] TextMeshProUGUI buttonText;

    // Start is called before the first frame update
    void Start()
    {
        if(SaveData.instance.playerSaveData.isPracticeModeUnlocked) UpdateButton();
        else gameObject.SetActive(false);
    }

    private void UpdateButton()
    {
        if (SaveData.instance.playerSettingsConfig.isSpeedrunModeEnabled)
        {
            buttonText.text = "Speedrun Mode: Enabled";
        }
        else
        {
            buttonText.text = "Speedrun Mode: Disabled";
        }
    }

    public void ToggleSpeedRunMode()
    {
        SaveData.instance.playerSettingsConfig.isSpeedrunModeEnabled = !SaveData.instance.playerSettingsConfig.isSpeedrunModeEnabled;
        SaveData.instance.SaveIntoJson();
        UpdateButton();
    }
}
