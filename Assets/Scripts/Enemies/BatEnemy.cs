using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatEnemy : MonoBehaviour, EnemyAI, ControlledCharacter
{
    public Health GetHealth()
    {
        throw new System.NotImplementedException();
    }

    public PlayerController GetPlayerController()
    {
        throw new System.NotImplementedException();
    }

    public PlayerUI GetPlayerUI()
    {
        throw new System.NotImplementedException();
    }

    public void PlayDamagedAnim()
    {
        throw new System.NotImplementedException();
    }

    public void RespawnPlayer()
    {
        throw new System.NotImplementedException();
    }

    //Dies in about 2 hits. Flys and pursues Big Joe from a set distance. Damages Joe if ran into, can be damaged by jumping on its head, groundpouding, or other attacks. Takes 3 hits to dispatch with no weapon.
    //Can shoot projectiles if Joe is within its firing range.


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
