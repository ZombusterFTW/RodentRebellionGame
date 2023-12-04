using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerCameraManager : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    private CinemachineConfiner2D cameraConfiner;

    public static PlayerCameraManager instance;

    // Start is called before the first frame update
    void Start()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.Log("2 Players in the scene. VERY BAD");
        }

        if(virtualCamera == null)
        {
            virtualCamera = GetComponent<CinemachineVirtualCamera>();   
        }
        cameraConfiner = virtualCamera.GetComponent<CinemachineConfiner2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    public void ForceCameraLookat(string objectTag)
    {
        //Force the camera to look at an object during a dialouge segment. 
        if (GameObject.FindGameObjectWithTag(objectTag) != null)
        {
            virtualCamera.LookAt = GameObject.FindGameObjectWithTag(objectTag).transform;
            virtualCamera.Follow = GameObject.FindGameObjectWithTag(objectTag).transform;
            if (objectTag == "Player") 
            {
                virtualCamera.ForceCameraPosition(new Vector3(GameObject.FindGameObjectWithTag(objectTag).transform.position.x, GameObject.FindGameObjectWithTag(objectTag).transform.position.y, virtualCamera.transform.position.z), Quaternion.identity);
            }
        }
        else Debug.Log("Could not find an object with that tag.");
    }

    public void ForceCameraLookat(GameObject objectToLookAt)
    {
        //Force the camera to look at an object during a dialouge segment. For GameObject
        if (objectToLookAt != null)
        {
            virtualCamera.LookAt = objectToLookAt.transform;
            virtualCamera.Follow = objectToLookAt.transform;
        }
        else Debug.Log("Could not find an object with that tag.");
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        //Move camera confinement to the correct shape
        if(collision.GetComponent<CameraBoundsSwitcher>() != null) 
        {
            cameraConfiner.m_BoundingShape2D = collision.gameObject.GetComponent<CameraBoundsSwitcher>()?.cameraBounds;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<AutoDialouge>() != null)
        {
            collision.GetComponent<AutoDialouge>().PlayDialouge();  
        }
    }
}
