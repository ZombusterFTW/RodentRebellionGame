using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Audio;

//reference: https://johnleonardfrench.com/ultimate-guide-to-playscheduled-in-unity/

public class SoundManager : MonoBehaviour
{
    //Public Values!!!

    [Header("Audio Mixer")]
    public AudioMixerGroup mainMixer;
    public AudioMixer audioMixer;

    
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
    
    [Header("Frenzy Mode!!")]
    public AudioSource[] frenzySrcArr;
    public AudioClip[] frenzyClipsArr;
    
    //Private Values!!!
    private double introDuration;
    private double nextStartTime;
    private int toggle = 0; //toggles audio sources
    private int nextClip = 0; //iterates through audio clips per source

    private DialogueManager dialogueManager; //grab active dialogue manager instance
    private FrenzyManager frenzyManager;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("SoundManager::Start()");

        //get dialogue manager
        dialogueManager = DialogueManager.GetInstance();
        frenzyManager = FrenzyManager.instance; //get FrenzyManager instance

        double startTime = AudioSettings.dspTime + 0.2; //add 2 ms for lag

        //play intro at start
        LoadIntroBGM();
        PlayIntroBGM(startTime);
        //all intro sources are set to play on awake with no loop
        // should be able to just calculate when they end to start the looping section

        //load frenzy clips
        LoadFrenzyBGM();

        //calculate length of intro, start rest of audio after that
        introDuration = (double) introClipsArr[3].samples / introClipsArr[3].frequency; //duration

        nextStartTime = startTime + introDuration; //set time for loop section to start
    }

    private void LoadIntroBGM() {

        Debug.Log("SoundManager::LoadIntroBGM()");

        for (int i = 0; i < introSrcArr.Length; i++)
        {
            introSrcArr[i].clip = introClipsArr[i];
        }
    }

    private void LoadFrenzyBGM()
    {
        //load at start for ease - its the same the whole time
        Debug.Log("SoundManager::LoadFrenzyBGM()");
        for(int i = 0; i < frenzySrcArr.Length; i++)
        {
            frenzySrcArr[i].clip = frenzyClipsArr[i];
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

        //check dialogue mode
        if (dialogueManager.dialogueIsPlaying)
        {
            Debug.Log("Dialogue is playing!!!");
            //enter dialogue snapshot
            
            audioMixer.FindSnapshot("Dialogue").TransitionTo(0.2f);
            
        }else
        {
            audioMixer.FindSnapshot("Gameplay").TransitionTo(0.5f);
        }

        //check frenzy mode
        if (frenzyManager.frenzyActive)
        {
            PlayFrenzyBGM();
        }else
        {
            StopFrenzyBGM();
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

    private void PlayFrenzyBGM()
    {

        leadGSrcArr[toggle].Pause();
        bassGSrcArr[toggle].Pause();
        chordGSrcArr[toggle].Pause();
        drumSrcArr[toggle].Pause();

        for (int i = 0; i < frenzySrcArr.Length; i++)
        {

            if (!frenzySrcArr[i].isPlaying) //if not already playing -> account for update() loop
            {
                frenzySrcArr[i].Play(); //set them all to play
            }
        }

        audioMixer.FindSnapshot("Frenzy").TransitionTo(0.01f); //trainsition to frenzy mixer
    }

    private void StopFrenzyBGM()
    {

        leadGSrcArr[toggle].UnPause();
        bassGSrcArr[toggle].UnPause();
        chordGSrcArr[toggle].UnPause();
        drumSrcArr[toggle].UnPause();

        for (int i = 0; i < frenzySrcArr.Length; i++)
        {
            frenzySrcArr[i].Stop(); //stop all audio
        }
        audioMixer.FindSnapshot("Gameplay").TransitionTo(0.01f); //back to gameplay
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
