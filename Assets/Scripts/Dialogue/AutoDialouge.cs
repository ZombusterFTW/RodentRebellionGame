using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class AutoDialouge : MonoBehaviour, R4Activatable
{
    //This class allows a dialouge segment to trigger without a player needing to go up to a character and interact with them.

    [Tooltip("The Ink JSON file that will player when the player overlaps the trigger.")][SerializeField] private TextAsset inkJSON;
    [Tooltip("If true the dialouge will begin at Start after dialougeDelay seconds.")][SerializeField] bool playDialougeAutomatically;
    [Tooltip("The time before dialouge plays. Only taken into account if playDialougeAutomatically is set to true.")][ SerializeField] float dialougeDelay;
    [Tooltip("If true this object will be destroyed on dialouge exit.")][SerializeField] bool cleanupOnDialougeExit = false;
    //Dialouge has been changed to always pause time since player movement is disabled during dialouge.
    bool stopTime = true;
    [Tooltip("Set this to true if you want this dialouge to be activated by a button or other R4Activator means.")][SerializeField] bool activatedByGameObject = false;
    [Tooltip("Set this to true if you want this dialouge to activate any R4 Activatable items on dialouge completion.")][SerializeField] bool activatesItems = false;
    [Tooltip("The list of R4 activatable items that will be activated if activatesItems is set to true.")][SerializeField] GameObject[] itemsToActivate;



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
                //DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 0, 0.25f);
                Time.timeScale = 0;
            }
        }
    }
    private void PlayDialougeScheduled()
    {
            GameObject.FindObjectOfType<PlayerController>().DisableControls(true);
            DialogueManager.GetInstance().EnterDialogueMode(inkJSON, gameObject, cleanupOnDialougeExit);
            if (stopTime)
            {
                //DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 0, 0.25f);
                Time.timeScale = 0f;
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
                //DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 0, 0.25f);
                Time.timeScale = 0f;
            }
        }
    }

    public void Deactivate()
    {
        Debug.Log("cannot deactivate autodialouge");
    }

    private void OnDestroy()
    {
        if(this != null)
        {
            //Activate items after the dialouge has completed. If its set.
            if (activatesItems)
            {
                if(itemsToActivate != null)
                {
                    foreach (var item in itemsToActivate)
                    {
                        if (item != null && item.GetComponent<R4Activatable>() != null) item.GetComponent<R4Activatable>()?.Activate();
                    }
                }
            }
        }
        
    }
}
