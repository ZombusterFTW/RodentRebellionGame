using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsSkip : MonoBehaviour
{
    [SerializeField] private float creditsWrapTime = 21f;
    [SerializeField] private UISceneChanger changer;
    [SerializeField] private string sceneToLoad = "MainMenu";
    void Start()
    {
        StartCoroutine(WaitForCreditsToFinish());
    }

    IEnumerator WaitForCreditsToFinish()
    {
        yield return new WaitForSecondsRealtime(creditsWrapTime);
        changer.ChangeSceneFast(sceneToLoad);
    }
}
