using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class SpiderCat : MonoBehaviour, ControlledCharacter, EnemyAI
{
    //Simple ambush AI. Hides on ceiling and drops down when Big Joe is in range. Lands and then begins to pursue Joe. Easily dispatched but quick

    [SerializeField] GameObject playerUI;
    private GameObject UIClone;
    [SerializeField] LayerMask ignore;
    [SerializeField] LayerMask surfaceLayer;
    [Tooltip("The SpiderCat sight range for the raycast. The player must be in range and seeable")][SerializeField] private float sightRange = 350f;
    [Tooltip("This is the damage the SpiderCat will do if it comes in contact with a player")][SerializeField] private float attackDamage = 15f;
    [Tooltip("The speed of the SpiderCat AI ")][SerializeField] private float movementSpeed = 15f;
    [Tooltip("Set this to true if you want the SpiderCat to activate something on its death")][SerializeField] private bool activateItemsOnDeath = false;
    [Tooltip("Add Gameobjects to this list that you want activated on the SpiderCat's death")][SerializeField] private GameObject[] itemsToActivate;
    [Tooltip("This is the time between hits if the player is in attack range of the AI")][SerializeField] private float attackDelay = 1f;
    [SerializeField] private EnemyStates currentState = EnemyStates.Perched;
    [SerializeField] private PlayerController playerController;
    private CapsuleCollider2D capsuleCollider;
    private bool isAlive = true;
    private bool attackDelayActive;
    private FrenzyManager frenzyManager;
    [SerializeField] private SpriteRenderer spriteRendererRubber;
    [SerializeField] private SpriteRenderer spriteRendererRat;
    [SerializeField] private Animator enemyAnimatorRat;
    [SerializeField] private Animator enemyAnimatorRubber;
    private Rigidbody2D enemyRB;
    private Coroutine death;
    private Coroutine attackCooldown;
    private Health health;
    [SerializeField] private Vector2 bottomOffset;
    [SerializeField] private float collRadius;
    private Color debugColor = Color.red;

    private void Awake()
    {
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        playerController = FindObjectOfType<PlayerController>();
        enemyRB = GetComponent<Rigidbody2D>();
        UIClone = Instantiate(playerUI, transform.position + new Vector3(0, 1, 0), Quaternion.identity);
        UIClone.GetComponentInChildren<TextMeshProUGUI>().enabled = false;
        UIClone.transform.parent = transform;
        health = GetComponent<Health>();
        enemyRB.gravityScale = 0;
    }

    private void Start()
    {
        frenzyManager = FrenzyManager.instance;
        playerController = FindObjectOfType<PlayerController>();
    }


    public PlayerUI GetPlayerUI()
    {
        return UIClone.GetComponent<PlayerUI>();
        //No HP bar for this enemy currently
        //return null;
    }


    public void RespawnPlayer()
    {
        isAlive = false;
        //Add to player frenzy meter here/
        frenzyManager.AddToFrenzyMeter(0.15f);
        enemyAnimatorRat.SetTrigger("Death");
        enemyAnimatorRubber.SetTrigger("Death");
        if (activateItemsOnDeath)
        {
            foreach (var item in itemsToActivate)
            {
                item.GetComponent<R4Activatable>()?.Activate();
            }
        }
        if (death == null)
        {
            StopAllCoroutines();
            death = StartCoroutine(DeathDelay());
        }
    }

    IEnumerator DeathDelay()
    {
        enemyRB.simulated = false;
        capsuleCollider.enabled = false;
        yield return new WaitForSeconds(0.267f);
        spriteRendererRubber.DOFade(0, 1f);
        spriteRendererRat.DOFade(0, 1f);
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }




    private void Update()
    {
        if (isAlive)
        {
            if (frenzyManager.inRubberMode)
            {
                //Enemy freezes and becomes intangible in rubbermode
                enemyRB.velocity = Vector3.zero;
                enemyRB.simulated = false;
                capsuleCollider.enabled = false;
            }
            else
            {
                spriteRendererRat.flipX = enemyRB.velocity.x > 0;
                spriteRendererRubber.flipX = enemyRB.velocity.x > 0;
                enemyRB.simulated = true;
                capsuleCollider.enabled = true;
                UpdateState();
            }
        }
    }



    void UpdateState()
    {
        
        if (currentState == EnemyStates.Perched)
        {
            RaycastHit2D hit = Physics2D.Linecast(gameObject.transform.position, playerController.gameObject.transform.position, ~ignore);
            if (!GameObject.ReferenceEquals(hit.collider.gameObject.GetComponent<PlayerController>(), null) && Vector2.Distance(gameObject.transform.position, playerController.gameObject.transform.position) <= sightRange)
            {
                Debug.DrawLine(gameObject.transform.position, playerController.transform.position, Color.green);
                Debug.Log("SpiderCat saw player");
                enemyRB.gravityScale = 1;
                spriteRendererRat.flipY = false;
                spriteRendererRubber.flipY = false; 
                if (Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset, collRadius, surfaceLayer))
                {
                    //Must be on the ground to start moving.
                    currentState = EnemyStates.Pursuing;
                }
            }
            else
            {
                Debug.DrawLine(gameObject.transform.position, playerController.transform.position, Color.red);
            }
            return;
        }
        else if(currentState == EnemyStates.Pursuing)
        {
            Vector2 direction = (Vector2)playerController.transform.position - (Vector2)transform.position;
            direction.Normalize();
            Debug.Log("moving");
            enemyAnimatorRubber.SetBool("IsMoving", true);
            enemyAnimatorRat.SetBool("IsMoving", true);
            //enemyRB.AddForce(direction * speed * Time.deltaTime, ForceMode2D.Force);
            enemyRB.velocity = new Vector2(direction.x * movementSpeed, enemyRB.velocity.y);
            if(capsuleCollider.IsTouching(playerController.GetPlayerCollider()))
            {
                currentState = EnemyStates.Attacking;
            }
            return;
        }
        else if(currentState == EnemyStates.Idle)
        {
            Debug.Log("SpiderCat on attack delay");
            enemyRB.velocity = Vector2.zero;
            if(!attackDelayActive)
            {
                currentState = EnemyStates.Pursuing;
            }
            return;
        }
        else if (currentState == EnemyStates.Attacking)
        {
            if (!attackDelayActive)
            {
                enemyRB.velocity = Vector2.zero;
                enemyAnimatorRat.SetTrigger("Attack");
                enemyAnimatorRubber.SetTrigger("Attack");
                playerController.GetComponent<Health>().SubtractFromHealth(attackDamage);
                //only run coroutine if it isn't already active.
                if (attackCooldown == null) attackCooldown = StartCoroutine(AttackDelayTimer());
            }
            else
            {
                currentState = EnemyStates.Idle;
            }
            return;
        }
    }


    //Cooldown to prevent crazy unfair attacks and movement.
    IEnumerator AttackDelayTimer()
    {
        attackDelayActive = true;
        yield return new WaitForSeconds(attackDelay);
        attackDelayActive = false;
        attackCooldown = null;
    }

    public void PlayDamagedAnim()
    {
        enemyAnimatorRat.SetTrigger("Damaged");
        enemyAnimatorRubber.SetTrigger("Damaged");
        if (Coroutine.ReferenceEquals(attackCooldown, null)) attackCooldown = StartCoroutine(AttackDelayTimer());
        else
        {
            StopCoroutine(attackCooldown);
            attackCooldown = StartCoroutine(AttackDelayTimer());
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = debugColor;
        var positions = new Vector2[] { bottomOffset };
        Gizmos.DrawSphere((Vector2)transform.position + bottomOffset, collRadius);
        Gizmos.DrawSphere((Vector2)transform.position, sightRange);
    }

    public PlayerController GetPlayerController()
    {
        throw new System.NotImplementedException();
    }
    public Health GetHealth() { return health; }
}

public interface EnemyAI
{
    public Health GetHealth();
}