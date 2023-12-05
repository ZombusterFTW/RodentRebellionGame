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



    [SerializeField] GameObject parentGameObject;
    public string[] gameHints;
    public string[] gameHintAnimations;
    public Animator transitionAnimator;
    public Animator transitionShowcaseAnimator;
    [SerializeField] private TextMeshProUGUI gameHintsText;
    private int lastScreen = 0;
    public static SceneTransitionerManager instance { get; private set; }
    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(parentGameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
            gameHintsText.text = "";
        }
        else if (instance != null && instance != this)
        {
            Debug.Log("Scene transitioner already exists. Deleting clone");
            DestroyImmediate(gameObject);
        }
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        //Play animator animation
        if(this != null && transitionAnimator != null) transitionAnimator.Play("BoxWipe_End");
        //gameHintsText.text = gameHints[Random.Range(0, gameHints.Length)];
    }

    public void StartTransition()
    {
        if (!SceneTransitionerManager.instance.transitionAnimator.GetCurrentAnimatorStateInfo(0).IsName("BoxWipe_Start"))
        {
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
            if(gameHintAnimations[index] != null) transitionShowcaseAnimator.Play(gameHintAnimations[index]);
        }
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
