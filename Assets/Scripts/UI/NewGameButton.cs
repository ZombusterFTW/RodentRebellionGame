using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewGameButton : MonoBehaviour
{
    bool hasRun = false;
    [SerializeField] Button newGameButton;
    //This script must reset player ability, last level, weapon list, and current weapon. Increment run counter.

    public void StartNewGame()
    {
        if(!hasRun)
        {
            //prevent a double click of this button.
            hasRun = true;
            //Increment run
            if(SaveData.instance.playerSaveData.currentRunCount < 10 && SaveData.instance.playerSaveData.currentRunCount != 0) SaveData.instance.playerSaveData.currentRunCount++;
            else SaveData.instance.playerSaveData.currentRunCount = 0;
            //Set level as lv0
            SaveData.instance.playerSaveData.currentLevel = "0Tutorial";
            //Clear list of current player weapons
            SaveData.instance.playerSaveData.currentPlayerWeapons.Clear();
            //reset weapon
            SaveData.instance.playerSaveData.currentWeapon = PlayerWeaponType.None;
            //Wipe all abilities
            SaveData.instance.playerSaveData.currentAbilities = new bool[7];
            //Save.
            SaveData.instance.SaveIntoJson();
        }
    }
}
