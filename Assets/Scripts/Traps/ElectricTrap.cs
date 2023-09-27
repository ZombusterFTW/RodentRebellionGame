using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricTrap : MonoBehaviour, R4Activatable, R4ActivatableTrap
{
    private PlayerController controller;
    [Tooltip("Set to true for the trap to activated and dangerous. If selfActivating is set to true this must be false.")][SerializeField] private bool isActive = false;
    [Tooltip("Set to true for trap to activate when a player contacts it")][SerializeField] private bool selfActivating = true;
    [Tooltip("Set to true for this trap to slow the player(or others) down when they are in it.")][SerializeField] private bool slowsVictim = true;
    [Tooltip("The percentage of speed to slow the victim down by. 80% speed is .80f")][SerializeField] private float movementSlowPercentage = 0.55f;
    [Tooltip("The delay before the trap activates. Only works in selfActivating trap mode.")][SerializeField] private float activationDelay = 0.35f;
    [Tooltip("How long the trap takes to reset if a player isn't touching it.")][SerializeField] private float isPlayerTouchingDelay = 0f;
    [Tooltip("How much damage the trap will do per second")][SerializeField] private float dmgPerTick = 0.75f;
    [Tooltip("The trigger for the trap to activate in self activating mode.")][SerializeField] private BoxCollider2D trapTrigger;
    [Tooltip("The collider of the electricity. ")][SerializeField] private BoxCollider2D trapElectric;
    [Tooltip("The color of the electricity when is isnt activated.")][SerializeField] private Color electricityDeactivatedColor = Color.green;
    [Tooltip("The color of the electricity when is isnt activated.")][SerializeField] private Color electricityActivatedColor = Color.blue;
    [SerializeField] SpriteRenderer spriteRenderer;
    BoxCollider2D playerCollider;
    private bool isTriggered = false;
    private bool isDamaging = false;

    // Start is called before the first frame update
    void Awake()
    {
        if (selfActivating) isActive = false;
    }

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        controller = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        playerCollider = controller.GetPlayerCollider();
        spriteRenderer.color = electricityDeactivatedColor;

        if (isActive)
        {
            spriteRenderer.color = electricityActivatedColor;
            isTriggered = true;
        }
        else trapElectric.enabled = false;
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
                if (playerCollider.IsTouching(trapElectric) && !isDamaging)
                {
                    isDamaging = true;
                    StartCoroutine(DamageWait());
                    if(slowsVictim) controller.GetComponent<R4MovementComponent>().SetMovementSpeed(controller.GetComponent<R4MovementComponent>().GetMovementSpeed()*movementSlowPercentage);
                }
            }
            else
            {
                StopCoroutine(DamageWait());
                controller.GetComponent<R4MovementComponent>().SetMovementSpeed(controller.GetComponent<R4MovementComponent>().GetMovementSpeed());
            }
        }
    }

    IEnumerator DamageWait()
    {
        yield return new WaitForFixedUpdate();
        //Slow player or character down.
        Health playerHealth = controller.GetHealthComponent();
        playerHealth.SubtractFromHealth(dmgPerTick);
        isDamaging = false;
    }


    private void FireTrapActivation()
    {
        trapElectric.enabled = true;
        isTriggered = true;
        spriteRenderer.color = electricityActivatedColor;
    }

    private void ResetTrap()
    {
        isTriggered = false;
        trapElectric.enabled = false;
        spriteRenderer.color = electricityDeactivatedColor;
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
            if (playerCollider.IsTouching(trapTrigger) || playerCollider.IsTouching(trapElectric)) continue;
            else break;
        }
        //reset trap
        ResetTrap();
        yield break;
    }
}
