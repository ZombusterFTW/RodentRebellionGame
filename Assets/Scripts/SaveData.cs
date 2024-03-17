using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveData : MonoBehaviour
{
    public PlayerSaveData playerSaveData = new PlayerSaveData();
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
        string playerData = JsonUtility.ToJson(playerSaveData);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/PlayerData.json", playerData);
    }

    public void LoadFromJson()
    {
        if(File.Exists(Application.persistentDataPath + "/PlayerData.json"))
        {
            //If the file exists we turn it into a string and convert from a Json to a PlayerSaveData type
            string playerData = File.ReadAllText(Application.persistentDataPath + "/PlayerData.json");
            playerSaveData = JsonUtility.FromJson<PlayerSaveData>(playerData);
        }
    }

}

[System.Serializable]
public class PlayerSaveData
{
    public string currentLevel = "0Tutorial";
    public bool[] currentAbilities = new bool[7];
    public List<PlayerWeaponType> currentPlayerWeapons = new List<PlayerWeaponType>();
    public PlayerWeaponType currentWeapon;
    //A 10 element float array
    public float[] playerBestRun= new float[7];
    //Store last 10 runs. Jagged array each float will be accessed and turned into a timespace. If an array is full of zeros we consider it empty. If the sum of all of these floats is the least out of the entire list we consider the player's best run
    public float[][] playerCurrentRuns = new float[][]{
        new float[7],
        new float[7],
        new float[7],
        new float[7],
        new float[7],
        new float[7],
        new float[7],
        new float[7],
        new float[7],
        new float[7]
    };

 
}