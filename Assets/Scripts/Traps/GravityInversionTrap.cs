using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityInversionTrap : MonoBehaviour, R4Activatable
{
    [Tooltip("Set this bool to false if you want this trap to start inactive")][SerializeField] private bool startOn = true;
    [Tooltip("After the player has touched the trap for this many seconds the trap will activate")][SerializeField] private float activationTime = 0.35f;
    [Tooltip("Time after the trap has been activated before it can activate again")][SerializeField] private float coolDownTime = 1f;
    private SpriteRenderer spriteRenderer;
    private bool isActive;
    private float timeCounter;
    private bool onCooldown;
    private Coroutine cooldown;

    public void Activate()
    {
        if(!isActive) 
        {
            isActive = true;
            spriteRenderer.color = Color.red;
        }
    }

    public void Deactivate()
    {
        if(isActive) 
        {
            isActive = false;
            spriteRenderer.color = Color.gray;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if(startOn) 
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
        if (player != null && !onCooldown)
        {
            timeCounter += Time.deltaTime;
            if (timeCounter >= activationTime)
            {
                player.ToggleGravityFlip();
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
        spriteRenderer.color = Color.gray;
        yield return new WaitForSeconds(coolDownTime);
        if(isActive) spriteRenderer.color = Color.red;
        onCooldown = false;
        cooldown = null;
    }


}
