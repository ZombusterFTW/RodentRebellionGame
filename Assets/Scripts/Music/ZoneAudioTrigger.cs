using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneAudioTrigger : MonoBehaviour
{
    [SerializeField] FinalLevelZone zoneToTrigger = FinalLevelZone.Zone1;
    [SerializeField] FinalBossMusicManager FBMusicManagerParent;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            FBMusicManagerParent.TransitionToZone(zoneToTrigger);
        }
    }
}
