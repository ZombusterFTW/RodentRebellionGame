using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BrokenBridge : MonoBehaviour
{

    //These appear as broken bridges while in rat mode, but when the player enters rubber mode the bridge gracefully reassembles itself allowing for the player to cross safely.
    private BoxCollider2D bridgeCollider;
    [Tooltip("If set to true the bridge will reassemble in rat mode")][SerializeField] private bool invertBridge = false;
    [Tooltip("This is the time in seconds the bridge will take to deploy")][SerializeField] private float bridgeDeployTime = 1.25f;
    [SerializeField] GameObject bridgeEndPosObj;
    [SerializeField] SpriteRenderer ratBridgeRenderer;
    [SerializeField] SpriteRenderer rubberBridgeRenderer;
    Vector2 bridgeEndPos;
    private Vector2 bridgeStartingLoc;
    Tween movementTween;

    void Start()
    {
        bridgeCollider = GetComponent<BoxCollider2D>();
        bridgeStartingLoc = gameObject.transform.position;
        bridgeEndPos = bridgeEndPosObj.transform.position;
        bridgeEndPosObj.SetActive(false);
        //Set bridge to its correct pos
        RetractBridge();
    }
    private void ResetBridgeColl()
    {
        bridgeCollider.enabled = true;
    }


    public void DeployBridge()
    {
        gameObject.transform.DOComplete();
        if(!invertBridge) 
        {
            movementTween = gameObject.transform.DOMove(bridgeStartingLoc, bridgeDeployTime);
            movementTween.onComplete = ResetBridgeColl;
            rubberBridgeRenderer.DOFade(1, bridgeDeployTime);
            ratBridgeRenderer.DOFade(0, bridgeDeployTime);
        }
        else
        {
            bridgeCollider.enabled = false;
            movementTween = gameObject.transform.DOMove(bridgeEndPos, bridgeDeployTime);
            ratBridgeRenderer.DOFade(0, bridgeDeployTime);
            rubberBridgeRenderer.DOFade(1, bridgeDeployTime);

        }
    }

    public void RetractBridge()
    {
        gameObject.transform.DOComplete();
        if (!invertBridge)
        {
            bridgeCollider.enabled = false;
            movementTween = gameObject.transform.DOMove(bridgeEndPos, bridgeDeployTime);
            rubberBridgeRenderer.DOFade(0, bridgeDeployTime);
            ratBridgeRenderer.DOFade(1, bridgeDeployTime);
        }
        else
        {
            movementTween = gameObject.transform.DOMove(bridgeStartingLoc, bridgeDeployTime);
            movementTween.onComplete = ResetBridgeColl;
            ratBridgeRenderer.DOFade(1, bridgeDeployTime);
            rubberBridgeRenderer.DOFade(0, bridgeDeployTime);
        }
    }


    
}
