using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MatchTimer : MonoBehaviour
{
    public TextMeshProUGUI timerText1;
    public TextMeshProUGUI timerText2;
    public TextMeshProUGUI timerText3;
    public TextMeshProUGUI levelName;
    private float elapsedTime;
    DialogueManager dialogueManager;

    // Start is called before the first frame update
    void Start()
    {
        dialogueManager = DialogueManager.GetInstance();
        SceneManager.sceneLoaded += OnSceneLoaded;
        levelName.text = SceneManager.GetActiveScene().name;
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if(this != null)
        {
            //We enter a new scene so we reset the time.
            elapsedTime = 0;
            //Set level name to the new scene
            levelName.text = arg0.name;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        //Time will no longer accrue on the timer during dialouge
        if (!GameObject.ReferenceEquals(dialogueManager, null) && !dialogueManager.dialogueIsPlaying)
        {
            elapsedTime += Time.deltaTime;
            UpdateTimer();
        }
    }


    private void UpdateTimer()
    {
        var ts = System.TimeSpan.FromSeconds(elapsedTime);
        timerText1.text = "<mspace=0.8em>" + ts.ToString("mm");
        timerText2.text = "<mspace=0.8em>" + ts.ToString("ss");
        timerText3.text = "<mspace=0.8em>" + ts.ToString("ff");
    }

    public void LapTime()
    {
        //Get level scene name. Save time
    }
}
