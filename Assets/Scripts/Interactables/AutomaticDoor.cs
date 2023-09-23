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
    bool isActivated = false;
    bool isMoving = false;
    Vector2 leftDoorStartPos;
    Vector2 rightDoorStartPos;
    Vector2 leftDoorTargetPos;
    Vector2 rightDoorTargetPos;



    // Start is called before the first frame update
    void Start()
    {
        leftDoorStartPos = leftDoor.transform.position;
        rightDoorStartPos = rightDoor.transform.position;   
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Activate()
    {
        //Activated the door.
        Debug.Log("Opening Door");
        StopCoroutine(OpenCloseDoor());
        leftDoorTargetPos = leftDoorTarget.transform.position;
        rightDoorTargetPos = rightDoorTarget.transform.position;
        StartCoroutine(OpenCloseDoor());
    }
    public void Deactivate() 
    {
        Debug.Log("Closing Door");
        StopCoroutine(OpenCloseDoor());
        leftDoorTargetPos = leftDoorStartPos;
        rightDoorTargetPos = rightDoorStartPos;
        StartCoroutine(OpenCloseDoor());
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
