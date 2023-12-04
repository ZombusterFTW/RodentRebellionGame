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


   private bool moveFinished = false;
    public void Activate()
    {
        if(itemToMove != null)
        {
            itemToMove.gameObject.SetActive(true);
            itemToMove.transform.DOMove(itemEndLocation.transform.position, itemMoveTime);
            PlayerCameraManager.instance.ForceCameraLookat(itemToMove);
        }
    }

    public void Deactivate()
    {
        if (itemToMove != null)
        {
            itemToMove.gameObject.SetActive(true);
            itemToMove.transform.DOMove(itemStartLocation.transform.position, itemMoveTime);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (itemStartLocation.GetComponent<SpriteRenderer>() != null) itemStartLocation.GetComponent<SpriteRenderer>().enabled = false;
       if (itemStartLocation.GetComponent<SpriteRenderer>() != null) itemEndLocation.GetComponent<SpriteRenderer>().enabled = false;
        itemToMove.transform.position = itemStartLocation.transform.position;
        if(startInvisible)
        {
            itemToMove.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(itemToMove != null && itemToMove.transform.position == itemEndLocation.transform.position && !moveFinished)
        {
            moveFinished = true;
            PlayerCameraManager.instance.ForceCameraLookat("Player");
        }
    }
}

public interface DialougeActivated
{
    public void Activate();
    public void Deactivate();
}