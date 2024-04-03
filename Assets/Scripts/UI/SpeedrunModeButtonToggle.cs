using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SpeedrunModeButtonToggle : MonoBehaviour
{
    [SerializeField] Button button;
    [SerializeField] TextMeshProUGUI buttonText;

    [SerializeField] Slider musicSlider;
    [SerializeField] Slider sfxSlider;


    public AudioMixer musicMixer;
    public AudioMixer sfxMixer;

    // Start is called before the first frame update
    void Start()
    {
        SaveData.instance.LoadFromJson();
        if (SaveData.instance.playerSaveData.isPracticeModeUnlocked) UpdateButton();
        else gameObject.SetActive(false);
        musicSlider.value = SaveData.instance.playerSettingsConfig.musicVolume;
        sfxSlider.value = SaveData.instance.playerSettingsConfig.sfxVolume;
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

    public void SetMusicSliderLevel()
    {
        float sliderValue = musicSlider.value;
        musicMixer.SetFloat("MMM", Mathf.Log10(sliderValue) *20);
        SaveData.instance.playerSettingsConfig.musicVolume = musicSlider.value;
        SaveData.instance.SaveIntoJson();
    }

    public void SetSFXSliderLevel()
    {
        float sliderValue = sfxSlider.value;
        sfxMixer.SetFloat("SFXMasterVolume", Mathf.Log10(sliderValue) * 20);
        SaveData.instance.playerSettingsConfig.sfxVolume = sfxSlider.value;
        SaveData.instance.SaveIntoJson();
    }
}
