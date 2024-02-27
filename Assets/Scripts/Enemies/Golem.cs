using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class Golem : MonoBehaviour, ControlledCharacter, EnemyAI
{
    private Vector2 bottomOffset = new Vector2(0, -0.60f);
    [SerializeField] LayerMask ignore;
    [SerializeField] bool drawGizmos = true;
    [SerializeField] GameObject playerUI;
    [SerializeField] LayerMask surfaceLayer;
    [SerializeField] LayerMask playerLayer;
    [Tooltip("The Golem sight range for the raycast. The player must be in range and seeable")][SerializeField] private float sightRange = 350f;
    [Tooltip("The delay before the Golem can begin pursuit if its los of the player is lost")][SerializeField] private float pursueBreakDelay = 3f;
    //[Tooltip("Set to true if you want the enemy to walk on the ceiling and have inverted physics.")][SerializeField] private bool isFlipped = false;
    [Tooltip("The distance the BasicRat Ai will move right and left from its starting position.")][SerializeField][Range(0.4f, 30f)] private float moveDistance = 5f;
    [Tooltip("The speed the BasicRat AI will move")][SerializeField][Range(0.2f, 1f)] private float moveSpeed = 7f;
    [Tooltip("Set to true if you want the BasicRat to activate items on its death")][SerializeField] private bool activateItemsOnDeath = false;
    [Tooltip("Add the wanted activated items to this list. These items must integrate the R4 Activatable interface")][SerializeField] private GameObject[] itemsToActivate;
    [Tooltip("Set this float to the damage a player will take from this enemy")][SerializeField] private float damageToPlayer = 3.5f;
    [Tooltip("The percentage of the frenzy bar killing the enemy will fill")][SerializeField][Range(0.0f, 1.0f)] private float frenzyPercentageFill = 0.15f;
    [SerializeField] private BasicRatAIStates currentState = BasicRatAIStates.Idle;
    private float collisionRadius = 0.26f;
    private Vector2 rightOffset, leftOffset;
    private FrenzyManager frenzyManager;
    private Rigidbody2D rigidBody;
    private CapsuleCollider2D capsuleCollider;
    [SerializeField] private SpriteRenderer spriteRendererRubber;
    [SerializeField] private SpriteRenderer spriteRendererRat;
    private bool isAlive = true;
    private Color debugColor = Color.red;
    private bool onLeftWall = false;
    private bool onRightWall = false;
    private Vector2 startingPos, leftExtreme, rightExtreme;
    private Coroutine extendedDamage;
    private GameObject UIClone;
    private Health health;
    private Health playerHealthObj;
    PlayerController playerController;
    private bool pursueCooldown = false;
    private Coroutine pursueCooldownTimer;

    //Like basic rat but chonky!

    private void Awake()
    {
        UIClone = Instantiate(playerUI, transform.position + new Vector3(0, 1, 0), Quaternion.identity);
        UIClone.GetComponentInChildren<TextMeshProUGUI>().enabled = false;
        UIClone.transform.parent = transform;
    }



    //Will pursue players within its relatively short sight range. When a player is out of range it creates a new patrol range starting from the position it lost sight of the player. Use majority logic of basic rat but add the health component.
    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        playerHealthObj = playerController.GetComponent<Health>();
        health = GetComponent<Health>();
        rightOffset = new Vector2(0.45f, 0);
        leftOffset = new Vector2(-0.45f, 0);
        //Calculate the spots the AI will move to.
        moveDistance = Mathf.Abs(moveDistance);
        moveSpeed = Mathf.Abs(moveSpeed);
        startingPos = transform.position;
        leftExtreme = startingPos - new Vector2(moveDistance, 0);
        rightExtreme = startingPos + new Vector2(moveDistance, 0);
        frenzyManager = FrenzyManager.instance;
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        rigidBody = GetComponent<Rigidbody2D>();
        /*
        if (isFlipped)
        {
            rigidBody.gravityScale = -1;
            spriteRendererRubber.flipY = true;
            spriteRendererRat.flipY = true;
        }
        */
        //Roll random 1/2 chance to start going left or right.
        if (Random.Range(0, 2) == 0)
        {
            spriteRendererRubber.flipX = false;
            spriteRendererRat.flipX = false;
            currentState = BasicRatAIStates.MovingRight;
        }
        else
        {
            spriteRendererRubber.flipX = true;
            spriteRendererRat.flipX = true;
            currentState = BasicRatAIStates.MovingLeft;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isAlive)
        {
            if (frenzyManager.inRubberMode)
            {
                rigidBody.simulated = false;
                capsuleCollider.enabled = false;
                if (!Coroutine.ReferenceEquals(extendedDamage, null)) StopCoroutine(extendedDamage);
            }
            else
            {
                rigidBody.simulated = true;
                capsuleCollider.enabled = true;
                if (Vector2.Distance(startingPos, transform.position) < 750f) UpdateAIState();
                else Destroy(gameObject);
            }
        }
    }

    private void FixedUpdate()
    {
        //This function will check if the AI has bumped against a wall and if it has, we instantly change the AI's movement direction.
        if (isAlive) CheckAICollision();
    }
    private void UpdateAIState()
    {

        if (!Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset, collisionRadius, surfaceLayer))
        {
            rigidBody.velocity = new Vector2(rigidBody.velocity.y, -5f);
        }
        else
        {
            rigidBody.velocity = new Vector2(rigidBody.velocity.y, 0);
        }

        switch (currentState)
        {
            case BasicRatAIStates.MovingLeft:
                {
                    if (onLeftWall)
                    {
                        //Invert direction
                        currentState = BasicRatAIStates.MovingRight;
                    }
                    else
                    {
                        if (Vector2.Distance(transform.position, leftExtreme) > 0.1f)
                        {
                            //Debug.Log(Vector2.Distance(transform.position, leftExtreme));
                            spriteRendererRat.flipX = false;
                            spriteRendererRubber.flipX = false;
                            //transform.position = Vector2.MoveTowards(transform.position, leftExtreme, Time.deltaTime * moveSpeed);
                            rigidBody.velocity = new Vector2(-moveSpeed, rigidBody.velocity.y);
                            RaycastHit2D hit = Physics2D.Linecast((Vector2)transform.position + bottomOffset, playerController.gameObject.transform.position, ~ignore);
                            if (!GameObject.ReferenceEquals(hit.collider.gameObject.GetComponent<PlayerController>(), null) && Vector2.Distance(gameObject.transform.position, playerController.gameObject.transform.position) <= sightRange && !pursueCooldown)
                            {
                                currentState = BasicRatAIStates.Pursuing;
                            }
                        }
                        else
                        {
                            currentState = BasicRatAIStates.MovingRight;
                        }
                    }
                    break;
                }
            case BasicRatAIStates.MovingRight:
                {
                    if (onRightWall)
                    {
                        //Invert direction.
                        currentState = BasicRatAIStates.MovingLeft;
                    }
                    else
                    {
                        if (Vector2.Distance(transform.position, rightExtreme) > 0.1f)
                        {
                            //Debug.Log(Vector2.Distance(transform.position, rightExtreme));
                            spriteRendererRat.flipX = true;
                            spriteRendererRubber.flipX = true;
                            //transform.position = Vector2.MoveTowards(transform.position, rightExtreme, Time.deltaTime * moveSpeed);
                            rigidBody.velocity = new Vector2(moveSpeed, rigidBody.velocity.y);
                            RaycastHit2D hit = Physics2D.Linecast((Vector2)transform.position + bottomOffset, playerController.gameObject.transform.position, ~ignore);
                            if (!GameObject.ReferenceEquals(hit.collider.gameObject.GetComponent<PlayerController>(), null) && Vector2.Distance(gameObject.transform.position, playerController.gameObject.transform.position) <= sightRange && !pursueCooldown)
                            {
                                currentState = BasicRatAIStates.Pursuing;
                            }
                        }
                        else
                        {
                            currentState = BasicRatAIStates.MovingLeft;
                        }
                    }
                    break;
                }
            case BasicRatAIStates.Pursuing:
                {
                    Vector2 direction = (Vector2)playerController.transform.position - (Vector2)transform.position;
                    direction.Normalize();
                    Debug.Log("moving");
                    //enemyAnimatorRubber.SetBool("IsMoving", true);
                   // enemyAnimatorRat.SetBool("IsMoving", true);
                    //enemyRB.AddForce(direction * speed * Time.deltaTime, ForceMode2D.Force);
                    RaycastHit2D hit = Physics2D.Linecast((Vector2)transform.position + bottomOffset, playerController.gameObject.transform.position, ~ignore);
                    if (!GameObject.ReferenceEquals(hit.collider.gameObject.GetComponent<PlayerController>(), null) && Vector2.Distance(gameObject.transform.position, playerController.gameObject.transform.position) <= sightRange)
                    {
                        Debug.DrawLine((Vector2)transform.position + bottomOffset, playerController.gameObject.transform.position, Color.green);

                        if(capsuleCollider.IsTouching(playerController.GetPlayerCollider()))
                        {
                            rigidBody.velocity = new Vector2(0, rigidBody.velocity.y);
                            currentState = BasicRatAIStates.Attack;
                        }
                        else
                        {
                            rigidBody.velocity = new Vector2(direction.x * moveSpeed, rigidBody.velocity.y);
                            spriteRendererRat.flipX = rigidBody.velocity.x > 0;
                            spriteRendererRubber.flipX = rigidBody.velocity.x > 0;
                        }
                    }
                    else
                    {
                        Debug.DrawLine((Vector2)transform.position + bottomOffset, playerController.gameObject.transform.position, Color.red);
                        if (direction.x < 0) currentState = BasicRatAIStates.MovingRight;
                        else currentState = BasicRatAIStates.MovingLeft;
                        if (pursueCooldownTimer == null) pursueCooldownTimer = StartCoroutine(PursueDelay());
                    }
                    break;
                }
            case BasicRatAIStates.Attack:
                {
                    if (extendedDamage == null) extendedDamage = StartCoroutine(PlayerDamageLoop());
                    currentState = BasicRatAIStates.Pursuing;
                    break;
                }
        }
    }


    void OnRatDeath()
    {
        if (isAlive)
        {
            frenzyManager.AddToFrenzyMeter(frenzyPercentageFill);
            isAlive = false;
            if (activateItemsOnDeath)
            {
                foreach (var item in itemsToActivate)
                {
                    item.GetComponent<R4Activatable>()?.Activate();
                }
            }
            rigidBody.simulated = false;
            capsuleCollider.enabled = false;
            //Fade out spriteRenderers
            spriteRendererRat.DOFade(0, 1);
            spriteRendererRubber.DOFade(0, 1);
            Destroy(gameObject, 1.5f);
        }
    }

    IEnumerator PursueDelay()
    {
        pursueCooldown = true;
        yield return new WaitForSeconds(pursueBreakDelay);
        pursueCooldown = false;
        pursueCooldownTimer = null;
    }

    IEnumerator PlayerDamageLoop()
    {
        yield return new WaitForSeconds(0.1f);
        if (capsuleCollider.IsTouching(playerController.GetPlayerCollider()))
        {
            playerHealthObj.SubtractFromHealth(damageToPlayer);
            yield return new WaitForSeconds(0.45f);
        }
        extendedDamage = null;
    }



    private void CheckAICollision()
    {
        onRightWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, collisionRadius, surfaceLayer);
        onLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, collisionRadius, surfaceLayer);
    }



    private void OnDrawGizmos()
    {
        if (drawGizmos)
        {
            Gizmos.color = new Color(1,0,0,0.5f);
            var positions = new Vector2[] { rightOffset, leftOffset };
            //Gizmos.DrawSphere((Vector2)transform.position + rightOffset, collisionRadius);
            //Gizmos.DrawSphere((Vector2)transform.position + leftOffset, collisionRadius);
            Gizmos.DrawSphere((Vector2)transform.position + new Vector2(moveDistance, 0), collisionRadius / 2);
            Gizmos.DrawSphere((Vector2)transform.position - new Vector2(moveDistance, 0), collisionRadius / 2);
            Gizmos.DrawSphere((Vector2)transform.position, sightRange);
            Gizmos.DrawSphere((Vector2)transform.position + bottomOffset, collisionRadius);
            Gizmos.color = Color.white;
            Gizmos.DrawSphere((Vector2)transform.position + new Vector2(moveDistance, 0), collisionRadius / 2);
            Gizmos.DrawSphere((Vector2)transform.position - new Vector2(moveDistance, 0), collisionRadius / 2);
        }
    }

    public Health GetHealth()
    {
        return health;
    }

    public PlayerUI GetPlayerUI()
    {
        return UIClone.GetComponent<PlayerUI>();
    }

    public void RespawnPlayer()
    {
        OnRatDeath();
    }

    public void PlayDamagedAnim()
    {
        //enemyAnimatorRat.SetTrigger("Damaged");
        //enemyAnimatorRubber.SetTrigger("Damaged");

        //For this enemy extended damage is an indentical coroutine to the one in the spidercat script. This one is combined with a delayed attack and an attack cooldown.
        if (!Coroutine.ReferenceEquals(extendedDamage, null))
        {
            StopCoroutine(extendedDamage);
            extendedDamage = null;
        }
    }

    public PlayerController GetPlayerController()
    {
        throw new System.NotImplementedException();
    }
}
