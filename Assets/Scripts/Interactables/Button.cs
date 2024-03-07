using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    [Tooltip("If the button will use toggle behavior. The itemActivateTime must be set to -1 for this to wor;")][SerializeField] private bool toggleBehavior = true;
    [Tooltip("How long the button will activate what its connected to. Set value to negative 1 for indefinite.")][SerializeField] float itemActivateTime = 5f;
    [Tooltip("The list of items to Activate.")][SerializeField] private List<GameObject> itemsToActivate = new List<GameObject>();
    [Tooltip("Wether or not the player must be groundpounding to activate the button.")] private bool groundPoundNeeded = true;
    [Tooltip("Set to true if you want the button to start on")][SerializeField] private bool startOn = false;
    [Tooltip("The color of the button when it is Activated.")][SerializeField] private Color activatedColor = Color.red;
    [Tooltip("The color of the button when it is Deactivated.")][SerializeField] private Color deactivatedColor = Color.white;

    private SpriteRenderer spriteRenderer;
    private bool buttonActivated = false;
    private bool buttonDelay = false;
    private bool toggleBehavior_Game = false;
    private float buttonDelayTime = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        //Prevent the possibility of toggle behavior and button delay being active at the same time.
        if(itemActivateTime < 0 && toggleBehavior) toggleBehavior_Game = true;
        if(startOn)
        {
            ActivateButton();
        }
        else
        {
            DeactivateButton();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void ActivateButton()
    {
        StopCoroutine(ButtonToggleDelay());
        StartCoroutine(ButtonToggleDelay());
        buttonActivated = true;
        spriteRenderer.color = activatedColor;
        if (itemActivateTime > 0) StartCoroutine(ButtonActivated());
        else ActivateItems();
    }

    void DeactivateButton()
    {
        StopCoroutine(ButtonToggleDelay());
        StartCoroutine(ButtonToggleDelay());
        DeactivateItems();
        spriteRenderer.color = deactivatedColor;
        buttonActivated = false;
    }

    IEnumerator ButtonToggleDelay()
    {
        buttonDelay = true;
        yield return new WaitForSeconds(buttonDelayTime);
        buttonDelay = false;    
    }




    IEnumerator ButtonActivated() 
    {
        ActivateItems();
        yield return new WaitForSeconds(itemActivateTime);
        DeactivateButton();
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



    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(!buttonActivated && !buttonDelay)
        {
            if (groundPoundNeeded)
            {
                //Player must ground pound to activate button.
                if (collision.gameObject.GetComponent<PlayerController>() != null && collision.gameObject.GetComponent<PlayerController>().GetIsGroundPounding())
                {
                    //player activated button
                    Debug.Log("Activate button");
                    ActivateButton();
                }
            }
            else
            {
                //Player must just touch the button to activate it.
                if (collision.gameObject.GetComponent<PlayerController>() != null)
                {
                    //player activated button
                    Debug.Log("Activate button");
                    ActivateButton();
                }
            }
        }
        else if (buttonActivated && toggleBehavior_Game && !buttonDelay)
        {
            if (groundPoundNeeded)
            {
                //Player must ground pound to activate button.
                if (collision.gameObject.GetComponent<PlayerController>() != null && collision.gameObject.GetComponent<PlayerController>().GetIsGroundPounding())
                {
                    //player activated button
                    Debug.Log("Deactivate button");
                    DeactivateButton();
                }
            }
            else
            {
                //Player must just touch the button to activate it.
                if (collision.gameObject.GetComponent<PlayerController>() != null)
                {
                    //player activated button
                    Debug.Log("Deactivate button");
                    DeactivateButton();
                }
            }
        }
    }
}
