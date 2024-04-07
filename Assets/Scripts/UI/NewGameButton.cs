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
            //Increment run only if the player has completed the game. If they begin a new game before finishing we reset their current run.

            //Fixed bug where clicking new game would delete your current run.
            if (SaveData.instance.playerSaveData.currentRunCount < 10 && SaveData.instance.playerSaveData.currentLevel == "MainMenu")
            {
                //Increment the run
                SaveData.instance.playerSaveData.currentRunCount++;
            }
            else
            {
                SaveData.instance.playerSaveData.currentRunCount = 0;
            }
            //We reset that run to ensure its cleared
            SaveData.instance.playerSaveData.playerCurrentRuns[SaveData.instance.playerSaveData.currentRunCount] = new float[8];

            ///Debug to manually reset the players best run
            //SaveData.instance.playerSaveData.playerBestRun = new float[8];

            //Set level as lv0
            SaveData.instance.playerSaveData.currentLevel = "0Tutorial";
            //Clear list of current player weapons
            SaveData.instance.playerSaveData.currentPlayerWeapons = new List<PlayerWeaponType>() { PlayerWeaponType.None };
            //reset weapon
            SaveData.instance.playerSaveData.currentWeapon = PlayerWeaponType.None;
            //Wipe all abilities
            SaveData.instance.playerSaveData.currentAbilities = new bool[7];
            if (SceneTransitionerManager.instance != null)
            {
                SceneTransitionerManager.instance.runsShowcase.UpdateTimes();
            }
            SaveData.instance.practiceModeLevelSettings.lastLevelAccruedTime = 0;
            //Save.
            SaveData.instance.SaveIntoJson();
        }
    }


    private void Start()
    {
        SaveData.instance.LoadFromJson();
        //This is to ensure that the player is always out of TimeWarp mode if they return to the mainmenu.
        SaveData.instance.playerSettingsConfig.playerInTimeWarpMode = false;
        //SaveData.instance.playerSaveData.isPracticeModeUnlocked = true;
        //SaveData.instance.SaveIntoJson();
    }
}
