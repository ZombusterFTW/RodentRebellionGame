using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyScript : MonoBehaviour, ControlledCharacter, EnemyAI
{
    //Notes for production. This script works decently well for the less engaging enemies etc the small cats, but is relatively boring and simple for bosses. 
    //Bosses need to have more attack states and intelligence. To acheive this I think its needed to create a new class that uses the majority of this class, but also include more code that allows for long range projectiles. 
    //Mr whiskers will likely have his own class completely allowing for him to have a noticably different attack pattern.


    //Enemies need to be "disabled" when the player is in rubber mode.
    //In rubber mode they will appear as translucent silhollettes that are intangible and can be passed through.

    [SerializeField] GameObject playerUI;
    private GameObject UIClone;
    private Health health;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator enemyAnimator;
    public EnemyStates currentState = EnemyStates.Searching;
    private GameObject target = null;
    [SerializeField] private Vector2 searchDirection = Vector2.left;
    private Vector2 targetPosition;
    private Vector2 startingPos;
    public float sightRange = 25.0f;
    public float speed = 150.0f;
    public float attackDistance = 5f;
    public float attackDelay = 1f;
    private bool attackDelayActive;
    public float searchDelay = 1.5f;
    [SerializeField] private bool searchDelayActive = false;
    [SerializeField] private bool activateOnDeath = false;
    [SerializeField] GameObject[] itemsToActivate;
    bool isAlive = true;
    public float searchDistance = 50f;
    public LayerMask playerLayer;
    private Rigidbody2D enemyRB;
    [SerializeField] bool isMiniBoss = false;
    private Coroutine attackCooldown;
    private Coroutine searchCooldown;
    private Coroutine death;
    private FrenzyManager frenzyManager;
    private bool rubberModeActive = false;
    [SerializeField] private CapsuleCollider2D catCollider1;
    [SerializeField] private CapsuleCollider2D catCollider2;

    public PlayerUI GetPlayerUI()
    {
        return UIClone.GetComponent<PlayerUI>();
    }

    public void RespawnPlayer()
    {
        isAlive = false;
        //Add to player frenzy meter here/

        if (isMiniBoss)
        {
            //GameObject.FindObjectOfType<FrenzyManager>()?.AddToFrenzyMeter(0.50f);
            frenzyManager.AddToFrenzyMeter(0.50f);
        }
        else
        {
            //GameObject.FindObjectOfType<FrenzyManager>()?.AddToFrenzyMeter(0.15f);
            frenzyManager.AddToFrenzyMeter(0.15f);
        }
        enemyAnimator.SetTrigger("Death");
        if(activateOnDeath)
        {
            foreach(var item in itemsToActivate)
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
        catCollider1.enabled = false;
        catCollider2.enabled = false;
        yield return new WaitForSeconds(0.267f);
        spriteRenderer.DOFade(0, 1f);
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }


    // Start is called before the first frame update
    void Awake()
    {
        startingPos = transform.position;
        if (isMiniBoss) UIClone = Instantiate(playerUI, transform.position + new Vector3(0, 2, 0), Quaternion.identity);
        else
        {
            UIClone = Instantiate(playerUI, transform.position + new Vector3(0, 1, 0), Quaternion.identity);
            UIClone.GetComponentInChildren<TextMeshProUGUI>().enabled = false;
        }
        UIClone.transform.parent = transform;
        //UIClone.GetComponent<Canvas>().worldCamera = Camera.main;
        health = GetComponent<Health>();
        //enemyAnimator = GetComponent<Animator>();
        targetPosition = ((Vector2)transform.position + (searchDirection* searchDistance));
        enemyRB = GetComponent<Rigidbody2D>();
        //spriteRenderer = GetComponent<SpriteRenderer>();    
    }

    private void Start()
    {
        //Find the victim
        if (GameObject.FindObjectOfType<PlayerController>() != null)
        {
            target = GameObject.FindObjectOfType<PlayerController>().gameObject;
        }
        frenzyManager = GameObject.FindObjectOfType<FrenzyManager>();
    }
    public Health GetHealth() { return health; }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(isAlive && !rubberModeActive)
        {
            UpdateState();
            spriteRenderer.flipX = enemyRB.velocity.x > 0;
            //Debug.Log(targetPosition);
            if(target == null)
            {
                target = GameObject.FindObjectOfType<PlayerController>().gameObject;
            }
        }
    }

    private void Update()
    {
        if(isAlive)
        {
            rubberModeActive = frenzyManager.inRubberMode;
            if (rubberModeActive)
            {
                //Enemy freezes and becomes intangible in rubbermode
                enemyRB.velocity = Vector3.zero;
                enemyRB.simulated = false;
                catCollider1.enabled = false;
                catCollider2.enabled = false;
            }
            else
            {
                enemyRB.simulated = true;
                catCollider1.enabled = true;
                catCollider2.enabled = true;
            }
        }
    }



    void UpdateState()
    {
        if (currentState == EnemyStates.Searching)
        {
            Debug.Log(Vector2.Distance((Vector2)target.transform.position, (Vector2)transform.position));
            if ((target != null) && Vector2.Distance((Vector2)target.transform.position, (Vector2)transform.position) <= sightRange)
            {
                Debug.Log("player in range");
                currentState = EnemyStates.Pursuing;
            }
            else
            {
                if(Vector2.Distance(targetPosition, (Vector2)transform.position) > 0.00000000001f)
                {
                    if (searchDelayActive == false)
                    {
                        Vector2 direction = targetPosition - (Vector2)transform.position;
                        direction.Normalize();
                        Debug.Log("moving");
                        enemyAnimator.SetBool("IsMoving", true);
                        //enemyRB.AddForce(direction * speed * Time.deltaTime, ForceMode2D.Force);
                        enemyRB.velocity = new Vector2(direction.x * speed, enemyRB.velocity.y);
                    }
                    else Debug.Log("Movement delay active");
                    
                }
                else
                {
                    Debug.Log("arrived");
                    enemyRB.velocity = Vector2.zero;
                    enemyAnimator.SetBool("IsMoving", false);
                    if (searchDirection == Vector2.left) searchDirection = Vector2.right;
                    else searchDirection = Vector2.left;
                    targetPosition = ((Vector2)transform.position + (searchDirection * searchDistance));
                    if (searchCooldown == null)
                    {
                        searchDelayActive = true;
                        searchCooldown = StartCoroutine(SearchDelayTimer());
                    }

                }
            }
        }
        else if(currentState == EnemyStates.Pursuing) 
        {
            Vector2 direction = (Vector2)target.transform.position - (Vector2)transform.position;
            direction.Normalize();
            enemyAnimator.SetBool("IsMoving", true);
            //enemyRB.AddForce(direction * speed * Time.deltaTime, ForceMode2D.Force);
            enemyRB.velocity = new Vector2(direction.x * speed, enemyRB.velocity.y);
            if (Vector2.Distance((Vector2)target.transform.position, (Vector2)transform.position) <= attackDistance)
            {
                targetPosition = startingPos;
                currentState = EnemyStates.Attacking;
                //Add here for projectile fire. Feature will be added in next semester. 
                //Projectile fire is a mini boss exclusive feature. In the vertical slice mini bosses just have more health and are bigger.
            }
            else if(Vector2.Distance((Vector2)target.transform.position, (Vector2)transform.position) > sightRange)
            {
                currentState = EnemyStates.Searching;
            }
        }
        else if(currentState == EnemyStates.Attacking) 
        {
            if (target != null && !attackDelayActive)
            {
                enemyRB.velocity = Vector2.zero;
                enemyAnimator.SetTrigger("Attack");
                target.GetComponent<Health>().SubtractFromHealth(5f);
                currentState = EnemyStates.Searching;
                targetPosition = (Vector2)target.transform.position;
                //only run coroutine if it isn't already active.
                if(attackCooldown == null) attackCooldown = StartCoroutine(AttackDelayTimer());
            }
            else
            {
                currentState = EnemyStates.Pursuing;
            }
        }
    }


    //Cooldown to prevent crazy unfair attacks and movement.
    IEnumerator SearchDelayTimer()
    {
        searchDelayActive = true;
        yield return new WaitForSeconds(searchDelay);
        searchDelayActive = false;
        searchCooldown = null;
    }

    IEnumerator AttackDelayTimer()
    {
        attackDelayActive = true;
        yield return new WaitForSeconds(attackDelay);
        attackDelayActive = false;
        attackCooldown = null;
    }

    public void PlayDamagedAnim()
    {
        enemyAnimator.SetTrigger("Damaged");
    }

    public PlayerController GetPlayerController()
    {
        throw new System.NotImplementedException();
    }
}


public enum EnemyStates
{
    Searching,
    Pursuing,
    Attacking,
    Perched,
    Idle,
    Landing
}