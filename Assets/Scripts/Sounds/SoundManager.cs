using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{

    [Header("Audio Tracks")]
    public AudioClip[] tracks; //list of audio tracks to select from

    [Header("Audio Sources")]
    public AudioSource bgm_source;

    [Header("Audio Mixers")]
    public AudioMixer bgm_mixer; //not really using this rn, but setting it up for future
    

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("SoundManager::Start()");

        bgm_source.clip = tracks[0]; //load init clip
        bgm_source.Play(); //play on start

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
