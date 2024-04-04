using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitDoor : MonoBehaviour,R4Activatable
{
    [SerializeField] private string sceneToLoad;
    bool playerInRange = false;
    bool isActive = false;
    [SerializeField] bool activatedByOtherObject = false;
    [SerializeField] private GameObject visualCue;
    [Tooltip("Set this to true if you want a scene change to happen on a switch pull for example. ACTIVATED BY GAMEOBJECT MUST BE TRUE! Place the door outside of the playable space if you use this.")][SerializeField] private bool forceSceneChangeOnDoorActivation = false;


    private void Awake()
    {
        playerInRange = false;
        visualCue.SetActive(false);
        if(!activatedByOtherObject)
        {
            isActive = true;
        }
    }

    private void Update()
    {
        if (playerInRange && !DialogueManager.GetInstance().dialogueIsPlaying && isActive)
        {
            if (PlayerController.instance.GetInteractPressed())
            {
                HandleSceneChange();
            }
        }
    }

    private void HandleSceneChange()
    {
        isActive = false;
        if (SaveData.instance.playerSettingsConfig.playerInTimeWarpMode) SceneTransitionerManager.instance.StartTransition("TimeWarp");
        else SceneTransitionerManager.instance.StartTransition(sceneToLoad);
        //Lap level time
        PlayerController.instance.GetPlayerUI().GetComponent<MatchTimer>().LapTime();
        SavePlayerVariables();
    }

    private void SavePlayerVariables()
    {
        //Check for player variables here and save them. 
        if(SaveData.instance != null) 
        {
            SaveData.instance.practiceModeLevelSettings.lastLevelAccruedTime = 0;
            //Track level name and save data put into memory by other classes
            if (!SaveData.instance.playerSettingsConfig.playerInTimeWarpMode)
            {
                SaveData.instance.playerSaveData.currentLevel = sceneToLoad;
                //If we get here the player completed the game
                if(sceneToLoad == "MainMenu")
                {
                    SaveData.instance.playerSaveData.isPracticeModeUnlocked = true; 
                }
            }
            SaveData.instance.SaveIntoJson();
        }
    }





    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            playerInRange = true;
            if(isActive) 
            {
                visualCue.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            playerInRange = false;
            if (isActive)
            {
                visualCue.SetActive(false);
            }
        }
    }

    public void Activate()
    {
        if(activatedByOtherObject && !isActive)
        {
            isActive = true;
            if(forceSceneChangeOnDoorActivation)
            {
                HandleSceneChange();
            }
        }
        
    }

    public void Deactivate()
    {
        if (activatedByOtherObject)
        {
            isActive = false;
        }
    }
}
