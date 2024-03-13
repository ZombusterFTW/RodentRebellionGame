using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using System.Linq;

public class BasicRat : MonoBehaviour, OneHitHealthEnemy
{
    [SerializeField] bool drawGizmos = true;
    [SerializeField] LayerMask surfaceLayer;
    [SerializeField] LayerMask playerLayer;
    [Tooltip("Set to true if you want the enemy to walk on the ceiling and have inverted physics.")][SerializeField] private bool isFlipped = false;
    [Tooltip("The distance the BasicRat Ai will move right and left from its starting position.")][SerializeField][Range(1.1f, 30f)] private float moveDistance = 5f;
    [Tooltip("The speed the BasicRat AI will move")][SerializeField][Range(1f, 15f)] private float moveSpeed = 7f;
    [Tooltip("Set to true if you want the BasicRat to activate items on its death")][SerializeField] private bool activateItemsOnDeath = false;
    [Tooltip("Add the wanted activated items to this list. These items must integrate the R4 Activatable interface")][SerializeField] private GameObject[] itemsToActivate;
    [Tooltip("Set this float to the damage a player will take from this enemy")][SerializeField] private float damageToPlayer = 3.5f;
    [Tooltip("The percentage of the frenzy bar killing the enemy will fill")][SerializeField][Range(0.0f, 1.0f)] private float frenzyPercentageFill = 0.15f;
    [SerializeField] private BasicRatAIStates currentState = BasicRatAIStates.Idle;
    private float collisionRadius = 0.25f;
    private Vector2 rightOffset, leftOffset, bottomOffset, topOffset;
    private FrenzyManager frenzyManager;
    private Rigidbody2D rigidBody;
    private Collider2D capsuleCollider;
    [SerializeField] private SpriteRenderer spriteRendererRubber;
    [SerializeField] private SpriteRenderer spriteRendererRat;
    private bool isAlive = true;
    private Color debugColor = Color.red;
    private bool onLeftWall = false;
    private bool onRightWall = false;
    private bool onGround = false;
    private Vector2 startingPos, leftExtreme, rightExtreme;
    private Coroutine extendedDamage;
   
