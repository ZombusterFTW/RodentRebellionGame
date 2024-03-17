using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomaticDoor : MonoBehaviour, R4Activatable
{
    [Tooltip("The left door GameObject")][SerializeField] private GameObject leftDoor;
    [Tooltip("The right door GameObject")][SerializeField] private GameObject rightDoor;
    [Tooltip("The left door will move to the center of this object when activated")][SerializeField] private GameObject leftDoorTarget;
    [Tooltip("The right door will move to the center of this object when activated")][SerializeField] private GameObject rightDoorTarget;
    [Tooltip("The speed the door will open and close")][SerializeField] private float openCloseSpeed = 8.5f;
    [Tooltip("Set to true if you want the automatic door to begin open.")][SerializeField] private bool startDoorOpen = false;
    bool isActivated = false;
    Vector2 leftDoorStartPos;
    Vector2 rightDoorStartPos;
    Vector2 leftDoorTargetPos;
    Vector2 rightDoorTargetPos;



    // Start is called before the first frame update
    void Awake()
    {
        leftDoorStartPos = leftDoor.transform.position;
        rightDoorStartPos = rightDoor.transform.position;


        leftDoorTarget.GetComponent<SpriteRenderer>().enabled = false;
        rightDoorTarget.GetComponent<SpriteRenderer>().enabled = false; 


        if(!startDoorOpen) 
        {
            if (isActivated)
            {
                Activate();
            }
            else
            {
                Deactivate();
            }
        }
        else
        {
            if (!isActivated)
            {
                Deactivate();
                
            }
            else
            {
                Activate();
            }
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Activate()
    {
        if (!startDoorOpen)
        {
            isActivated = true;
            //Activated the door.
            Debug.Log("Opening Door");
            StopCoroutine(OpenCloseDoor());
            leftDoorTargetPos = leftDoorTarget.transform.position;
            rightDoorTargetPos = rightDoorTarget.transform.position;
            StartCoroutine(OpenCloseDoor());
        }
        else
        {
            isActivated = false;
            Debug.Log("Closing Door");
            StopCoroutine(OpenCloseDoor());
            leftDoorTargetPos = leftDoorStartPos;
            rightDoorTargetPos = rightDoorStartPos;
            StartCoroutine(OpenCloseDoor());
        }
        
    }
    public void Deactivate() 
    {
        if(!startDoorOpen)
        {
            isActivated = false;
            Debug.Log("Closing Door");
            StopCoroutine(OpenCloseDoor());
            leftDoorTargetPos = leftDoorStartPos;
            rightDoorTargetPos = rightDoorStartPos;
            StartCoroutine(OpenCloseDoor());
        }
        else
        {
            isActivated = true;
            //Activated the door.
            Debug.Log("Opening Door");
            StopCoroutine(OpenCloseDoor());
            leftDoorTargetPos = leftDoorTarget.transform.position;
            rightDoorTargetPos = rightDoorTarget.transform.position;
            StartCoroutine(OpenCloseDoor());
        }
        
    }

    IEnumerator OpenCloseDoor()
    {
        while ((leftDoorTargetPos != (Vector2)leftDoor.transform.position) && (rightDoorTargetPos != (Vector2)rightDoor.transform.position))
        {
            leftDoor.transform.position = Vector2.MoveTowards(leftDoor.transform.position, leftDoorTargetPos, openCloseSpeed * Time.deltaTime);
            rightDoor.transform.position = Vector2.MoveTowards(rightDoor.transform.position, rightDoorTargetPos, openCloseSpeed * Time.deltaTime);
            yield return null;
        }
        Debug.Log("Door move complete");
    }
}
