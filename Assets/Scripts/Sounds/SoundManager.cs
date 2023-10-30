using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{

    [Header("Instrument Tracks")] // track list per instrument
    public AudioClip[] tracks; //list of full tracks (for testing)
    public AudioClip[] lead_guitar;
    public AudioClip[] back_guitar;
    public AudioClip[] bass_guitar;
    public AudioClip[] drum_kit;

    [Header("Audio Sources")]
    public AudioSource bgm_source;
    public AudioSource lead_guitar_src;
    public AudioSource bass_guitar_src;
    public AudioSource back_guitar_src;
    public AudioSource drum_kit_src;

    [Header("Audio Mixers")]
    public AudioMixer bgm_mixer; //not really using this rn, but setting it up for future

    public float bpm = 140.0f;
    public int numBeatsPerMeasure = 32;

    private double nextEventTime;
    

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("SoundManager::Start()");

        //bgm_source.clip = tracks[0]; //load init clip
        //bgm_source.Play(); //play on start

        double startTime = AudioSettings.dspTime + 0.5; //delay for cleanliness

        //Load baselines
        bass_guitar_src.clip = bass_guitar[0];
        drum_kit_src.clip = drum_kit[0];

        bass_guitar_src.PlayScheduled(startTime);
        drum_kit_src.PlayScheduled(startTime);

        //set guitar to start after one loop of base line
        double duration = (double)drum_kit[0].samples / drum_kit[0].frequency;

        bgm_source.clip = tracks[1];
        bgm_source.PlayScheduled(startTime + duration);

    }

    // Update is called once per frame
    void Update()
    {

        //nextEventTime += 60.0f / bpm * numBeatsPerMeasure;
        //back_guitar_src.PlayScheduled(nextEventTime);
        

    }
}
