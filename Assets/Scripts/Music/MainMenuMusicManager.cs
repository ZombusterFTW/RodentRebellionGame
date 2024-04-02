using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class MainMenuMusicManager : MonoBehaviour
{
    public static MainMenuMusicManager Instance = null;
    public AudioMixerSnapshot fullOn;
    public AudioMixerSnapshot fullOff;
    public float audioTransitionTime = 1;
    private void Awake()
    {
        if(Instance == null )
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
            //SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            DestroyImmediate(this.gameObject);
        }
    }
    private void Start()
    {
        fullOn.TransitionTo(audioTransitionTime);
    }

    public void MainMenuMusicOnSceneLoad(string sceneToLoad)
    {
        Debug.Log(sceneToLoad);
        //if the scene loaded isn't a menu, we need to fade out the audio and destroy this object on fade completion.
        if(sceneToLoad == "MainMenu" || sceneToLoad == "Runs" || sceneToLoad == "Timewarp" || sceneToLoad == "Settings")
        {
            //if we get here we are still in the menu and we do nothing
            Debug.Log("Loaded a menu");
        }
        else
        {
            //Else we destroy the player object
            Debug.Log("Loaded a level, destroying menumusicobj");
            fullOff.TransitionTo(audioTransitionTime);
            Destroy(gameObject, audioTransitionTime + 0.05f);

        }
    }
}
