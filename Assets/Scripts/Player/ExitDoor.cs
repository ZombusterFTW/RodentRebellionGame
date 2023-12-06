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
            if (GameObject.FindObjectOfType<PlayerController>().GetInteractPressed())
            {
                SceneTransitionerManager.instance.StartTransition(sceneToLoad);
            }
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
        if(activatedByOtherObject)
        {
            isActive = true;
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
