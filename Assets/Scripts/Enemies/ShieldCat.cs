using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShieldCat : MonoBehaviour, ControlledCharacter, EnemyAI
{
    private Vector2 bottomOffset = new Vector2(0, -0.60f);
    [SerializeField] LayerMask ignore;
    [SerializeField] bool drawGizmos = true;
    [SerializeField] GameObject playerUI;
    [SerializeField] LayerMask surfaceLayer;
    [SerializeField] LayerMask playerLayer;
    [Tooltip("The Golem sight range for the raycast. The player must be in range and seeable")][SerializeField] private float sightRange = 350f;
    //[Tooltip("Set to true if you want the enemy to walk on the ceiling and have inverted physics.")][SerializeField] private bool isFlipped = false;
    [Tooltip("The time the Shield Cat will be idle after its charge completes")][SerializeField] private float chargeCooldownTime = 2.5f;
    [Tooltip("The speed the Shield Cat AI will move during its charge")][SerializeField][Range(5f, 15f)] private float chargeMoveSpeed = 7f;
    [Tooltip("The speed the Shield Cat AI will move during its windup")][SerializeField][Range(0.1f, 0.2f)] private float windUpMoveSpeed = 0.1f;
    [Tooltip("Set to true if you want the Shield Cat AI to activate items on its death")][SerializeField] private bool activateItemsOnDeath = false;
    [Tooltip("Add the wanted activated items to this list. These items must integrate the R4 Activatable interface")][SerializeField] private GameObject[] itemsToActivate;
    [Tooltip("Set this float to the damage a player will take from this enemy")][SerializeField] private float damageToPlayer = 55f;
    [Tooltip("The percentage of the frenzy bar killing the enemy will fill")][SerializeField][Range(0.0f, 1.0f)] private float frenzyPercentageFill = 0.15f;
    [Tooltip("How long a shield cat can charge before it is considered out of the map or bugged. If this number is hit the AI is destroyed")][SerializeField] private float maxChargeTimeValue = 15f;
    [SerializeField] private ShieldCatStates currentState = ShieldCatStates.Idle;
    private float collisionRadius = 0.26f;
    [SerializeField] private Vector2 rightOffset, leftOffset, startingPos;
    private FrenzyManager frenzyManager;
    private Rigidbody2D rigidBody;
    private CapsuleCollider2D capsuleCollider;
    [SerializeField] private GameObject shieldLeft, shieldRight;
    private GameObject activeShield;
    private Collider2D activeShieldCollider;
    [SerializeField] private SpriteRenderer spriteRendererRubber;
    [SerializeField] private SpriteRenderer spriteRendererRat;
    private bool isAlive = true;
    private Color debugColor = Color.red;
    private bool onLeftWall = false;
    private bool onRightWall = false;
    private bool isFacingLeft = true;
    private GameObject UIClone;
    private Health health;
    private Health playerHealthObj;
    PlayerController playerController;
    private bool pursueCooldown = false;
    private float timeOnSide = 0;
    private float timeCharging = 0;
    private Vector2 playerDirection;
    private Vector2 chargePos;
    private BoxCollider2D playerCollider;

    //Like basic rat but chonky!

    private void Awake()
    {
        startingPos = transform.position;
        UIClone = Instantiate(playerUI, transform.position + new Vector3(0, 1, 0), Quaternion.identity);
        UIClone.GetComponentInChildren<TextMeshProUGUI>().enabled = false;
        UIClone.transform.parent = transform;
        health = GetComponent<Health>();
    }
    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        playerHealthObj = playerController.GetComponent<Health>();
        frenzyManager = FrenzyManager.instance;
        rigidBody = GetComponent<Rigidbody2D>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        //Roll to determine which direction the cat will face
        if (Random.Range(0, 2) == 0)
        {
            shieldRight.SetActive(false);
            shieldLeft.SetActive(true);
            isFacingLeft = true;
            activeShield = shieldLeft;
        }
        else
        {
            shieldRight.SetActive(true);
            shieldLeft.SetActive(false);
            isFacingLeft = false;
            activeShield = shieldRight;
        }
        activeShieldCollider = activeShield.GetComponent<CapsuleCollider2D>();
        playerCollider = playerController.GetPlayerCollider();
    }
    //This cat will begin idle and completely static. When a player enters its sight range it begins to move towards them slowly but then breaks into a fast charge in the direction of the player. 
    //This directional charge cannot be canceled unless the shield cat hits a wall or a player. When the shield cat hits a wall or player its facing direction changes and it has a cooldown before it can move again. 
    //If a player is hit by a charge deal massive damage.
    // Update is called once per frame
    void Update()
    {
        if(isAlive) 
        {
            spriteRendererRat.flipX = rigidBody.velocity.x > 0;
            spriteRendererRubber.flipX = rigidBody.velocity.x > 0;
            if (frenzyManager.inRubberMode)
            {
                rigidBody.simulated = false;
                capsuleCollider.enabled = false;
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

    void FixedUpdate()
    {
        //Check for wall collsions here.
        CheckAICollision();
    }





    private void UpdateAIState()
    {
        switch(currentState) 
        {
            case ShieldCatStates.Idle:
                //Shield cat is entirely idle until it sees a player. We need an idle cooldown that will serve as the charge "cooldown"
                RaycastHit2D hit = Physics2D.Linecast((Vector2)transform.position + bottomOffset, playerController.gameObject.transform.position, ~ignore);
                if (!GameObject.ReferenceEquals(hit.collider.gameObject.GetComponent<PlayerController>(), null) && Vector2.Distance(gameObject.transform.position, playerController.gameObject.transform.position) <= sightRange && !pursueCooldown)
                {
                    Debug.DrawLine((Vector2)transform.position + bottomOffset, playerController.gameObject.transform.position, Color.green);
                    //Player was spotted. proceed to wind up state.
                    playerDirection = transform.position - playerController.gameObject.transform.position;
                    playerDirection.Normalize();
                    chargePos = transform.position;
                    currentState = ShieldCatStates.SwapDirection;
                    return;
                }
                break;
            case ShieldCatStates.WindUp:
                //Need to see how long the player is in front of it and if we run over it then the charge occurs
                RaycastHit2D playerHitCheck = Physics2D.Linecast((Vector2)transform.position + bottomOffset, playerController.gameObject.transform.position, ~ignore);
                if (!GameObject.ReferenceEquals(playerHitCheck.collider.gameObject.GetComponent<PlayerController>(), null) && Vector2.Distance(gameObject.transform.position, playerController.gameObject.transform.position) <= sightRange)
                {
                    Debug.DrawLine((Vector2)transform.position + bottomOffset, playerController.gameObject.transform.position, Color.green);
                    //If the player direction x coordinate is the same the player is on the same side of the shield cat so we accrue deltatime
                    if(playerDirection.normalized.x == (chargePos - (Vector2)playerController.gameObject.transform.position).normalized.x)
                    {
                        //Once we have accured enough delta time we charge in the set direction.
                        timeOnSide += Time.deltaTime;
                        if (timeOnSide >= 1)
                        {
                            timeOnSide = 0;
                            currentState = ShieldCatStates.Charging;
                        }
                        else
                        {
                            if ((isFacingLeft && onRightWall) || (!isFacingLeft && onLeftWall))
                            {
                                rigidBody.velocity = new Vector2(-playerDirection.x * windUpMoveSpeed, rigidBody.velocity.y);
                                playerDirection = chargePos - (Vector2)playerController.gameObject.transform.position;
                                playerDirection.Normalize();
                            }
                        }
                    }
                    else
                    {
                        timeOnSide = 0;
                        playerDirection = transform.position - playerController.gameObject.transform.position;
                        playerDirection.Normalize();
                        chargePos = transform.position;
                        currentState = ShieldCatStates.SwapDirection;
                    }
                }
                return;
            case ShieldCatStates.Charging:
                //Charge until a wall or player is hit. 
                if(!onLeftWall && !onRightWall)
                {
                    rigidBody.velocity = new Vector2(-playerDirection.x * chargeMoveSpeed, rigidBody.velocity.y);
                    timeCharging += Time.deltaTime;
                }
                else
                {
                    timeCharging = 0;
                    rigidBody.velocity = Vector3.zero;
                    if(isFacingLeft)
                    {
                        if(Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, collisionRadius, playerLayer))
                        {
                            playerController.GetHealthComponent().SubtractFromHealth(damageToPlayer);
                        }
                    }
                    else
                    {
                        if(Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, collisionRadius, playerLayer))
                        {
                            playerController.GetHealthComponent().SubtractFromHealth(damageToPlayer);
                        }
                    }
                    //When we hit something go to idle.
                    currentState = ShieldCatStates.Idle;
                    StartCoroutine(ChargingCooldown());
                }
                //Kill cat if charging for too long. Failsafe.
                if(timeCharging > maxChargeTimeValue) OnCatDeath();
                return;
            case ShieldCatStates.SwapDirection:
                //Update the shield direction so it faces the player accurately. 
                rigidBody.velocity = Vector3.zero;
                Vector2 directionOfPlayer = transform.position - playerController.transform.position;
                directionOfPlayer.Normalize();
                if(directionOfPlayer.x > 0)
                {
                    //Facing right
                    shieldRight.SetActive(false);
                    shieldLeft.SetActive(true);
                    isFacingLeft = true;
                    activeShield = shieldRight;
                }
                else
                {
                    //Facing left
                    shieldRight.SetActive(true);
                    shieldLeft.SetActive(false);
                    isFacingLeft = false;
                    activeShield = shieldLeft;
                }
                activeShieldCollider = activeShield.GetComponent<CapsuleCollider2D>();
                currentState = ShieldCatStates.WindUp;
                return;
        }
    }



    IEnumerator ChargingCooldown()
    {
        pursueCooldown = true;
        yield return new WaitForSeconds(chargeCooldownTime);
        pursueCooldown = false;
    }



    private void OnCollisionEnter2D(Collision2D collision)
    {
        
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        
    }




    private void CheckAICollision()
    {
        onRightWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, collisionRadius, surfaceLayer);
        onLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, collisionRadius, surfaceLayer);
        if (!Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset, collisionRadius, surfaceLayer))
        {
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, -5f);
        }
        else
        {
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, 0);
        }
    }

    private void OnDrawGizmos()
    {
        if (drawGizmos)
        {
            Gizmos.color = debugColor;
            var positions = new Vector2[] { rightOffset, leftOffset };
            Gizmos.DrawSphere((Vector2)transform.position + rightOffset, collisionRadius);
            Gizmos.DrawSphere((Vector2)transform.position + leftOffset, collisionRadius);
            Gizmos.DrawSphere((Vector2)transform.position, sightRange);
        }
    }


    private void OnCatDeath()
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
            activeShield.GetComponent<SpriteRenderer>().DOFade(0, 1);
            Destroy(gameObject, 1.5f);
        }
    }
    public Health GetHealth()
    {
        return health;
    }
    public PlayerController GetPlayerController()
    {
        return null;
    }
    public PlayerUI GetPlayerUI()
    {
        return UIClone.GetComponent<PlayerUI>();
    }
    public void PlayDamagedAnim()
    {
        //enemyAnimatorRat.SetTrigger("Damaged");
        //enemyAnimatorRubber.SetTrigger("Damaged");
    }
    public void RespawnPlayer()
    {
        OnCatDeath();
    }
}


public enum ShieldCatStates
{
    Idle,
    Charging,
    WindUp,
    SwapDirection,
    Stunned
}