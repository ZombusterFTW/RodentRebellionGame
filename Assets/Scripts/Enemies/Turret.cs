using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class Turret : MonoBehaviour, R4Activatable, OneHitHealthEnemy
{
    //Shoot at targets within its range and cone of view. Can be destroyed by attacks.
    //Shoots at players position, so it doesn't unfairly hit them as they move. 
    //Needs to become "dead" in rubber mode.
    [Tooltip("Set to true to see gizmos that show turret range")][SerializeField] private bool drawGizmos = true;
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

    private void Awake()
    {
        
        playerController = FindObjectOfType<PlayerController>();
        //spriteRenderer = GetComponent<SpriteRenderer>();
    }
    // Start is called before the first frame update
    void Start()
    {
        frenzyManager = FrenzyManager.instance;

        if (startOn) 
        {
            Activate();
        }
        else
        {
            Deactivate();
        }
    }


    private void OnDrawGizmos()
    {
        if(drawGizmos) 
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere((Vector2)transform.position, turretSightRange);
        }
    }


    // Update is called once per frame
    void Update()
    {
        if(!isOnCooldown && !frenzyManager.inRubberMode && isActive) 
        {
            RaycastHit2D hit = Physics2D.Linecast(gameObject.transform.position, playerController.gameObject.transform.position, ~ignore);
            if (!GameObject.ReferenceEquals(hit.collider.gameObject.GetComponent<PlayerController>(), null) && Vector2.Distance(gameObject.transform.position, playerController.gameObject.transform.position) <= turretSightRange)
            {
                Debug.DrawLine(gameObject.transform.position, playerController.transform.position, Color.green);
                StartCoroutine(TurretFiringDelayControl());
            }
            else
            {
                Debug.DrawLine(gameObject.transform.position, playerController.transform.position, Color.red);
            }
        }
    }
    

    IEnumerator TurretFiringDelayControl()
    {
        isOnCooldown = true;
        playerPos = playerController.transform.position;
        yield return new WaitForSeconds(turretFiringDelay);
        if (frenzyManager.inRubberMode)
        {
            yield return null;
        }
        GameObject projectileClone = Instantiate(projectile, gameObject.transform.position, Quaternion.identity);
        Projectile projectileRef = projectileClone.GetComponent<Projectile>();
        projectileRef.targetPosition = playerPos;
        projectileRef.damageByProjectile = projectileDamage;
        projectileRef.projectileSpeed = projectileSpeed;
        yield return new WaitForSeconds(turretFireCooldown);
        isOnCooldown = false;
    }

    public void DestroyTurret()
    {
        frenzyManager.AddToFrenzyMeter(0.15f);
        //Kill turret here.
        if (activateItemsOnDeath)
        {
            foreach (var item in itemsToActivate)
            {
                item.GetComponent<R4Activatable>()?.Activate();
            }
        }
        StopAllCoroutines();
        Destroy(gameObject);
    }

    public void Activate()
    {
        isActive = true;
        //edit sprite here.
        spriteRenderer.color = Color.white; 
    }

    public void Deactivate()
    {
        isActive = false;
        //Edit sprite here
        spriteRenderer.color = Color.gray;
    }

    public void OnOneHitEnemyDeath()
    {
        DestroyTurret();
    }
}
