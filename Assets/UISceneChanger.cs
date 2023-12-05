using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISceneChanger : MonoBehaviour
{
    public void CloseApplication()
    {
        Application.Quit();
    }

    public void ChangeScene(string sceneName)
    {
        SceneTransitionerManager.instance.StartTransition(sceneName);
    }
}
