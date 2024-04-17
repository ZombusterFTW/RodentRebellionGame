using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartingCutscene : MonoBehaviour, R4Activatable
{
    [SerializeField] private GameObject[] cutsceneFrames;
    [SerializeField] private float timePerFrame = 1.5f;
    [SerializeField] private float frameFadeTime = 3f;
    [Tooltip("If false cutscene will activate on start")][SerializeField] private bool activatedByGameObject = false;
    [SerializeField] private bool activatesGameObjects = false;
    [SerializeField] private GameObject[] objectsToActivate;
    private bool isRunning = false;
    [SerializeField] private bool isEndingCutscene = false;


    // Start is called before the first frame update
    void Start()
    {
        
       if(!activatedByGameObject) StartCoroutine(CutsceneLoop());
    }

    IEnumerator CutsceneLoop() 
    {
        if(!isRunning)
        {
            PlayerController.instance.DisableControls(true);
            isRunning = true;
            int i = 0;
            //Frames must be in chronological order in the inspector for this to work properly
            foreach (GameObject frame in cutsceneFrames)
            {
                i++;
                frame.SetActive(true);
                frame.GetComponent<Image>().DOFade(1, frameFadeTime);
                yield return new WaitForSecondsRealtime(frameFadeTime);
                yield return new WaitForSecondsRealtime(timePerFrame);
            }

            if(!isEndingCutscene)
            {
                foreach (GameObject frame in cutsceneFrames)
                {
                    frame.SetActive(false);
                }
                cutsceneFrames[cutsceneFrames.Length - 1].gameObject.SetActive(true);
                cutsceneFrames[cutsceneFrames.Length - 1].gameObject.GetComponent<Image>().DOFade(0, frameFadeTime);
                yield return new WaitForSecondsRealtime(timePerFrame);
                PlayerController.instance.DisableControls(false);
                Destroy(gameObject, 10);
            }
            ActivateGameObjects();
        }
    }


    private void ActivateGameObjects()
    {
        if(activatesGameObjects)
        {
            foreach(GameObject objToActivate in objectsToActivate)
            {
                R4Activatable rInterface = objToActivate.GetComponent<R4Activatable>();
                if(rInterface != null)
                {
                    rInterface.Activate();
                }
            }
        }
    }

    public void Activate()
    {
        if(activatedByGameObject) 
        {
            StartCoroutine(CutsceneLoop());
        }
    }

    public void Deactivate()
    {
        Debug.Log("Starting Cutscene cannot be deactivated");
    }
}
