using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class GameMusicTempSolution : MonoBehaviour
{

    //Area 1 should be tutorial and all labyrinths
    //Area 2 is cave, area 3 is lab, and area 4 should be surface
    [SerializeField] AudioClip area1Intro, area2Intro, area3Intro, area4Intro, area1Loop, area2Loop, area3Loop, area4Loop;
    private AudioClip intro, loop;
    private string sceneName;
    [SerializeField] AudioSource introMusicSource;
    [SerializeField] AudioSource loopMusicSource;



    [SerializeField] AudioMixerSnapshot introOnly;
    [SerializeField] AudioMixerSnapshot loopOnly;
    [SerializeField] AudioMixerSnapshot unmuteGame;
    // Start is called before the first frame update
    void Start()
    {
        unmuteGame.TransitionTo(0);
        sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "0Tutorial" || sceneName == "Labyrinth1" || sceneName == "Labyrinth2" || sceneName == "Labyrinth3")
        {
            intro = area1Intro;
            loop = area1Loop;
        }
        else if (sceneName == "RadioactiveCave")
        {
            intro = area2Intro;
            loop = area2Loop;
        }
        else if (sceneName == "LabLevel")
        {
            intro = area3Intro;
            loop = area3Loop;
        }
        else if (sceneName == "Surface")
        {
            intro = area4Intro;
            loop = area4Loop;
        }


        introMusicSource.clip = intro;
        loopMusicSource.clip = loop;
        introOnly.TransitionTo(0);
        introMusicSource.Play();
        
        StartCoroutine(WaitAndPlayLoop(intro.length*0.70f));
    }

   IEnumerator WaitAndPlayLoop(float timeToWait)
    {
        yield return new WaitForSecondsRealtime(timeToWait);
        loopMusicSource.Play();
        loopOnly.TransitionTo(0.1f);
    }
}
