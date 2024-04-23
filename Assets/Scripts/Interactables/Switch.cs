using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour
{
    //switch can be activated by things that damage it. Add support to check damage type. This way a special switch could only be activated by the laser gun etc. 
    [Tooltip("The list of items to Activate.")][SerializeField] private List<GameObject> itemsToActivate = new List<GameObject>();
    [Tooltip("Set to true if you want the switch to start on")][SerializeField] private bool startOn = false;
    [Tooltip("If attacktyperequired is set to true, this switch will only activate when hit by a specific attack")][SerializeField] private bool attackTypeRequired = false;
    [Tooltip("If attacktyperequired is set to true, this switch will only activate when hit by a specific attack. Note that switches cannot be activated by ground pounds")][SerializeField] private PlayerAttackType requiredAttackType;
    [Tooltip("If set to true, once interacted with this switch can't be used again")][SerializeField] bool switchIsUsedOnce = false;
    [Tooltip("The animator used for the switch.")][SerializeField] Animator animator;
    //delay before the switch can be triggered again
    private float switchDelayTime = 0.25f;
    private bool switchActivated = false;
    private bool switchDelayActive = false;
    private AudioSource switchAudioSource;
    [SerializeField] private AudioClip switchOn;
    [SerializeField] private AudioClip switchOff;
    [SerializeField] private bool disableActivationByLaser = false;
    [SerializeField] private bool isFinalBossTerminal = false;
    //private AudioClip switchPullSound;
    //private AudioSource switchAudioSource;


    // Start is called before the first frame update
    void Start()
    {
        switchAudioSource = GetComponent<AudioSource>();
        if (startOn)
        {
            switchActivated = true; 
            ActivateButton();
        }
        else
        {
            switchActivated = false;
            DeactivateButton();
        }
    }

    public void ToggleSwitch(PlayerAttackType inType = PlayerAttackType.StandardAttack)
    {
        Debug.Log("hit switch");
        //If switch is only activatiable with a certain attack prevent activation if the wrong attack is used.
        if (attackTypeRequired)
        {
            if (inType != requiredAttackType)
            {
                return;
            }
        }
        else if (disableActivationByLaser && inType == PlayerAttackType.LaserBlast) return;
        //The switch delay will prevent a player from activating the switch multiple times in a single frame. 
        if(!switchDelayActive)
        {
            if (switchActivated) DeactivateButton();
            else ActivateButton();
            StartCoroutine(SwitchReactivationDelay());
        }
    }


    IEnumerator SwitchReactivationDelay()
    {
        switchDelayActive = true;
        if (!switchIsUsedOnce)
        {
            //Prevent switch reactivation for X seconds. 
            yield return new WaitForSeconds(switchDelayTime);
            switchDelayActive = false;
        }
        else
        {
            //If the switch can only be used once we fall out of the coroutine and end the function.
            yield return null;
            this.enabled = false;
        }
    }

    //The code under this directly pulls from the button script. The underlying logic is nearly identical. Just need to add support for it to flip between 2 different sprites or play an animation. 
    void ActivateButton()
    {
        switchAudioSource.clip = switchOn;
        switchAudioSource.Play();
        switchActivated = true;
        if(isFinalBossTerminal) animator.Play("TerminalDestroyed");
        else animator.Play("FloorSwitchOn");
        ActivateItems();
    }

    void DeactivateButton()
    {
        switchAudioSource.clip = switchOff;
        switchAudioSource.Play();
        DeactivateItems();
        if (isFinalBossTerminal) animator.Play("TerminalOn");
        else animator.Play("FloorSwitchOff");
        switchActivated = false;
    }

    void ActivateItems()
    {
        //Ensure that the list doesn't contain a null item.
        itemsToActivate.RemoveAll(x => x == null);
        foreach (var item in itemsToActivate)
        {
            //Check if item has the activatable interface and if so, we activate.
            if (item != null && item.GetComponent<R4Activatable>() != null)
            {
                item.GetComponent<R4Activatable>().Activate();
            }
        }
    }


    void DeactivateItems()
    {
        //Ensure that the list doesn't contain a null item.
        itemsToActivate.RemoveAll(x => x == null);
        foreach (var item in itemsToActivate)
        {
            //Check if item has the activatable interface and if so, we deactivate.
            if (item != null && item.GetComponent<R4Activatable>() != null)
            {
                item.GetComponent<R4Activatable>().Deactivate();
            }
        }
    }
}
