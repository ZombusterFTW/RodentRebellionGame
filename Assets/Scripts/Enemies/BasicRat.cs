using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using System.Linq;

public class BasicRat : MonoBehaviour, OneHitHealthEnemy
{
    [SerializeField] LayerMask surfaceLayer;
    [SerializeField] LayerMask playerLayer;
    [Tooltip("Set to true if you want the enemy to walk on the ceiling and have inverted physics.")][SerializeField] private bool isFlipped = false;
    [Tooltip("The distance the BasicRat Ai will move right and left from its starting position.")][SerializeField][Range(0.4f, 30f)] private float moveDistance = 5f;
    [Tooltip("The speed the BasicRat AI will move")][SerializeField][Range(1f, 15f)] private float moveSpeed = 7f;
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
   
    //private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
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
        if(isFlipped)
        {
            rigidBody.gravityScale = -1;
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
        if(isAlive) 
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
                        if (Vector2.Distance(transform.position, leftExtreme) > 0.1f)
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
                        if(Vector2.Distance(transform.position, rightExtreme) > 0.1f)
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
            frenzyManager.AddToFrenzyMeter(0.15f);
            isAlive = false;
            if (activateItemsOnDeath)
            {
                foreach (var item in itemsToActivate)
                {
                    item.GetComponent<R4Activatable>()?.Activate();
                }
            }
            frenzyManager.AddToFrenzyMeter(0.15f);
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
            PlayerController player = collision.gameObject.GetComponent<PlayerController>();
            Collider2D[] hitColliders;
            if (!player.isFlipped) hitColliders = Physics2D.OverlapCircleAll((Vector2)player.transform.position + player.bottomOffset, player.collisionRadius * 2f, playerLayer);
            else hitColliders = Physics2D.OverlapCircleAll((Vector2)player.transform.position + player.topOffset, player.collisionRadius*2f, playerLayer);
            bool ratUnderPlayer = false;
            foreach(var hit in hitColliders)
            {
                if(hit == capsuleCollider)
                {
                    ratUnderPlayer = true;
                    break;
                }
            }
            if(hitColliders.Length > 0 && ratUnderPlayer)
            {
                Debug.Log("Player landed on an enemy");
                OnRatDeath();

            }
            else
            {
                if(!player.isDashing && player.isGroundPounding)
                {
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
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = debugColor;
        var positions = new Vector2[] { rightOffset, leftOffset };
        //Gizmos.DrawSphere((Vector2)transform.position + rightOffset, collisionRadius);
        //Gizmos.DrawSphere((Vector2)transform.position + leftOffset, collisionRadius);
        Gizmos.DrawSphere((Vector2)transform.position + new Vector2(moveDistance,0), collisionRadius/2);
        Gizmos.DrawSphere((Vector2)transform.position - new Vector2(moveDistance,0), collisionRadius/2);
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
    Idle
}

public interface OneHitHealthEnemy
{
    //Use this interface if an enemy must die when they take damage
    public void OnOneHitEnemyDeath();
}