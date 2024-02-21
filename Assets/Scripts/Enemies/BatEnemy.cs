using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatEnemy : MonoBehaviour, EnemyAI, ControlledCharacter
{
    //Dies in about 2 hits. Flys and pursues Big Joe from a set distance. Damages Joe if ran into, can be damaged by jumping on its head, groundpouding, or other attacks. Takes 3 hits to dispatch with no weapon.
    //Can shoot projectiles if Joe is within its firing range.
    [SerializeField] LayerMask ignore;
    [Tooltip("The turrets sight range for the raycast. The player must be in range and seeable")][SerializeField] private float turretSightRange = 350f;
    [Tooltip("This is the delay between shots from the turret")][SerializeField] private float turretFireCooldown = 0.75f;
    [Tooltip("This is the delay before the turret shoots. Change this to shoot at an old turret pos")][SerializeField] private float turretFiringDelay = 0.15f;
    [Tooltip("This is the damage the turret will do if its projectile hits a player")][SerializeField] private float projectileDamage = 15f;
    [Tooltip("The speed of the projectile")][SerializeField] private float projectileSpeed = 15f;
    [Tooltip("Set this to true if you want the turret to begin active")][SerializeField] private bool startOn = true;
    [Tooltip("Set this to true if you want the turret to activate something on its death")][SerializeField] private bool activateItemsOnDeath = false;
    [Tooltip("Add Gameobjects to this list that you want activated on the turret's death")][SerializeField] private GameObject[] itemsToActivate;
    [SerializeField] private GameObject projectile;
    [SerializeField] private PlayerController playerController;
    private bool isOnCooldown = false;
    private bool isActive;
    private FrenzyManager frenzyManager;
    [SerializeField] private SpriteRenderer spriteRenderer;
    Vector2 playerPos;


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

    


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
