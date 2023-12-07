using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPannerRR : MonoBehaviour, DialougeActivated, R4Activatable
{
    [SerializeField] GameObject cameraStart;
    [SerializeField] GameObject cameraEnd;

    [SerializeField] float panTime;
    [SerializeField] bool sceneChangeOnPanFinish = false;

    public void Activate()
    {
        PlayerCameraManager.instance.virtualCamera.ForceCameraPosition(new Vector3(cameraStart.transform.position.x, cameraStart.transform.position.y, PlayerCameraManager.instance.virtualCamera.gameObject.transform.position.z), Quaternion.identity);
        PlayerCameraManager.instance.virtualCamera.LookAt =  null;
        PlayerCameraManager.instance.virtualCamera.Follow = null;
        PlayerCameraManager.instance.virtualCamera.gameObject.transform.DOMove(cameraEnd.transform.position, panTime).OnComplete(() => ItemMoveCompleted());
    }

    private void ItemMoveCompleted()
    {
        if(sceneChangeOnPanFinish) 
        {
            SceneTransitionerManager.instance.StartTransition();
        }
    }

    public void Deactivate()
    {
        throw new System.NotImplementedException();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
