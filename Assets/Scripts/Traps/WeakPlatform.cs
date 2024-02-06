using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class WeakPlatform : MonoBehaviour, R4Activatable
{
    [Tooltip("Set this bool to false if you want this platform to start inactive")][SerializeField] private bool startOn = true;
    [Tooltip("This value is the time in seconds before a platform will fall after being in contact with the player.")][SerializeField] private float platformFallDelay = 0.25f;
    [Tooltip("If this bool is set to true the platform will return after a customizable delay.")][SerializeField] private bool canPlatformRespawn = false;
    [Tooltip("If the above bool is true the platform will respawn in this many seconds after reaching its destination.")][SerializeField] private float platformRespawnDelay = 5f;
    [Tooltip("If this bool is true a platform will only fall if a player has touched it for platformFallDelay seconds")][SerializeField] private bool platformDurability = false;
    [Tooltip("This distance is how far the platform will fall. This value is sign sensitive. negative values will cause the platform to fall downwards, positive values will cause it to fall up.")][SerializeField] private float fallDistance = -1000f;
    [Tooltip("How long it takes for the falling platform to reach its destination")][SerializeField] private float platformFallTime = 2f;
    private SpriteRenderer spriteRenderer;
    private bool isActive;
    private Coroutine platformLoop;
    private float platformTouchedSeconds;
    private Vector3 targetPosition;
    private Vector3 startingPosition;
    private BoxCollider2D platformCollider;

    public void Activate()
    {
        if(!isActive) 
        {
            isActive = true;
            spriteRenderer.color = Color.white;
        }
    }

    public void Deactivate()
    {
        Debug.Log("Once active weak platforms cannot be deactivated");
    }

    void TriggerPlatform()
    {
       //Prevent more than one coroutine running on this script. 
       if(platformLoop == null) platformLoop = StartCoroutine(FallingPlatformLoop());
    }

    IEnumerator FallingPlatformLoop()
    {
        spriteRenderer.DOComplete();
        if (!platformDurability) yield return new WaitForSeconds(platformFallDelay);
        spriteRenderer.color = Color.red;
        spriteRenderer.DOFade(0, 2);
        platformCollider.enabled = false;
        gameObject.transform.DOMove(targetPosition, platformFallTime);
        yield return new WaitForSeconds(platformFallTime);
        if (canPlatformRespawn) yield return new WaitForSeconds(platformRespawnDelay);
        else
        {
            CleanupPlatform();
            yield break;
        }
        //If we get here the platform needs to respawn.
        spriteRenderer.color = Color.white;
        spriteRenderer.DOFade(1, 2);
        gameObject.transform.position = startingPosition;
        platformCollider.enabled = true;
        platformLoop = null;
        yield return null;  
    }

    void CleanupPlatform()
    {
        //Destroy is delayed to allow the coroutine time to end.
        Destroy(gameObject, 0.25f);
    }


    // Start is called before the first frame update
    void Start()
    {
        startingPosition = gameObject.transform.position;
        targetPosition = gameObject.transform.position + new Vector3(0, fallDistance,0);
        spriteRenderer = GetComponent<SpriteRenderer>();
        platformCollider = GetComponent<BoxCollider2D>();   
        spriteRenderer.color = Color.gray;
        if(startOn) 
        {
            Activate();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player" && !platformDurability)
        {
            //Moving platforms should only be triggered by the player. If its decided to let enemies affect them, check for the R4 movement component instead.
            TriggerPlatform();
        }
    }


    private void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player" && platformDurability)
        {
            platformTouchedSeconds += Time.deltaTime;
            if (platformTouchedSeconds >= platformFallDelay)
            {
                TriggerPlatform();
            }
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        platformTouchedSeconds = 0;
    }
}
