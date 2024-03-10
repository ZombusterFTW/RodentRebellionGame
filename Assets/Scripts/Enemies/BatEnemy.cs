using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Pathfinding;
using UnityEngine.Windows;

public class BatEnemy : MonoBehaviour, EnemyAI, ControlledCharacter
{
    //Dies in about 2 hits. Flys and pursues Big Joe from a set distance. Damages Joe if ran into, can be damaged by jumping on its head, groundpouding, or other attacks. Takes 3 hits to dispatch with no weapon.
    //Can shoot projectiles if Joe is within its firing range.
    [SerializeField] bool drawGizmos = true;
    [SerializeField] GameObject playerUI;
    private GameObject UIClone;
    [SerializeField] LayerMask ignore;
    [Tooltip("If the player is within this distance from the bat it will pursue them")] private float batPursueRange = 6.6f;
    [Tooltip("The bat sight range for the raycast. The player must be in range and seeable")] private float turretSightRange = 4f;
    [Tooltip("This is the delay between shots from the bat")] private float turretFireCooldown = 2.5f;
    [Tooltip("This is the delay before the bat shoots. Change this to shoot at an old turret pos")] private float turretFiringDelay = 1f;
    [Tooltip("This value is how much damage the player will take if they collide with the bat and aren't attacking")] private float damageOnCollision = 1.5f;
    [Tooltip("This is the damage the bat will do if its projectile hits a player")] private float projectileDamage = 5f;
    [Tooltip("The speed of the projectile")] private float projectileSpeed = 10f;
    [Tooltip("Set this to true if you want the bat to activate something on its death")][SerializeField] private bool activateItemsOnDeath = false;
    [Tooltip("Add Gameobjects to this list that you want activated on the bat's death")][SerializeField] private GameObject[] itemsToActivate;
    [SerializeField] private GameObject projectile;
    private bool isOnCooldown = false;
    private bool isActive;
    private FrenzyManager frenzyManager;
    [SerializeField] private SpriteRenderer spriteRendererRubber;
    [SerializeField] private SpriteRenderer spriteRendererRat;
    Vector2 playerPos;
    private Health health;
    private Transform target;
    private float speed = 3.25f;
    private float nextWaypointDistance = 0.75f;
    Path path;
    int currentWaypoint = 0;
    bool reachedEndOfPath = false;
    private Seeker seeker;
    Rigidbody2D rb;
    private CircleCollider2D circleCollider;
    private Color debugColor = Color.red;
    bool isAlive = true;
    private Coroutine extendedDamage;

    private void Awake()
    {
        UIClone = Instantiate(playerUI, transform.position + new Vector3(0, 1, 0), Quaternion.identity);
        UIClone.GetComponentInChildren<TextMeshProUGUI>().enabled = false;
        UIClone.transform.parent = transform;
    }

    // Start is called before the first frame update
    void Start()
    {
        circleCollider = GetComponent<CircleCollider2D>();  
        target = FindObjectOfType<PlayerController>().GetComponent<Transform>();
        frenzyManager = FrenzyManager.instance;
        health = GetComponent<Health>();
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        InvokeRepeating("UpdatePath", 0f, 0.5f);
        seeker.StartPath(rb.position, target.position, OnPathComplete);
    }

    void UpdatePath()
    {
        if(seeker.IsDone()) 
        {
            seeker.StartPath(rb.position, target.position, OnPathComplete);
        }
    }

    void OnPathComplete(Path p)
    {
        if(!p.error) 
        {
            path = p;
            currentWaypoint = 0;    
        }
    }

    private void BatMovement()
    {
        if(!frenzyManager.inRubberMode)
        {
            rb.simulated = true;
            circleCollider.enabled = true;
            if (Vector2.Distance(gameObject.transform.position, target.position) <= batPursueRange && path != null)
            {
                if (currentWaypoint >= path.vectorPath.Count)
                {
                    reachedEndOfPath = true;
                    return;
                }
                else
                {
                    reachedEndOfPath = false;
                }
                rb.MovePosition(Vector2.MoveTowards(transform.position, (Vector2)path.vectorPath[currentWaypoint], speed * Time.deltaTime));
                //Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
                //Vector2 force = direction * speed * Time.deltaTime;
               // rb.AddForce(force);
                float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);


                float temp = 0;

                temp = (rb.position - (Vector2)path.vectorPath[currentWaypoint]).normalized.x;




                if (temp < 0)
                {
                    spriteRendererRubber.flipX = true;
                    spriteRendererRat.flipX = true;
                }
                else
                {
                    spriteRendererRubber.flipX = false;
                    spriteRendererRat.flipX = false;
                }

                if (distance < nextWaypointDistance)
                {
                    currentWaypoint++;
                }
            }
        }
        else
        {
            rb.simulated = false;
            circleCollider.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isOnCooldown && !frenzyManager.inRubberMode)
        {
            RaycastHit2D hit = Physics2D.Linecast(gameObject.transform.position, target.position, ~ignore);
            if (!GameObject.ReferenceEquals(hit.collider.gameObject.GetComponent<PlayerController>(), null) && Vector2.Distance(gameObject.transform.position, target.position) <= turretSightRange)
            {
                //Shoot proj at player
                Debug.DrawLine(gameObject.transform.position, target.position, Color.green);
                StartCoroutine(BatFiringDelayControl());
            }
            else
            {
                Debug.DrawLine(gameObject.transform.position, target.position, Color.red);
            }
        }
        BatMovement();
    }


    IEnumerator BatFiringDelayControl()
    {
        isOnCooldown = true;
        playerPos = target.position;
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

    public void KillBat()
    {
        if(isAlive)
        {
            isAlive = false;
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
    }

    private void OnDrawGizmos()
    {
        if(drawGizmos)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere((Vector2)transform.position, batPursueRange);
            Gizmos.color = Color.red;
            Gizmos.DrawSphere((Vector2)transform.position, turretSightRange);
            //Gizmos.DrawSphere((Vector2)transform.position + leftCollCheck, collRadiusWall);
            //Gizmos.DrawSphere((Vector2)transform.position + rightCollCheck, collRadiusWall);
        }
    }



    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!GameObject.ReferenceEquals(collision.gameObject.GetComponent<PlayerController>(), null))
        {
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
                if (!player.isDashing && !player.isGroundPounding)
                {
                    //The player must take damage here.
                    collision.gameObject.GetComponent<Health>().SubtractFromHealth(damageOnCollision);
                    if (extendedDamage == null) extendedDamage = StartCoroutine(PlayerDamageLoop(collision.gameObject.GetComponent<Health>()));
                }
        }
    }


    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!GameObject.ReferenceEquals(collision.gameObject.GetComponent<PlayerController>(), null))
        {
            if (extendedDamage != null) StopCoroutine(extendedDamage);
        }
    }




    IEnumerator PlayerDamageLoop(Health playerHealth)
    {
        while (isAlive)
        {
            yield return new WaitForSeconds(5);
            playerHealth.SubtractFromHealth(damageOnCollision);
        }

    }


    public Health GetHealth()
    {
        return health;
    }

    public PlayerController GetPlayerController()
    {
        throw new System.NotImplementedException();
    }

    public PlayerUI GetPlayerUI()
    {
        return UIClone.GetComponent<PlayerUI>();
    }

    public void PlayDamagedAnim()
    {
        //No anim for this yet
        //Prevent player from being hurt by the enemy if they are actively attacking it
        if(!Coroutine.ReferenceEquals(extendedDamage, null)) StopCoroutine(extendedDamage);
    }

    public void RespawnPlayer()
    {
        KillBat();
    }

}
