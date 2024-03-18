using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class SceneTransitionerManager : MonoBehaviour
{
    //This class allows for each scene transition to have a unique image and text!


    [SerializeField] Canvas canvas;
    [SerializeField] GameObject parentGameObject;
    public string[] gameHints;
    public string[] gameHintAnimations;
    public Animator transitionAnimator;
    public Animator transitionShowcaseAnimator;
    [SerializeField] private TextMeshProUGUI gameHintsText;
    private int lastScreen = 0;
    [SerializeField] private float sceneTransitionTime = 2f;
    private bool transitionInProgress = false;


    public static SceneTransitionerManager instance { get; private set; }
    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(parentGameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
            gameHintsText.text = "";
            canvas.enabled = true;
        }
        else if (instance != null && instance != this)
        {
            Debug.Log("Scene transitioner already exists. Deleting copy");
        }
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        //Play animator animation
        if (this != null && transitionAnimator != null)
        {
            transitionAnimator.Play("BoxWipe_End");
            transitionInProgress = false;
        }
        //gameHintsText.text = gameHints[Random.Range(0, gameHints.Length)];
    }

    public void StartTransition(string sceneToGo = "MainMenu")
    {
        if (!SceneTransitionerManager.instance.transitionAnimator.GetCurrentAnimatorStateInfo(0).IsName("BoxWipe_Start") /*&& SceneManager.GetActiveScene() != SceneManager.GetSceneByName(sceneToGo)*/ && !transitionInProgress)
        {
            transitionInProgress= true;
            //For demonstration purposes a text hint is directly linked to an animation that would play with it.
            //Screens are random but they cannot be the same twice in a row
            int index = Random.Range(0, gameHints.Length);
            while(index == lastScreen) 
            {
                index = Random.Range(0, gameHints.Length);
            }
            lastScreen = index;
            transitionAnimator.SetTrigger("Transition");
            gameHintsText.text = gameHints[index];
            //Commented until anims are added
            //if(gameHintAnimations[index] != null) transitionShowcaseAnimator.Play(gameHintAnimations[index], 1);
            StartCoroutine(SceneTransition(sceneToGo));
        }
       
    }



    IEnumerator SceneTransition(string sceneToGo)
    {
        //Future, include a pause menu with options. For now just return to the main menu if the player hits escape.
        Time.timeScale = 0;
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneToGo);
        asyncLoad.allowSceneActivation = false;
        yield return new WaitForSecondsRealtime(sceneTransitionTime);
        asyncLoad.allowSceneActivation = true;
        Time.timeScale = 1;
        transitionInProgress = false;
    }

    IEnumerator SceneTransition(int sceneToGo)
    {
        //Future, include a pause menu with options. For now just return to the main menu if the player hits escape.
        Time.timeScale = 0;
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneToGo);
        asyncLoad.allowSceneActivation = false;
        yield return new WaitForSecondsRealtime(sceneTransitionTime);
        asyncLoad.allowSceneActivation = true;
        Time.timeScale = 1;
        transitionInProgress = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartTransition(int buildIndex)
    {
        if (!SceneTransitionerManager.instance.transitionAnimator.GetCurrentAnimatorStateInfo(0).IsName("BoxWipe_Start") /*&& SceneManager.GetActiveScene() != SceneManager.GetSceneByBuildIndex(buildIndex) */&& !transitionInProgress)
        {
            transitionInProgress = true;
            //For demonstration purposes a text hint is directly linked to an animation that would play with it.
            //Screens are random but they cannot be the same twice in a row
            int index = Random.Range(0, gameHints.Length);
            while (index == lastScreen)
            {
                index = Random.Range(0, gameHints.Length);
            }
            lastScreen = index;
            transitionAnimator.SetTrigger("Transition");
            gameHintsText.text = gameHints[index];
            //Commented until anims are added
            //if(gameHintAnimations[index] != null) transitionShowcaseAnimator.Play(gameHintAnimations[index], 1);
            StartCoroutine(SceneTransition(buildIndex));
        }
    }
}
