using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class StartingCutscene : MonoBehaviour
{
    [SerializeField] private GameObject[] cutsceneFrames;
    [SerializeField] private float timePerFrame = 1.5f;




    // Start is called before the first frame update
    void Start()
    {
        GameObject.FindObjectOfType<PlayerController>().DisableControls(true);
        StartCoroutine(CutsceneLoop());
    }

    IEnumerator CutsceneLoop() 
    {
        //Frames must be in chronological order in the inspector for this to work properly
        foreach (GameObject frame in cutsceneFrames) 
        {
            frame.SetActive(true);
            yield return new WaitForSecondsRealtime(timePerFrame);
        }


        foreach (GameObject frame in cutsceneFrames)
        {
            frame.SetActive(false);
        }
        cutsceneFrames[cutsceneFrames.Length - 1].gameObject.SetActive(true);
        cutsceneFrames[cutsceneFrames.Length - 1].gameObject.GetComponent<Image>().DOFade(0, 3);

        yield return new WaitForSecondsRealtime(timePerFrame);

        GameObject.FindObjectOfType<PlayerController>().DisableControls(false);
        Destroy(gameObject);
    }
}
