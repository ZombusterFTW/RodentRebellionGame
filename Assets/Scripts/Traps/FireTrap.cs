using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireTrap : MonoBehaviour, R4Activatable, R4ActivatableTrap
{
    private PlayerController controller;
    [Tooltip("Set to true for the trap to activated and dangerous. If selfActivating is set to true this must be false.")][SerializeField] private bool isActive = false;
    [Tooltip("Set to true for trap to activate when a player contacts it")][SerializeField] private bool selfActivating = true;
    [Tooltip("The delay before the trap activates. Only works in selfActivating trap mode.")][SerializeField] private float activationDelay = 0.35f;
    [Tooltip("How long the trap takes to reset if a player isn't touching it.")][SerializeField] private float isPlayerTouchingDelay = 0.75f;
    [Tooltip("How much damage the trap will do per second")][SerializeField] private float dmgPerTick = 0.75f;
    [Tooltip("The trigger for the trap to activate in self activating mode.")][SerializeField] private BoxCollider2D trapTrigger;
    [Tooltip("The collider of the fire. ")][SerializeField] private BoxCollider2D trapFire;
    //[Tooltip("The color of the fire when is isnt activated.")][SerializeField] private Color fireDeactivatedColor = Color.green;
    //[Tooltip("The color of the fire when is isnt activated.")][SerializeField] private Color fireActivatedColor = Color.red;
    [SerializeField] SpriteRenderer spriteRenderer;
    BoxCollider2D playerCollider;
    private bool isTriggered = false;
    private bool isDamaging = false;
    [SerializeField] private Animator fireTrapAnimator;

    // Start is called before the first frame update
    void Awake()
    {
        if (selfActivating) isActive = false;
    }

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();    
        controller = FindObjectOfType<PlayerController>();
        playerCollider = controller.GetPlayerCollider();
        //spriteRenderer.color = fireDeactivatedColor;
        fireTrapAnimator.Play("FireTrap_Idle");
        if (isActive)
        {
            //spriteRenderer.color = fireActivatedColor;
            isTriggered = true;
            fireTrapAnimator.Play("FireTrap_Active");
        }
        else trapFire.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Activate()
    {
        if (!selfActivating)
        {
            isActive = true;
            FireTrapActivation();
        }
    }

    public void Deactivate()
    {
        if (!selfActivating)
        {
            isActive = false;
            ResetTrap();
        }

    }

    public void TriggerTrap()
    {
        if (selfActivating)
        {
            //Trigger trap by player hitting prox trigger. 
            Debug.Log("Trap triggered");
            StopCoroutine(TrapActivationProximity());
            StartCoroutine(TrapActivationProximity());
        }
        else
        {
            Debug.Log("Trap triggered by button or other activator.");

        }

    }


    public void DealPlayerDamage(bool damage)
    {
        if (isTriggered)
        {
            if (damage)
            {
                if (playerCollider.IsTouching(trapFire) && !isDamaging)
                {
                    isDamaging = true;
                    StartCoroutine(DamageWait());
                    //controller.PlayHurt();
                }
            }
            else
            {
                //controller.StopHurt();
                StopCoroutine(DamageWait());
            }
        }
    }

    IEnumerator DamageWait()
    {
        yield return new WaitForFixedUpdate();
        Health playerHealth = controller.GetHealthComponent();
        playerHealth.SubtractFromHealth(dmgPerTick, Vector2.zero);
        isDamaging = false;
    }


    private void FireTrapActivation()
    {
        trapFire.enabled = true;
        isTriggered = true;
        //spriteRenderer.color = fireActivatedColor;
        fireTrapAnimator.Play("FireTrap_Active");
    }

    private void ResetTrap()
    {
        isTriggered = false;
        trapFire.enabled = false;
        //spriteRenderer.color = fireDeactivatedColor;
        fireTrapAnimator.Play("FireTrapEnd");
    }


    IEnumerator TrapActivationProximity()
    {
        yield return new WaitForSeconds(activationDelay);
        //fire trap
        FireTrapActivation();
        while (true)
        {
            yield return new WaitForSeconds(isPlayerTouchingDelay);
            //check if player is touching trap. if not deactivate spikes and break. If they are we loop and keep spikes up.
            if (playerCollider.IsTouching(trapTrigger) || playerCollider.IsTouching(trapFire)) continue;
            else break;
        }
        //reset trap
        ResetTrap();
        yield break;
    }


}
