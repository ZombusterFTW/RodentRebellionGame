using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour
{
    //switch can be activated by things that damage it. Add support to check damage type. This way a special switch could only be activated by the laser gun etc. 
    [Tooltip("The list of items to Activate.")][SerializeField] private List<GameObject> itemsToActivate = new List<GameObject>();
    [Tooltip("Set to true if you want the switch to start on")][SerializeField] private bool startOn = false;
    [Tooltip("The color of the switch when it is Activated.")][SerializeField] private Color activatedColor = Color.red;
    [Tooltip("The color of the switch when it is Deactivated.")][SerializeField] private Color deactivatedColor = Color.white;
    [Tooltip("If attacktyperequired is set to true, this switch will only activate when hit by a specific attack")][SerializeField] private bool attackTypeRequired = false;
    [Tooltip("If attacktyperequired is set to true, this switch will only activate when hit by a specific attack. Note that switches cannot be activated by ground pounds")][SerializeField] private PlayerAttackType requiredAttackType;



    //delay before the switch can be triggered again
    private float switchDelayTime = 0.5f;
    private bool switchActivated = false;
    private bool switchDelayActive = false;
    SpriteRenderer spriteRenderer;
    //private AudioClip switchPullSound;
    //private AudioSource switchAudioSource;


    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
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
        //If switch is only activatiable with a certain attack prevent activation if the wrong attack is used.
        if (attackTypeRequired)
        { 
            if(inType != requiredAttackType)
            {
                return;
            }
        }
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
        //Prevent switch reactivation for X seconds. 
        switchDelayActive = true;
        yield return new WaitForSeconds(switchDelayTime);
        switchDelayActive = false;
    }

    //The code under this directly pulls from the button script. The underlying logic is nearly identical. Just need to add support for it to flip between 2 different sprites or play an animation. 
    void ActivateButton()
    {
        switchActivated = true;
        spriteRenderer.color = activatedColor;
        ActivateItems();
    }

    void DeactivateButton()
    {
        DeactivateItems();
        spriteRenderer.color = deactivatedColor;
        switchActivated = false;
    }

    void ActivateItems()
    {
        //Ensure that the list doesn't contain a null item.
        itemsToActivate.RemoveAll(x => x == null);
        foreach (var item in itemsToActivate)
        {
            //Check if item has the activatable interface and if so, we activate.
            if (item.GetComponent<R4Activatable>() != null)
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
            if (item.GetComponent<R4Activatable>() != null)
            {
                item.GetComponent<R4Activatable>().Deactivate();
            }
        }
    }
}
