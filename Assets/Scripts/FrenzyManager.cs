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
    private bool frenzyActive = false;
    // Start is called before the first frame update
    void Start()
    {
        frenzyAmountCurrent = frenzyStartingValue;
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
        if(frenzyAmountCurrent >= frenzyMaxAmount && frenzyBarAnimation == null)
        {
            //Set bool to make stance transition impossible
            playerController.ActivateFrenzy(true);
            frenzyBarAnimation = StartCoroutine(FrenzyMeterCountdown());
        }
        else
        {
            Debug.Log("No frenzy juice or frenzy active");
        }
    }


    IEnumerator FrenzyMeterCountdown()
    {
        frenzyActive = true;
        DOTween.To(() => frenzyAmountCurrent, x => frenzyAmountCurrent = x, 0, frenzyMaxAmount);
        playerUIManager.UpdateFrenzyBar(frenzyAmountCurrent, frenzyMaxAmount);
        while (frenzyAmountCurrent > 0) 
        {
            Debug.Log(frenzyAmountCurrent);
            playerUIManager.UpdateFrenzyBar(frenzyAmountCurrent, frenzyMaxAmount);
            yield return null;
        }
        playerController.ActivateFrenzy(false);
        frenzyBarAnimation = null;
        frenzyActive = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!frenzyActive) AddToFrenzyMeter(Time.deltaTime);
    }
}
