using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FrenzyManager : MonoBehaviour
{
    [Tooltip("How long the frenzy meter will last")][SerializeField] private float frenzyMaxAmount = 7.5f;
    private float frenzyStartingValue = 0f;
    [SerializeField] private float frenzyAmountCurrent;
    private PlayerController playerController;
    [SerializeField] private PlayerUI playerUIManager;
    private Coroutine frenzyBarAnimation;

    private Coroutine rubberModeBarAnimation;
    public bool frenzyActive { get; private set; } = false;

    public bool inRubberMode { get; private set; } = false;
    //Static reference so it can be referenced in any script easily.
    public static FrenzyManager instance;

    [SerializeField] private Canvas transitionCanvas;
    private Animator transitionCanvasAnimator;


    // Start is called before the first frame update
    void Awake()
    {
        if(instance == null) 
        {
            instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else if(instance != null && instance != this)
        {
            Debug.Log("A player with a frenzy manager already exists. Deleting duplicate");
           // Destroy(gameObject);
        }
        frenzyAmountCurrent = frenzyStartingValue;
        transitionCanvasAnimator = transitionCanvas.GetComponent<Animator>();


    }
    private void Start()
    {
        playerController = GetComponent<PlayerController>();
        playerUIManager = playerController.GetPlayerUI();
    }

    public void AddToFrenzyMeter(float percentage)
    {

        frenzyAmountCurrent = Mathf.Clamp(frenzyAmountCurrent + frenzyMaxAmount * Mathf.Abs(percentage), 0, frenzyMaxAmount);
        playerUIManager.UpdateFrenzyBar(frenzyAmountCurrent, frenzyMaxAmount);
    }

    public void SubtractFromFrenzyMeter(float percentage)
    {

        frenzyAmountCurrent = Mathf.Clamp(frenzyAmountCurrent - frenzyMaxAmount * Mathf.Abs(percentage), 0, frenzyMaxAmount);
        playerUIManager.UpdateFrenzyBar(frenzyAmountCurrent, frenzyMaxAmount);
    }

    public void UpgradeFrenzyMeter(float percentage)
    {
        frenzyMaxAmount += frenzyMaxAmount * Mathf.Abs(percentage);
        playerUIManager.UpdateFrenzyBar(frenzyAmountCurrent, frenzyMaxAmount);
    }

    public void ActivateFrenzyMeter()
    {
        if(frenzyAmountCurrent >= frenzyMaxAmount && frenzyBarAnimation == null && !inRubberMode)
        {
            //Set bool to make stance transition impossible
            playerController.ActivateFrenzy(true);
            frenzyBarAnimation = StartCoroutine(FrenzyMeterCountdown());
        }
        else
        {
            Debug.Log("No frenzy juice, frenzy active, or in Rubber mode");
        }
    }


    public void ToggleRubberMode()
    {
        if (frenzyAmountCurrent > 0 && rubberModeBarAnimation == null && !inRubberMode)
        {
            //Player cannot be in rubber mode while in rage mode, so we check if the player is in rage mode. If they are we kick them out of it.
            if(frenzyActive &&  frenzyBarAnimation != null)
            {
                StopCoroutine(frenzyBarAnimation);
                frenzyActive = false;
                playerController.ActivateFrenzy(false);
            }
            //We change to rubber mode here
            rubberModeBarAnimation = StartCoroutine(RubberModeMeterCountdown());
            Debug.Log("Entered Rubber Mode");
            transitionCanvasAnimator.Play("WorldStateTransition");
        }

        else if(frenzyAmountCurrent > 0 && rubberModeBarAnimation != null && inRubberMode)
        {
            //If we get here the player is in rubber mode and attempting to return to rat mode while they still have charge in their meter.
            StopCoroutine(rubberModeBarAnimation);
            rubberModeBarAnimation = null;
            inRubberMode = false;
            Debug.Log("Exited Rubber Mode");
            transitionCanvasAnimator.Play("WorldStateTransitionReverse");
        }
        
        else
        {
            Debug.Log("No frenzy juice to enter rubber mode");
        }
    }


    IEnumerator FrenzyMeterCountdown()
    {
        frenzyActive = true;
        //Frenzy bar changed to use delta time, so picking up new collectibles will increase the meters amount during frenzy or rubber mode.
       // DOTween.To(() => frenzyAmountCurrent, x => frenzyAmountCurrent = x, 0, frenzyMaxAmount);
       // playerUIManager.UpdateFrenzyBar(frenzyAmountCurrent, frenzyMaxAmount);
        while (frenzyAmountCurrent > 0) 
        {
            //Debug.Log(frenzyAmountCurrent);
            frenzyAmountCurrent -= Time.deltaTime;
            playerUIManager.UpdateFrenzyBar(frenzyAmountCurrent, frenzyMaxAmount);
            yield return null;
        }
        playerController.ActivateFrenzy(false);
        frenzyBarAnimation = null;
        frenzyActive = false;
        Debug.Log("Exited Frenzy mode, ran out of juice");
    }

    IEnumerator RubberModeMeterCountdown()
    {
        inRubberMode = true;
        while (frenzyAmountCurrent > 0)
        {
            //Debug.Log(frenzyAmountCurrent);
            frenzyAmountCurrent -= Time.deltaTime * 0.1f;
            playerUIManager.UpdateFrenzyBar(frenzyAmountCurrent, frenzyMaxAmount);
            yield return null;
        }
        rubberModeBarAnimation = null;
        inRubberMode = false;
        Debug.Log("Exited Rubber Mode, Ran out of juice.");
        transitionCanvasAnimator.Play("WorldStateTransitionReverse");
    }



    // Update is called once per frame
    void Update()
    {
        //For testing 
        if (!frenzyActive && !inRubberMode) AddToFrenzyMeter(0.01f*Time.deltaTime);
    }
}
