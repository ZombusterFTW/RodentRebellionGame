using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System;

public class PlayerCameraManager : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    private CinemachineConfiner2D cameraConfiner;

    public static PlayerCameraManager instance;

    public GameObject mapImage;
    public Sprite level1Image;
    public Sprite level2Image;
    public Sprite level3Image;


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
        SetLevelImage(SceneManager.GetActiveScene().buildIndex);
        SceneManager.sceneLoaded += OnSceneChanged;
    }

    private void OnSceneChanged(Scene arg0, LoadSceneMode arg1)
    {
        SetLevelImage(SceneManager.GetActiveScene().buildIndex);
    }

    public void SetLevelImage(int sceneIndex)
    {
        Debug.Log("Attempted to load image");
        switch(sceneIndex) 
        {
            case 1:
                mapImage.GetComponent<SpriteRenderer>().sprite = level1Image;
                break;
            case 2:
                mapImage.GetComponent<SpriteRenderer>().sprite = level2Image;
                break;
            case 3:
                mapImage.GetComponent<SpriteRenderer>().sprite = level3Image;
                break;
        }

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
        if(collision.GetComponent<TriggerActivater>() != null)
        {
            collision.GetComponent<TriggerActivater>().Trigger();
        }
    }
}
