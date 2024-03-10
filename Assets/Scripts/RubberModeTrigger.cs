using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RubberModeTrigger : MonoBehaviour
{
    [Header("This script forces the player out of rubbermode")]
    //This trigger will force a player to leave rubbermode and has the option to prevent them from entering rubber mode at all if they are inside it.
    [SerializeField][Tooltip("If true the player will be preventing from phase changing at all when inside this trigger.")] bool disablePlayerPhaseChange = false;
    FrenzyManager frenzyManager;
    void Start()
    {
        frenzyManager = FrenzyManager.instance;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(PlayerController.instance != null && collision.gameObject == PlayerController.instance.gameObject) 
        {
            if (frenzyManager.inRubberMode)
            {
                Debug.Log("Kicked player out of rubber mode");
                frenzyManager.ToggleRubberMode();
            }
            else Debug.Log("Player in rat mode");
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (PlayerController.instance != null && collision.gameObject == PlayerController.instance.gameObject)
        {
            if (disablePlayerPhaseChange) frenzyManager.stateChangeDisabled = true;
        }
            
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (PlayerController.instance != null && collision.gameObject == PlayerController.instance.gameObject)
        {
            if (disablePlayerPhaseChange) frenzyManager.stateChangeDisabled = false;
        }
            
    }





}
