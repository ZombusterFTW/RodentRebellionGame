using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityInversionTrap : MonoBehaviour, R4Activatable
{
    [Tooltip("Set this bool to false if you want this trap to start inactive")][SerializeField] private bool startOn = true;
    [Tooltip("After the player has touched the trap for this many seconds the trap will activate")][SerializeField] private float activationTime = 0.35f;
    [Tooltip("Time after the trap has been activated before it can activate again")][SerializeField] private float coolDownTime = 1f;
    [Tooltip("The image that shows when the platform is ready to use")][SerializeField] private Sprite activatedImage;
    [Tooltip("The image that shows when the platform has just been used or deactivated")][SerializeField] private Sprite deActivatedImage;
    [SerializeField] private SpriteRenderer spriteRenderer;
    private bool isActive;
    private float timeCounter;
    private bool onCooldown;
    private Coroutine cooldown;
    private AudioSource audioSource;

    public void Activate()
    {
        if(!isActive) 
        {
            isActive = true;
            //spriteRenderer.color = Color.red;
            spriteRenderer.sprite = activatedImage;
        }
    }

    public void Deactivate()
    {
        if(isActive) 
        {
            isActive = false;
            //spriteRenderer.color = Color.gray;
            spriteRenderer.sprite = deActivatedImage;   
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();  
        if (startOn) 
        {
            Activate();
        }
        else
        {
            Deactivate();
        }
    }



    private void OnCollisionStay2D(Collision2D collision)
    {
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        if (player != null && !onCooldown && player.frenzyManager.inRubberMode == false)
        {
            timeCounter += Time.deltaTime;
            if (timeCounter >= activationTime)
            {
                player.ToggleGravityFlip();
                audioSource.Play();
                timeCounter = 0;
                if(cooldown == null) cooldown = StartCoroutine(TrapCooldown());
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        timeCounter = 0;
    }



    IEnumerator TrapCooldown()
    {
        onCooldown = true;
        spriteRenderer.sprite = deActivatedImage;
        yield return new WaitForSeconds(coolDownTime);
        if(isActive) spriteRenderer.sprite = activatedImage;
        onCooldown = false;
        cooldown = null;
    }


}
