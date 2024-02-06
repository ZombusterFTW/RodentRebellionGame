using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapBridge : MonoBehaviour, R4Activatable
{
    //This trap will be a bridge that will snap into place when activated. Think of it as a binary moving platform. Any player or enemy caught between the platform during its movement will be eliminated instantly. 
    //This trap doesn't really have an on/off state. The platform will begin at a pos and go to the 2nd pos when power is active.

    [Tooltip("If set to true the snap bridge will start at the red pos")][SerializeField] private bool startAtEnd;
    [Tooltip("How long the snap bridge takes to go between its 2 positions")][SerializeField] private float timeToSnap;
    [SerializeField] private GameObject startPosition;
    [SerializeField] private GameObject endPosition;
    private bool inMotion = false;
    private bool isActive = false;
    private SpriteRenderer spriteRenderer;
    private Vector3 targetPosition;
    private Vector3 startingVec;
    private Vector3 endingVec;
    //Slighty redundant here as the on and off of a button connected to this bridge would just toggle it. 
    public void Activate()
    {
        if (!isActive)
        {
            isActive = true;
            TriggerSnapBridge();
        }
    }

    public void Deactivate()
    {
        if (isActive) 
        {
            isActive = false;
            TriggerSnapBridge();
        }
    }


    void OnMovementEnd()
    {
        inMotion = false;
        spriteRenderer.color = Color.white;
    }

    void TriggerSnapBridge()
    {
        inMotion = false;
        gameObject.transform.DOComplete();
        gameObject.transform.DOMove(targetPosition, timeToSnap).onComplete = OnMovementEnd;
        spriteRenderer.color = Color.red;
        inMotion = true;
        if(targetPosition == startingVec)
        {
            targetPosition = endingVec;
        }
        else
        {
            targetPosition = startingVec;
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        startPosition.GetComponent<SpriteRenderer>().enabled = false;   
        endPosition.GetComponent<SpriteRenderer>().enabled = false;
        spriteRenderer = GetComponent<SpriteRenderer>();  
        startingVec = startPosition.transform.position;
        endingVec = endPosition.transform.position;


        spriteRenderer.color = Color.white;
        if (startAtEnd)
        {
            gameObject.transform.position = endingVec;
            targetPosition = startingVec;
        }
        else
        {
            gameObject.transform.position = startingVec;
            targetPosition = endingVec;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(inMotion) 
        {
            if(collision.gameObject.GetComponent<ControlledCharacter>() != null)
            {
                //If a player or other controlled enemy is caught between the platform during its move respawn them.
                collision.gameObject.GetComponent<ControlledCharacter>().RespawnPlayer();
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (inMotion)
        {
            if (collision.gameObject.GetComponent<ControlledCharacter>() != null)
            {
                //If a player or other controlled enemy is caught between the platform during its move respawn them.
                collision.gameObject.GetComponent<ControlledCharacter>().RespawnPlayer();
            }
        }
    }
}
