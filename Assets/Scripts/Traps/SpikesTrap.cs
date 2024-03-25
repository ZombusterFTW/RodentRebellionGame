using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikesTrap : MonoBehaviour, R4Activatable, R4ActivatableTrap
{
    private PlayerController controller;
    [Tooltip("Set to true for spikes to activated and dangerous. If selfActivating is set to true this must be false.")] [SerializeField] private bool isActive = false;
    [Tooltip("Set to true for trap to activate when a player contacts it")][SerializeField] private bool selfActivating = true;
    [Tooltip("The delay before the trap activates. Only works in selfActivating trap mode.")][SerializeField] private float activationDelay = 0.35f;
    [Tooltip("How long the trap takes to reset if a player isn't touching it.")][SerializeField] private float isPlayerTouchingDelay = 0.75f;
    [Tooltip("How high the spikes will come out of the ground")][SerializeField] private float spikeHeight = 0.63f;
    [Tooltip("The speed the spikes will come out of the ground.")][SerializeField] private float spikeSpeed = 5f;
    [Tooltip("The trigger for the trap to activate in self activating mode.")][SerializeField] private BoxCollider2D trapTrigger;
    [Tooltip("The collider of the spikes. ")][SerializeField] private BoxCollider2D trapSpikes;
    public GameObject spikeEndPoint;
    public Animator spikeAnimator;
    BoxCollider2D playerCollider;
    private Vector3 startingPos;
    private Vector3 triggeredPos;
    private bool isTriggered = false;
    
    // Start is called before the first frame update
    void Awake()
    {
        startingPos = trapSpikes.gameObject.transform.position; 
        triggeredPos = spikeEndPoint.gameObject.transform.position;
        if (selfActivating) isActive = false;
    }

    private void Start()
    {
        controller = FindObjectOfType<PlayerController>();
        playerCollider = controller.GetPlayerCollider();
        

        if (isActive)
        {
            spikeAnimator.SetTrigger("Triggered");
            trapSpikes.gameObject.transform.position = triggeredPos;
            isTriggered = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(isTriggered)
        {
            if (playerCollider.IsTouching(trapSpikes)) 
            {
                Health playerHealth = controller.GetHealthComponent();
                playerHealth.SubtractFromHealth(playerHealth.GetCurrentHealth(), transform.position);
            }
        }
    }

    public void Activate()
    {
        if (!selfActivating)
        {
            isActive = true;
            FireTrap();
        }
    }

    public void Deactivate()
    {
        if(!selfActivating)
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


    private void OnCollisionEnter2D(Collision2D collision)
    {
        
    }







    private void FireTrap()
    {
        trapSpikes.enabled = true;
        isTriggered = true;
        spikeAnimator.SetTrigger("Triggered");
        StopCoroutine(MoveTrapSpikes(startingPos));
        StartCoroutine(MoveTrapSpikes(triggeredPos));
        //spriteRenderer.color = spikesActivatedColor;
    }

    private void ResetTrap()
    {
        isTriggered = false;
        trapSpikes.enabled = false;
        StopCoroutine(MoveTrapSpikes(triggeredPos));
        StartCoroutine(MoveTrapSpikes(startingPos));
        spikeAnimator.SetTrigger("Reset");
        //spriteRenderer.color = spikesDeactivatedColor;
    }

    IEnumerator TrapActivationProximity()
    {
        yield return new WaitForSeconds(activationDelay);
        //fire trap
        FireTrap();
        while (true)
        {
            yield return new WaitForSeconds(isPlayerTouchingDelay);
            //check if player is touching trap. if not deactivate spikes and break. If they are we loop and keep spikes up.
            if (playerCollider.IsTouching(trapTrigger) || playerCollider.IsTouching(trapSpikes)) continue;
            else break;
        }
        //reset trap
        ResetTrap();
        yield break;
    }


    IEnumerator MoveTrapSpikes(Vector3 posToMoveTo)
    {
        while(trapSpikes.gameObject.transform.position != posToMoveTo)
        {
            trapSpikes.gameObject.transform.position = Vector3.MoveTowards(trapSpikes.gameObject.transform.position, posToMoveTo, spikeSpeed * Time.deltaTime);
            yield return null;
        }
    }

    public void DealPlayerDamage(bool damage)
    {
        //throw new System.NotImplementedException();
    }
}




public interface R4ActivatableTrap
{
    public void TriggerTrap();
    public void DealPlayerDamage(bool damage);
}
