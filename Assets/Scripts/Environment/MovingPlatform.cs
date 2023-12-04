using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour, R4Activatable
{
    [SerializeField] private GameObject movingPlatform;
    [SerializeField] private GameObject platformStart;
    [SerializeField] private GameObject platformEnd;

    [Tooltip("Set to true for platform to begin at the end position.")][SerializeField] private bool startAtEnd = false;
    [Tooltip("Set to true for platform to begin moving.")][SerializeField] private bool isActive = false;
    [Tooltip("The speed the platform will move at.")][SerializeField] private float platformSpeed = 8f;
    [Tooltip("How long the platform will wait once it reaches destination")][SerializeField] private float platformWaitTime = 5f;


    private Vector3 currentTarget;




    public void Activate()
    {
        if(!isActive) 
        {
            StartCoroutine(MovePlatform());
            isActive = true;
        }
    }

    public void Deactivate()
    {
        if (isActive)
        {
            StopCoroutine(MovePlatform());
            isActive = false;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        platformStart.GetComponent<SpriteRenderer>().enabled = false;
        platformEnd.GetComponent<SpriteRenderer>().enabled = false; 
        //Move platform to its start or end. 
        if (startAtEnd)
        {
            movingPlatform.transform.position = platformEnd.transform.position;
            currentTarget = platformStart.transform.position;
        }
        else
        {
            movingPlatform.transform.position = platformStart.transform.position;
            currentTarget = platformEnd.transform.position;
        }
        if(isActive)StartCoroutine(MovePlatform());
    }



    IEnumerator MovePlatform() 
    {
        while(isActive)
        {
            yield return new WaitForSeconds(platformWaitTime);
            while (movingPlatform.transform.position != currentTarget)
            {
                movingPlatform.transform.position = Vector3.MoveTowards(movingPlatform.transform.position, currentTarget, platformSpeed * Time.deltaTime);
                yield return null;
            }

            if(currentTarget == platformEnd.transform.position)
            {
                currentTarget = platformStart.transform.position;
            }
            else currentTarget = platformEnd.transform.position;
        }
        yield return null;
    }




    private void Update()
    {
        
    }
}
