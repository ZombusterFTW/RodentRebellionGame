using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class FinalBossMusicManager : MonoBehaviour
{
    [SerializeField] private AudioSource levelIntro;
    [SerializeField] private AudioSource levelLoop;
    [SerializeField] private AudioSource area1Loop;
    [SerializeField] private AudioSource area2Loop;
    [SerializeField] private AudioSource area3Loop;
    [SerializeField] private AudioSource area4Loop;


    [SerializeField] private AudioMixerSnapshot gameIntroFull;
    [SerializeField] private AudioMixerSnapshot gameLoopFull;
    [SerializeField] private AudioMixerSnapshot bossArea1;
    [SerializeField] private AudioMixerSnapshot bossArea2;
    [SerializeField] private AudioMixerSnapshot bossArea3;
    [SerializeField] private AudioMixerSnapshot bossArea4;
    [SerializeField] AudioMixerSnapshot unmuteGame;


    [SerializeField] private float zoneTransitionTime = 0.25f;
    bool introPlaying = true;
    // Start is called before the first frame update
    void Start()
    {
        unmuteGame.TransitionTo(0);
        levelIntro.Play();
        gameIntroFull.TransitionTo(0);
        StartCoroutine(WaitAndPlayLoop(levelIntro.clip.length * 0.80f));
    }

    IEnumerator WaitAndPlayLoop(float timeToWait)
    {
        yield return new WaitForSecondsRealtime(timeToWait);
        introPlaying = false;
        levelLoop.Play();
        gameLoopFull.TransitionTo(0.1f);
        bossArea1.TransitionTo(0.1f);
        area1Loop.Play();
        area2Loop.Play();
        area3Loop.Play();
        area4Loop.Play();
    }

    //A zone transition trigger will fire this function.
    public void TransitionToZone(FinalLevelZone zoneName)
    {
        if (introPlaying) return;
        switch (zoneName)
        {
            case FinalLevelZone.Zone1:
                bossArea1.TransitionTo(zoneTransitionTime);
                break;
            case FinalLevelZone.Zone2:
                bossArea2.TransitionTo(zoneTransitionTime);
                break;
            case FinalLevelZone.Zone3:
                bossArea3.TransitionTo(zoneTransitionTime);
                break;
            case FinalLevelZone.Zone4:
                bossArea4.TransitionTo(zoneTransitionTime);
                break;
        }
    }


}
public enum FinalLevelZone
{
    Zone1,Zone2,Zone3,Zone4
}