    //private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        collisionRadius = 0.15f;
        rightOffset = new Vector2(0.35f, 0.1f);
        leftOffset = new Vector2(-0.35f, 0.1f);
        bottomOffset = new Vector2(0, -.2f);
        topOffset = new Vector2(0, .3f);
        //Calculate the spots the AI will move to.
        moveDistance = Mathf.Abs(moveDistance);
        moveSpeed = Mathf.Abs(moveSpeed);
        startingPos = transform.position;
        leftExtreme = startingPos - new Vector2(moveDistance, 0);
        rightExtreme = startingPos + new Vector2(moveDistance, 0);
        frenzyManager = FrenzyManager.instance;
        capsuleCollider = GetComponent<Collider2D>();
        rigidBody = GetComponent<Rigidbody2D>();
        if(isFlipped)
        {
            spriteRendererRubber.flipY = true;
            spriteRendererRat.flipY = true;
        }
        //Roll random 1/2 chance to start going left or right.
        if(Random.Range(0,2) == 0) 
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
        
    }
    private void FixedUpdate()
    {
        //This function will check if the AI has bumped against a wall and if it has, we instantly change the AI's movement direction.
        if (isAlive) CheckAICollision();
        if (isAlive)
        {
            if (frenzyManager.inRubberMode)
            {
                rigidBody.velocity = Vector2.zero;
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

    private void UpdateAIState()
    {
        //Debug.Log(Vector2.Distance(transform.position, new Vector2(rightExtreme.x, transform.position.y)));

        //Debug.Log(Vector2.Distance(transform.position, new Vector2(leftExtreme.x, transform.position.y)));

        //Simulate gravity
        if (!onGround)
        {
            if (!isFlipped) rigidBody.velocity = new Vector2(rigidBody.velocity.x, -5);
            else rigidBody.velocity = new Vector2(rigidBody.velocity.x, 5);
        }
        else rigidBody.velocity = new Vector2(rigidBody.velocity.x, 0);

        switch(currentState)
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
                        float temp = ((Vector2)transform.position - new Vector2(leftExtreme.x, transform.position.y)).normalized.x;
                        if (temp > 0 || isFlipped && temp < 0)
                        {
                            //Debug.Log(Vector2.Distance(transform.position, leftExtreme));
                            spriteRendererRat.flipX = false;
                            spriteRendererRubber.flipX = false;
                            //transform.position = Vector2.MoveTowards(transform.position, leftExtreme, Time.deltaTime * moveSpeed);
                            rigidBody.velocity = new Vector2(-moveSpeed, rigidBody.velocity.y); 
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
                    if(onRightWall) 
                    {
                        //Invert direction.
                        currentState = BasicRatAIStates.MovingLeft;
                    }
                    else
                    {
                        float temp = ((Vector2)transform.position - new Vector2(rightExtreme.x, transform.position.y)).normalized.x;
                        if (temp < 0 || isFlipped && temp > 0)
                        {
                            //Debug.Log(Vector2.Distance(transform.position, rightExtreme));
                            spriteRendererRat.flipX = true;
                            spriteRendererRubber.flipX = true;
                            //transform.position = Vector2.MoveTowards(transform.position, rightExtreme, Time.deltaTime * moveSpeed);
                            rigidBody.velocity = new Vector2(moveSpeed, rigidBody.velocity.y);
                        }
                        else
                        {
                            currentState = BasicRatAIStates.MovingLeft;
                        }
                    }
                    break;
                }
        }
    }


    void OnRatDeath()
    {
        if(isAlive)
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


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(!GameObject.ReferenceEquals(collision.gameObject.GetComponent<PlayerController>(), null))
        {

            //To ensure the rat can be dispatched by a groundpound.

            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            Collider2D[] hitColliders;
            if (!player.isFlipped) hitColliders = Physics2D.OverlapCircleAll((Vector2)player.transform.position + player.bottomOffset, player.collisionRadius * 2.5f, playerLayer);
            else hitColliders = Physics2D.OverlapCircleAll((Vector2)player.transform.position + player.topOffset, player.collisionRadius*2.5f, playerLayer);
            bool ratUnderPlayer = false;
            foreach(var hit in hitColliders)
            {
                if(hit == capsuleCollider)
                {
                    ratUnderPlayer = true;
                    break;
                }
            }
            if(hitColliders.Length > 0 && ratUnderPlayer && player.isGroundPounding)
            {
                Debug.Log("Player landed on an enemy");
                OnRatDeath();

            }
            else
            {
                if(!player.isDashing)
                {
                    Debug.Log("Player hit by rat");
                    //The player must take damage here.
                    collision.gameObject.GetComponent<Health>().SubtractFromHealth(damageToPlayer);
                    if (extendedDamage == null) extendedDamage = StartCoroutine(PlayerDamageLoop(collision.gameObject.GetComponent<Health>()));
                }
            }
        }
    }
    IEnumerator PlayerDamageLoop(Health playerHealth)
    {
        while(isAlive)
        {
            yield return new WaitForSeconds(2);
            playerHealth.SubtractFromHealth(damageToPlayer);
        }
        
    }


    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!GameObject.ReferenceEquals(collision.gameObject.GetComponent<PlayerController>(), null))
        {
            if(!Coroutine.ReferenceEquals(extendedDamage, null)) StopCoroutine(extendedDamage);
        }
    }

    private void CheckAICollision()
    {
        onRightWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, collisionRadius, surfaceLayer);
        onLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, collisionRadius, surfaceLayer);
        if(!isFlipped) onGround = Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset, collisionRadius/3, surfaceLayer);
        else onGround = Physics2D.OverlapCircle((Vector2)transform.position + topOffset, collisionRadius/3, surfaceLayer);
    }

    private void OnDrawGizmos()
    {
        if(drawGizmos)
        {
            Gizmos.color = debugColor;
            //var positions = new Vector2[] { rightOffset, leftOffset };
            //Gizmos.DrawSphere((Vector2)transform.position + rightOffset, collisionRadius);
            //Gizmos.DrawSphere((Vector2)transform.position + leftOffset, collisionRadius);
           // Gizmos.DrawSphere((Vector2)transform.position + bottomOffset, collisionRadius);
            Gizmos.DrawSphere((Vector2)transform.position + new Vector2(moveDistance, 0), collisionRadius);
            Gizmos.DrawSphere((Vector2)transform.position - new Vector2(moveDistance, 0), collisionRadius);
        }
    }

    public void OnOneHitEnemyDeath()
    {
        OnRatDeath();
    }
}

public enum BasicRatAIStates
{
    MovingLeft,
    MovingRight,
    Idle,
    Pursuing,
    Attack
}

public interface OneHitHealthEnemy
{
    //Use this interface if an enemy must die when they take damage
    public void OnOneHitEnemyDeath();
}