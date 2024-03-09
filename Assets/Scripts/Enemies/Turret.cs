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
    [Header("Animation Settings")]
    [Tooltip("Set this to true if the turret animations appear mirrored in-game")][SerializeField] private bool mirrorAnims = false;
    [Tooltip("Set this to true if the turret is mounted on a wall")][SerializeField] private bool isOnWall = false;
    [Header("Turret Settings")]
    [Tooltip("Set to true to see gizmos that show turret range")][SerializeField] private bool drawGizmos = true;
    [SerializeField] LayerMask ignore;
    [Tooltip("Set this to true if you want the turret to begin active")][SerializeField] private bool startOn = true;
    [Tooltip("The turrets sight range for the raycast. The player must be in range and seeable")][SerializeField] private float turretSightRange = 15f;
    [Tooltip("This is the delay between shots from the turret")] private float turretFireCooldown = 1.5f;
    [Tooltip("This is the delay before the turret shoots. Change this to shoot at an old turret pos")] private float turretFiringDelay = 0.15f;
    [Tooltip("This is the damage the turret will do if its projectile hits a player")][SerializeField] private float projectileDamage = 34f;
    [Tooltip("The speed of the projectile")] private float projectileSpeed = 6f;
    [Tooltip("Set this to true if you want the turret to activate something on its death")][SerializeField] private bool activateItemsOnDeath = false;
    [Tooltip("Add Gameobjects to this list that you want activated on the turret's death")][SerializeField] private GameObject[] itemsToActivate;
    [SerializeField] private GameObject projectile;
    [SerializeField] private PlayerController playerController;
    private bool isOnCooldown = false;
    private bool isActive;
    private FrenzyManager frenzyManager;
    [Tooltip("Sprite renderer for the rat mode turret.")][SerializeField] private SpriteRenderer spriteRenderer;
    Vector2 playerPos;
    [SerializeField] private Animator ratModeAnimator;
    [SerializeField] private Animator rubberModeAnimator;
    [SerializeField] private GameObject[] firingPositions;

    private void Awake()
    {
        
        
        //spriteRenderer = GetComponent<SpriteRenderer>();
    }
    // Start is called before the first frame update
    void Start()
    {
        playerController = PlayerController.instance;
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
        if (!isOnCooldown && !frenzyManager.inRubberMode && isActive && Vector2.Distance(gameObject.transform.position, playerController.gameObject.transform.position) <= turretSightRange) 
        {
            RaycastHit2D hit = Physics2D.Linecast(gameObject.transform.position, playerController.gameObject.transform.position, ~ignore);
            if (!GameObject.ReferenceEquals(hit.collider.gameObject.GetComponent<PlayerController>(), null) )
            {
                Debug.DrawLine(gameObject.transform.position, playerController.transform.position, Color.green);
                StartCoroutine(TurretFiringDelayControl());
                if (!isOnWall)
                {
                    if (!mirrorAnims)
                    {
                        float playerXDir = ((Vector2)playerController.transform.position - (Vector2)transform.position).normalized.x;
                        ratModeAnimator.SetFloat("PlayerDir", playerXDir);
                        rubberModeAnimator.SetFloat("PlayerDir", playerXDir);
                    }
                    else
                    {
                        float playerXDir = -((Vector2)playerController.transform.position - (Vector2)transform.position).normalized.x;
                        ratModeAnimator.SetFloat("PlayerDir", playerXDir);
                        rubberModeAnimator.SetFloat("PlayerDir", playerXDir);
                    }
                }
                else
                {
                    if (!mirrorAnims)
                    {
                        float playerYDir = ((Vector2)playerController.transform.position - (Vector2)transform.position).normalized.y;
                        ratModeAnimator.SetFloat("PlayerDir", playerYDir);
                        rubberModeAnimator.SetFloat("PlayerDir", playerYDir);
                    }
                    else
                    {
                        float playerYDir = -((Vector2)playerController.transform.position - (Vector2)transform.position).normalized.y;
                        ratModeAnimator.SetFloat("PlayerDir", playerYDir);
                        rubberModeAnimator.SetFloat("PlayerDir", playerYDir);
                    }
                }
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
        //This finds the current running anim and picks the correct firing spot. 
        {
            GameObject firingPos;
            if (ratModeAnimator.GetCurrentAnimatorStateInfo(0).IsName("turretleft"))
            {
                firingPos = firingPositions[0];
            }
            else if (ratModeAnimator.GetCurrentAnimatorStateInfo(0).IsName("turretleftup"))
            {
                firingPos = firingPositions[1];
            }
            else if (ratModeAnimator.GetCurrentAnimatorStateInfo(0).IsName("turretleftcenter"))
            {
                firingPos = firingPositions[2];
            }
            else if (ratModeAnimator.GetCurrentAnimatorStateInfo(0).IsName("turretcenter"))
            {
                firingPos = firingPositions[3];
            }
            else if (ratModeAnimator.GetCurrentAnimatorStateInfo(0).IsName("turretrightcenter"))
            {
                firingPos = firingPositions[4];
            }
            else if (ratModeAnimator.GetCurrentAnimatorStateInfo(0).IsName("turretrightup"))
            {
                firingPos = firingPositions[5];
            }
            else if (ratModeAnimator.GetCurrentAnimatorStateInfo(0).IsName("turretright"))
            {
                firingPos = firingPositions[6];
            }
            else
            {
                //bad thing happened if we get here
                firingPos = gameObject;
            }
            GameObject projectileClone = Instantiate(projectile, firingPos.transform.position, Quaternion.identity);
            Projectile projectileRef = projectileClone.GetComponent<Projectile>();
            projectileRef.targetPosition = playerPos;
            projectileRef.damageByProjectile = projectileDamage;
            projectileRef.projectileSpeed = projectileSpeed;
        }
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
