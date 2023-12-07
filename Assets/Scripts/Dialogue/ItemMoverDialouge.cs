using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

public class ItemMoverDialouge : MonoBehaviour, DialougeActivated
{
    [SerializeField] private GameObject itemStartLocation;
    [SerializeField] private GameObject itemEndLocation;
    [SerializeField] private GameObject itemToMove;
    [SerializeField] private float itemMoveTime;
    [SerializeField] private bool startInvisible = false;
    [SerializeField] private bool freezePlayer = false;
    private bool moveInProgress = false;    
    private bool moveFinished = false;
    PlayerController player;
    [SerializeField] private bool persistAfterMove = false;
    [SerializeField] private bool returnToMenuOnFinish = false;
    Tween itemMoveTween;
    public void Activate()
    {
        if(itemToMove != null)
        {
            itemToMove.gameObject.SetActive(true);
            moveInProgress = true;
            itemToMove.transform.DOMove(itemEndLocation.transform.position, itemMoveTime).OnComplete(() => ItemMoveCompleted());
            PlayerCameraManager.instance.ForceCameraLookat(itemToMove);
            player.DisableControls(true);
        }
    }

   

    public void Deactivate()
    {
        //Not really supported yet
        if (itemToMove != null)
        {
            itemToMove.gameObject.SetActive(true);
            itemToMove.transform.DOMove(itemStartLocation.transform.position, itemMoveTime);
        }
    }


    private void Update()
    {
        if(this != null)
        {
            if(moveInProgress == true)
            {
                player.DisableControls(true);
            }
        }
    }


    private void ItemMoveCompleted()
    {
        player.DisableControls(true);
        Debug.Log("Move done");
        moveInProgress = false;
        moveFinished = true;
        if(returnToMenuOnFinish)
        {
            SceneTransitionerManager.instance.StartTransition();
        }
        else PlayerCameraManager.instance.ForceCameraLookat("Player");
        player.DisableControls(false);
        if (!persistAfterMove)
        {
            this.enabled = false;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindObjectOfType<PlayerController>();
        if (itemStartLocation.GetComponent<SpriteRenderer>() != null) itemStartLocation.GetComponent<SpriteRenderer>().enabled = false;
        if (itemStartLocation.GetComponent<SpriteRenderer>() != null) itemEndLocation.GetComponent<SpriteRenderer>().enabled = false;
        itemToMove.transform.position = itemStartLocation.transform.position;
        if(startInvisible)
        {
            itemToMove.gameObject.SetActive(false);
        }
    }


}

public interface DialougeActivated
{
    public void Activate();
    public void Deactivate();
}