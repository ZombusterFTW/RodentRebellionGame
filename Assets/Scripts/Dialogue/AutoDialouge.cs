using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDialouge : MonoBehaviour
{
    //This class allows a dialouge segment to trigger without a player needing to go up to a character and interact with them.

    [Header("Ink JSON")]
    [SerializeField] private TextAsset inkJSON;
    [SerializeField] bool playDialougeAutomatically;
    [SerializeField] float dialougeDelay;
    [SerializeField] bool cleanupOnDialougeExit = false;

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
        if(!playDialougeAutomatically)
        {
            GameObject.FindObjectOfType<PlayerController>().DisableControls(true);
            DialogueManager.GetInstance().EnterDialogueMode(inkJSON, gameObject, cleanupOnDialougeExit);
        }
    }
    private void PlayDialougeScheduled()
    {
            GameObject.FindObjectOfType<PlayerController>().DisableControls(true);
            DialogueManager.GetInstance().EnterDialogueMode(inkJSON, gameObject, cleanupOnDialougeExit);
    }
}
