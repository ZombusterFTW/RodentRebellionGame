using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Audio;

//reference: https://johnleonardfrench.com/ultimate-guide-to-playscheduled-in-unity/

public class SoundManager : MonoBehaviour
{

    //Public Values!!!
    [Header("Audio Sources")]
    //these are all lists so they can be cylced through
    public AudioSource[] introSrcArr;
    public AudioSource[] leadGSrcArr;
    public AudioSource[] bassGSrcArr;
    public AudioSource[] chordGSrcArr;
    public AudioSource[] drumSrcArr;

    [Header("Number of Looping Audio Clips")]
    public int numLoopingClips;

    [Header("Audio Clips")]
    public AudioClip[] introClipsArr;
    public AudioClip[] leadGClipsArr;
    public AudioClip[] bassGClipsArr;
    public AudioClip[] chordGClipsArr;
    public AudioClip[] drumClipsArr;

    [Header("Audio Mixer")]
    public AudioMixerGroup mainMixer;
    public AudioMixer audioMixer;
    
    //Private Values!!!
    private double introDuration;
    private double nextStartTime;
    private int toggle = 0; //toggles audio sources
    private int nextClip = 0; //iterates through audio clips per source

    private DialogueManager dialogueManager; //grab active dialogue manager instance

    private bool justExitedDialogue = false;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("SoundManager::Start()");

        //get dialogue manager
        dialogueManager = DialogueManager.GetInstance();

        double startTime = AudioSettings.dspTime + 0.2; //add 2 ms for lag

        //play intro at start
        LoadIntroBGM();
        PlayIntroBGM(startTime);
        //all intro sources are set to play on awake with no loop
        // should be able to just calculate when they end to start the looping section

        //calculate length of intro, start rest of audio after that
        introDuration = (double) introClipsArr[0].samples / introClipsArr[0].frequency; //duration

        nextStartTime = startTime + introDuration; //set time for loop section to start
    }

    private void LoadIntroBGM() {

        Debug.Log("SoundManager::LoadIntroBGM()");

        for (int i = 0; i < introSrcArr.Length; i++)
        {
            introSrcArr[i].clip = introClipsArr[i];
        }
    }

    private void PlayIntroBGM(double start) {
        Debug.Log("SoundManager::PlayIntroBGM()");
        for(int i = 0; i < introSrcArr.Length; i++)
        {
            introSrcArr[i].PlayScheduled(start);
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (dialogueManager.dialogueIsPlaying)
        {
            //Debug.Log("SoundManager::Update() - Dialogue is Playing!!!");

            //enter dialogue snapshot
            AudioMixerSnapshot dialougeSnap = audioMixer.FindSnapshot("Dialogue");
            dialougeSnap.TransitionTo(0.5f);
            
        }else
        {
            AudioMixerSnapshot snap = audioMixer.FindSnapshot("Snapshot");
            snap.TransitionTo(0.5f);
        }
        
        if(AudioSettings.dspTime > nextStartTime - 1)
        {
           
            leadGSrcArr[toggle].clip = leadGClipsArr[nextClip];
            bassGSrcArr[toggle].clip = bassGClipsArr[nextClip];
            chordGSrcArr[toggle].clip = chordGClipsArr[nextClip];
            drumSrcArr[toggle].clip = drumClipsArr[nextClip];

            leadGSrcArr[toggle].PlayScheduled(nextStartTime);
            bassGSrcArr[toggle].PlayScheduled(nextStartTime);
            chordGSrcArr[toggle].PlayScheduled(nextStartTime);
            drumSrcArr[toggle].PlayScheduled(nextStartTime);


            //check duration of next clip to update next start time
                //all clips should be same length so use lead guitar as reference
            double duration = (double)leadGClipsArr[nextClip].samples / leadGClipsArr[nextClip].frequency;
            //nextStartTime = nextStartTime + duration;
            nextStartTime += duration;

            //switch audio sources
            toggle = 1 - toggle; // 1 - 0 = 1 -> 1 - 1 = 0

            //a nice terinary for array indexing
            //TODO: numLoopingClips edited when checkpoint is met so loops are changed "dynamically"
            nextClip = nextClip < numLoopingClips - 1 ? nextClip + 1 : 0;

        }
    }

    void PauseMusic()
    {

        //might want to leave bass line unpaused...
        //bass_guitar_src.ignoreListenerPause = true;

        AudioListener.pause = true; //should pause all audio sources
        
    }

    void UnPauseMusic()
    {
        AudioListener.pause = false; //should unpause all audio sources
    }
}
