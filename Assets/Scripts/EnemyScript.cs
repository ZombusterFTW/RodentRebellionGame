using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class EnemyScript : MonoBehaviour, ControlledCharacter
{
    [SerializeField] GameObject playerUI;
    private GameObject UIClone;
    private Health health;
    private SpriteRenderer spriteRenderer;
    private Animator enemyAnimator;
    public EnemyStates currentState = EnemyStates.Searching;
    private GameObject target = null;
    private Vector2 searchDirection = Vector2.left;
    private Vector2 targetPosition;
    public float sightRange = 25.0f;
    public float speed = 50.0f;
    public float attackDistance = 5f;
    public float searchDistance;
    public LayerMask playerLayer;
    private Rigidbody2D enemyRB;

    public PlayerUI GetPlayerUI()
    {
        return UIClone.GetComponent<PlayerUI>();
    }

    public void RespawnPlayer()
    {
        Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Awake()
    {
        UIClone = Instantiate(playerUI, transform.position + new Vector3(0,2,0), Quaternion.identity);
        UIClone.transform.parent = transform;
        //UIClone.GetComponent<Canvas>().worldCamera = Camera.main;
        health = GetComponent<Health>();
        enemyAnimator = GetComponent<Animator>();
        targetPosition = transform.position;
        enemyRB = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();    
    }
    public Health GetHealth() { return health; }

    // Update is called once per frame
    void Update()
    {

        UpdateState();
        spriteRenderer.flipX = enemyRB.velocity.x > 0;
    }



    void UpdateState()
    {
        if (currentState == EnemyStates.Searching)
        {
            if (Physics2D.Raycast((Vector2)transform.position, Vector2.left, sightRange, playerLayer) || Physics2D.Raycast((Vector2)transform.position, Vector2.right, sightRange, playerLayer))
            {
                RaycastHit2D hitLeft = Physics2D.Raycast((Vector2)transform.position, Vector2.left, sightRange, playerLayer);
                RaycastHit2D hitRight = Physics2D.Raycast((Vector2)transform.position, Vector2.right, sightRange, playerLayer);

                if (hitLeft) target = hitLeft.collider.gameObject;
                if (hitRight) target = hitRight.collider.gameObject;
                if (target != null) currentState = EnemyStates.Pursuing;
            }
            else
            {
                Vector2 direction = targetPosition - (Vector2)transform.position;
                if((Vector2)transform.position != targetPosition)
                {
                    enemyAnimator.SetBool("IsMoving", true);
                    enemyRB.AddForce(direction*speed, ForceMode2D.Force);
                }
                else
                {
                    enemyAnimator.SetBool("IsMoving", false);
                    if (searchDirection == Vector2.left) searchDirection = Vector2.right;
                    else searchDirection = Vector2.left;
                    targetPosition = ((Vector2)transform.position + searchDirection) * searchDistance;
                    
                }
            }
        }
        else if(currentState == EnemyStates.Pursuing) 
        {
            Vector2 direction = (Vector2)target.transform.position - (Vector2)transform.position;
            enemyAnimator.SetBool("IsMoving", true);
            enemyRB.AddForce(direction * speed, ForceMode2D.Force);
            if (Vector2.Distance((Vector2)target.transform.position, (Vector2)transform.position) <= attackDistance)
            {
                targetPosition = gameObject.transform.position;
                currentState = EnemyStates.Attacking;
            }
        }
        else if(currentState == EnemyStates.Attacking) 
        {
            if (target != null)
            {
                enemyAnimator.SetTrigger("Attack");
                target.GetComponent<Health>().SubtractFromHealth(5f);
                currentState = EnemyStates.Searching;
                targetPosition = (Vector2)target.transform.position;
            }
        }



    }

    public void PlayDamagedAnim()
    {
        enemyAnimator.SetTrigger("Damaged");
    }
}


public enum EnemyStates
{
    Searching,
    Pursuing,
    Attacking
}