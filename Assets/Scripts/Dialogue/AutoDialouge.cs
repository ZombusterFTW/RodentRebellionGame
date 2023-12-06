using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AutoDialouge : MonoBehaviour, R4Activatable
{
    //This class allows a dialouge segment to trigger without a player needing to go up to a character and interact with them.

    [SerializeField] private TextAsset inkJSON;
    [SerializeField] bool playDialougeAutomatically;
    [SerializeField] float dialougeDelay;
    [SerializeField] bool cleanupOnDialougeExit = false;
    [SerializeField] bool stopTime = false;
    [SerializeField] bool activatedByGameObject = false;
    [SerializeField] bool activatesItems = false;
    [SerializeField] GameObject[] itemsToActivate;



    // Start is called before the first frame update
    void Start()
    {
        if(playDialougeAutomatically) 
        {
            StartCoroutine(DialougeStartDelay());
        }
    }

    IEnumerator DialougeStartDelay()
    {
        //Wait for the delay and enter dialouge mode.
        yield return new WaitForSeconds(dialougeDelay);
        PlayDialougeScheduled();
    }

    public void PlayDialouge()
    {
        if(!playDialougeAutomatically && !activatedByGameObject)
        {
            GameObject.FindObjectOfType<PlayerController>().DisableControls(true);
            DialogueManager.GetInstance().EnterDialogueMode(inkJSON, gameObject, cleanupOnDialougeExit);
            if(stopTime)
            {
                DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 0, 0.25f);
                
            }
        }
    }
    private void PlayDialougeScheduled()
    {
            GameObject.FindObjectOfType<PlayerController>().DisableControls(true);
            DialogueManager.GetInstance().EnterDialogueMode(inkJSON, gameObject, cleanupOnDialougeExit);
            if (stopTime)
            {
                DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 0, 0.25f);

            }
    }

    public void Activate()
    {
        //Allow activating a button or picking something up to trigger dialouge
       if(activatedByGameObject)
        {
            GameObject.FindObjectOfType<PlayerController>().DisableControls(true);
            DialogueManager.GetInstance().EnterDialogueMode(inkJSON, gameObject, cleanupOnDialougeExit);
            if (stopTime)
            {
                DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 0, 0.25f);

            }
        }
    }

    public void Deactivate()
    {
        Debug.Log("cannot deactivate autodialouge");
    }

    private void OnDestroy()
    {
        //Activate items after the dialouge has completed. If its set.
        if(activatesItems)
        {
            foreach(var item in itemsToActivate)
            {
                item.GetComponent<R4Activatable>()?.Activate();
            }
        }
    }
}
