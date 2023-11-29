using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSoundManager : MonoBehaviour
{

    [SerializeField] private AudioSource movementSource;
    [SerializeField] private AudioSource attackSource;
    [SerializeField] private AudioSource hurtSource;
    [SerializeField] private AudioSource footstepSource;
    [SerializeField] private AudioSource weaponSource;

    [SerializeField] private AudioClip[] attackSounds;
    [SerializeField] private AudioClip[] landingSounds;
    [SerializeField] private AudioClip[] hurtSounds;
    [SerializeField] private AudioClip deathSound;

    [SerializeField] private AudioClip weapon1Sound;
    [SerializeField] private AudioClip weapon2Sound;
    [SerializeField] private AudioClip noWeaponSound;
    [SerializeField] private AudioClip movementLoop;

    public bool movementPlaying = false;

    // private Coroutine movementLoop;


    private void Start()
    {
        //Set the movement source's clip to the movement clip automatically
        movementSource.clip = movementLoop;
    }



    public AudioClip PlayAudioCallout(CharacterAudioCallout callout)
    {
        AudioClip clipToReturn;
        switch (callout)
        {
            default: clipToReturn = null; 
                break;
            case CharacterAudioCallout.Death:
                hurtSource.Stop();
                hurtSource.clip = deathSound;
                if (hurtSource.clip != null) hurtSource.Play();
                clipToReturn = hurtSource.clip;
                break;
            case CharacterAudioCallout.Attack:
                hurtSource.Stop();
                hurtSource.clip = attackSounds[Random.Range(0, attackSounds.Length)];
                if (hurtSource.clip != null) hurtSource.Play();
                clipToReturn = hurtSource.clip;
                break;
            case CharacterAudioCallout.Weapon1:
                weaponSource.Stop();
                weaponSource.clip = weapon1Sound;
                if (weaponSource.clip != null) weaponSource.Play();
                clipToReturn = weaponSource.clip;
                break;
            case CharacterAudioCallout.Weapon2:
                weaponSource.Stop();
                weaponSource.clip = weapon2Sound;
                if (weaponSource.clip != null) weaponSource.Play();
                clipToReturn = weaponSource.clip;
                break;
            case CharacterAudioCallout.NoWeapon:
                weaponSource.Stop();
                weaponSource.clip = noWeaponSound;
                if(weaponSource.clip != null) weaponSource.Play();
                clipToReturn = weaponSource.clip;
                break;
            case CharacterAudioCallout.Hurt:
                hurtSource.Stop();
                hurtSource.clip = hurtSounds[Random.Range(0, hurtSounds.Length)];
                if (hurtSource.clip != null) hurtSource.Play();
                clipToReturn = hurtSource.clip;
                break;
            case CharacterAudioCallout.Land:
                movementSource.Stop();
                movementSource.clip = landingSounds[Random.Range(0, hurtSounds.Length)];
                if (movementSource.clip != null) movementSource.Play();
                clipToReturn = movementSource.clip;
                break;
        }
        return clipToReturn;
    }

    public void ActivateMovementLoopSound(bool activate)
    {
        if(activate && !movementPlaying)
        {
            movementPlaying = true;
            footstepSource.Play();
        }
        else
        {
            movementPlaying = false;
            footstepSource.Stop();
        }
    }
}


//Each enum is a specific audio callout type
public enum CharacterAudioCallout
{
    Death,
    Attack,
    Weapon1,
    Weapon2,
    NoWeapon,
    Hurt,
    Land
}
