using Ink.Parsed;
using Newtonsoft;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


public class SaveData : MonoBehaviour
{
    public PlayerSaveData playerSaveData = new PlayerSaveData();
    public PlayerSettingsConfig playerSettingsConfig = new PlayerSettingsConfig();
    public PracticeModeLevelSettings practiceModeLevelSettings = new PracticeModeLevelSettings();
    public static SaveData instance;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
            //Load from data just in case
            LoadFromJson();
            
        }
        else
        {
            //Delete a copy if it exists.
            Destroy(this.gameObject);
        }
    }


    public void SaveIntoJson()
    {
        //ONLY SAVE AT THE END OF A LEVEL ON EXIT DOOR!
        string playerData = JsonConvert.SerializeObject(playerSaveData);
        string playerSettings = JsonConvert.SerializeObject(playerSettingsConfig);
        string practiceModeSettings = JsonConvert.SerializeObject(practiceModeLevelSettings);
        if(!playerSettingsConfig.playerInTimeWarpMode) System.IO.File.WriteAllText(Application.persistentDataPath + "/PlayerData.json", playerData);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/PlayerConfigSettings.json", playerSettings);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/PracticeModeSettings.json", practiceModeSettings);
        Debug.Log("Saved Player Data");
    }

    public void LoadFromJson()
    {
        Debug.Log("Loaded Save Data");
        //Attempt to load existing save data. 
        if(File.Exists(Application.persistentDataPath + "/PlayerData.json"))
        {
            //If the file exists we turn it into a string and convert from a Json to a PlayerSaveData type
            string playerData = File.ReadAllText(Application.persistentDataPath + "/PlayerData.json");
            playerSaveData = JsonConvert.DeserializeObject<PlayerSaveData>(playerData);
        }
        if (File.Exists(Application.persistentDataPath + "/PlayerConfigSettings.json"))
        {
            //If the file exists we turn it into a string and convert from a Json to a PlayerSaveData type
            string playerConfigSettings = File.ReadAllText(Application.persistentDataPath + "/PlayerConfigSettings.json");
            playerSettingsConfig = JsonConvert.DeserializeObject<PlayerSettingsConfig>(playerConfigSettings);
        }
        if (File.Exists(Application.persistentDataPath + "/PracticeModeSettings.json"))
        {
            //If the file exists we turn it into a string and convert from a Json to a PlayerSaveData type
            string practiceModeSettings = File.ReadAllText(Application.persistentDataPath + "/PracticeModeSettings.json");
            practiceModeLevelSettings = JsonConvert.DeserializeObject<PracticeModeLevelSettings>(practiceModeSettings);
        }
    }
}

[System.Serializable]
public class PlayerSaveData
{
    public string currentLevel = "MainMenu";
    public bool[] currentAbilities = new bool[7];
    public List<PlayerWeaponType> currentPlayerWeapons = new List<PlayerWeaponType>();
    public PlayerWeaponType currentWeapon = PlayerWeaponType.None;
    //A 10 element float array
    public float[] playerBestRun= new float[8];
    //Store last 10 runs. Jagged array each float will be accessed and turned into a timespace. If an array is full of zeros we consider it empty. If the sum of all of these floats is the least out of the entire list we consider the player's best run
    public float[][] playerCurrentRuns = new float[][]{
        new float[8],
        new float[8],
        new float[8],
        new float[8],
        new float[8],
        new float[8],
        new float[8],
        new float[8],
        new float[8],
        new float[8]
    };
    public int currentRunCount = 0;
    public bool isPracticeModeUnlocked = false;
    //List of integer arrays that represent a full run. Make it so a run can only be stored if its completed. Check if the last element of the array is greater than zero to acheive this
    public float[] playerCollectiblesTracker = new float[8];
    public List<RunDataSet> playerSavedRuns = new List<RunDataSet>();    
}

[System.Serializable]
public class PlayerSettingsConfig
{
    public bool isSpeedrunModeEnabled = false;
    public bool playerInTimeWarpMode = false;
    public float musicVolume = 0.75f;
    public float sfxVolume = 0.75f;
}

[System.Serializable]
public class PracticeModeLevelSettings
{
    //This class will store data on what unlocks the player should have if they play a level in practice mode. Track best time on a per level basis in practice mode.
    //Track a player's time if speedrun is enabled to disallow a player from closing the game and restarting their level time with no drawbacks. storing here cause why not.
    public float lastLevelAccruedTime = 0;
    public float[] warpModeLevelTimesPlayer = new float[8];
    public float[] warpModeLevelTimes = new float[8];
    public int[] warpModeCollectibleCountPlayer = new int[8];
    public int[] warpModeCollectibleCount = new int[8];
}

[System.Serializable]
public class RunDataSet
{
    public float[] playerRunTimes = new float[8];
    public string runName = "Unnamed Run";
    public int runIndex = 0;
}

public enum R4Level
{
    //set each enumeration to the build index of the actual level for easy casting.
    Tutorial,
    Labyrinth1,
    Labyrinth2,
    Labyrinth3,
    RadioactiveCave,
    TheSurface,
    FinalBoss,
    TheLab
}