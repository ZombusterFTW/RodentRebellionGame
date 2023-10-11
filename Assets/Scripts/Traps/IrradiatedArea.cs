using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class IrradiatedArea : MonoBehaviour,R4Activatable, R4ActivatableTrap
{


    //Slow player down and make them receive small regen overtime
    [Tooltip("Player will recieve health every interval set here. In Seconds")] public float healDelay = 2.0f;
    [Tooltip("Health to heal on each interval")] public float hpToHeal = 1.0f;
    [Tooltip("Set to true for irradiated area to be on by default.")][SerializeField] private bool isActive = true;
    [Tooltip("Set to the percentage the player's movement will be slowed down by")][SerializeField] private float movementSlowPercentage = 0.66f;
    private bool isTriggered = false;
    private PlayerController controller;
    private SpriteRenderer sprite;
    // Start is called before the first frame update
    void Start()
    {
        controller = FindObjectOfType<PlayerController>();
        sprite = GetComponent<SpriteRenderer>();    
        if (!isActive) sprite.enabled = false;
        isTriggered = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }






    public void DealPlayerDamage(bool damage)
    {
        if (isTriggered && isActive)
        {
            if (damage)
            {
                if (!isTriggered)
                {
                    Debug.Log("triggered");
                    isTriggered = true;
                    StartCoroutine(HealOverTime());
                    controller.GetComponent<R4MovementComponent>().SetMovementSpeed(controller.GetComponent<R4MovementComponent>().GetMovementSpeed() * movementSlowPercentage);
                }
            }
            else
            {
                isTriggered = false;
                StopCoroutine(HealOverTime());
                controller.GetComponent<R4MovementComponent>().SetMovementSpeed(controller.GetComponent<R4MovementComponent>().GetMovementSpeed());
            }
        }
    }


    IEnumerator HealOverTime()
    {
        while (isTriggered) 
        {
            yield return new WaitForSeconds(healDelay);
            Debug.Log("add to health");
            Health playerHealth = controller.GetHealthComponent();
            playerHealth.AddToHealth(hpToHeal);
        }

    }


    public void Activate()
    {
        isActive = true;
        sprite.enabled = true;
    }

    public void Deactivate()
    {
        isActive = false;
        sprite.enabled = false;
    }

    public void TriggerTrap()
    {
        if (isActive)
        {
            //Trigger trap by player hitting prox trigger. 
            Debug.Log("Trap triggered");
        }
        else
        {
            Debug.Log("Trap triggered by button or other activator.");

        }
    }
}
