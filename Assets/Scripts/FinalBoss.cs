using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class FinalBoss : MonoBehaviour, R4Activatable, OneHitHealthEnemy
{
    /// <summary>
    /// ////Temp final boss flow. Joe speaks to whiskers and they have an argument with whiskers begging joe to reconsider. This dialouge activates this script, 
    /// on activation the shield appears over whiskers and players must find three switches that disable it.
    /// upon the activation of 3 switches the player can hit whiskers once resulting in his death. This script is an activator that can activate dialouge on its death.
    /// </summary>
    [Tooltip("The array of switches that must be activated to deactivate the shield.")][SerializeField] private GameObject[] switchesToActivate;
    [Tooltip("This is the dialouge object or objects that will be activated on boss death")][SerializeField] private GameObject[] itemsToActivateOnDeath;
    [Tooltip("This the sprite renderer that will represent Dr Whiskers in rat mode")][SerializeField] private SpriteRenderer drWhiskersSpriteRenderer;
    [SerializeField] private GameObject shield;
    private bool bossActivated = false;
    private int currentSwitchesActivatedCount = 0;
    private bool allSwitchesActivated = false;
    private CapsuleCollider2D whiskersCapsuleCollider;

   
    // Start is called before the first frame update
    void Start()
    {
        shield.GetComponent<SpriteRenderer>().DOFade(0, 0);
        shield.SetActive(false);
        whiskersCapsuleCollider = GetComponent<CapsuleCollider2D>();
        foreach (GameObject item in switchesToActivate)
        {
            if (item.GetComponent<Switch>() != null)
            {
                item.GetComponent<Collider2D>().enabled = false;
            }
        }
    }

    



    public void Activate()
    {
        if(!allSwitchesActivated) 
        {
            //This case is when the boss is activated for the first time. Any other activations are treated as the switches needed to deactivate the shield. 
            if (!bossActivated)
            {
                Debug.Log("Activated the whiskers boss for the first time");
                shield.SetActive(true);
                foreach (GameObject item in switchesToActivate)
                {
                    if (item.GetComponent<Switch>() != null)
                    {
                        item.GetComponent<Collider2D>().enabled = true;
                    }
                }
                shield.GetComponent<SpriteRenderer>().DOFade(0.25f, 0.5f);
                bossActivated = true;
            }
            else
            {
                Debug.Log("Activated a switch");
                currentSwitchesActivatedCount++;
                allSwitchesActivated = currentSwitchesActivatedCount >= switchesToActivate.Length;
                if(allSwitchesActivated)
                {
                    Debug.Log("All switches activated");
                    shield.SetActive(false);
                }
            }
        }
    }

    public void Deactivate()
    {
        Debug.Log("Cannot deactivate the final boss");
    }

    public void OnOneHitEnemyDeath()
    {
        if(allSwitchesActivated)
        {
            Debug.Log("Boss officially dead. Play dialouge now");
            drWhiskersSpriteRenderer.DOFade(0, 1);
            whiskersCapsuleCollider.enabled = false;
            ActivateItemsOnDeath();
            Destroy(gameObject, 1.5f);
        }
    }

    private void ActivateItemsOnDeath()
    {
        foreach(GameObject item in itemsToActivateOnDeath) 
        {
            if(item.GetComponent<R4Activatable>() != null)
            {
                item.GetComponent<R4Activatable>().Activate();
            }
        }
    }

}
