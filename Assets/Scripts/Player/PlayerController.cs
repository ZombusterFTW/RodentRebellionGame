using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private PlayerInput playerInput;
    [SerializeField] private float playerSpeed = 500f;
    [SerializeField] private float jumpForce = 15f;
    [SerializeField] private float wallSlideSpeed = 3f;
    [SerializeField] private float groundPoundVel = 20f;
    [SerializeField] private int playerMaxJumpCount = 1;
    private int playerDoubleJumpsRemaining;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask movingPlatformLayer;
    





    //Do box trace down to figure out if the player is grounded.
    [SerializeField] private bool onGround = true;
    [SerializeField] private bool isJumping = false;
    [SerializeField] private bool canJump = true;
    [SerializeField] private bool onWall = false;
    [SerializeField] private bool canMove = true;
    [SerializeField] private float lerpTime = 0.5f;
    [SerializeField] private float wallJumpTime = 0.15f;
    [SerializeField] private bool wallJumped = false;
    [SerializeField] private bool onLeftWall = false;
    [SerializeField] private bool onRightWall = false;
    [SerializeField] private bool isGroundPounding = false;
    [SerializeField] private bool canGroundPound = true;



    private Vector2 movementDirection;
    private Rigidbody2D playerRigidBody;
    private BoxCollider2D playerCollider;
    private SpriteRenderer spriteRenderer;
    public float collisionRadius = 0.25f;
    public Vector2 bottomOffset, rightOffset, leftOffset;
    private Color debugColor = Color.red;


    
    // Start is called before the first frame update
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        playerRigidBody = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerDoubleJumpsRemaining = playerMaxJumpCount;
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
        CheckGrounding();
        //IsTouchingWall();
    }



    void HandleInput()
    {
        //https://www.youtube.com/watch?v=STyY26a_dPY
        //Make player have the ability to "claw" into a wall and fall slowly. By doing this they can then jump which will cause them to do the up wall jump.
        if (canMove && !wallJumped) playerRigidBody.velocity = new Vector2(movementDirection.x * playerSpeed, playerRigidBody.velocity.y);
        else if(canMove && wallJumped && !onWall) playerRigidBody.velocity = Vector2.Lerp(playerRigidBody.velocity, (new Vector2(movementDirection.x * playerSpeed, playerRigidBody.velocity.y)), lerpTime * Time.deltaTime);

        if (onWall && !onGround && canMove && !isGroundPounding)
        {
            //Wall slide
            playerRigidBody.velocity = new Vector2(playerRigidBody.velocity.x, -wallSlideSpeed);
        }

        
        
    }

    void CheckGrounding()
    {
        onGround = Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset, collisionRadius, groundLayer);
        onWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, collisionRadius, groundLayer) || Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, collisionRadius, groundLayer);
        onRightWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, collisionRadius, groundLayer);
        onLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, collisionRadius, groundLayer);
        if (onGround)
        {
            GroundPound();
            wallJumped = false;
            ResetJumpCounter();
        }
       
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = debugColor;

        var positions = new Vector2[] {bottomOffset, rightOffset, leftOffset};

        Gizmos.DrawSphere((Vector2)transform.position + bottomOffset, collisionRadius);
        Gizmos.DrawSphere((Vector2)transform.position + rightOffset, collisionRadius);
        Gizmos.DrawSphere((Vector2)transform.position + leftOffset, collisionRadius);
    }


    private void Jump()
    {
        if(canJump)
        {
            if (onGround)
            {
                playerRigidBody.velocity = new Vector2(playerRigidBody.velocity.x, 0);
                playerRigidBody.velocity += Vector2.up * jumpForce;
                Debug.Log("Normal jump");
            }
            else if (!onGround && !onWall && playerDoubleJumpsRemaining > 0)
            {
                playerRigidBody.velocity = new Vector2(playerRigidBody.velocity.x, 0);
                playerRigidBody.velocity += Vector2.up * jumpForce;
                playerDoubleJumpsRemaining--;
                Debug.Log("Dbl jump");
            }
            else if (!onGround && onWall) WallJump();
        }
    }

    private void WallJump()
    {
        wallJumped = true;
        StopCoroutine(DisableMovement(0));
        StartCoroutine(DisableMovement(wallJumpTime));
        //check direction
        Vector2 wallDir = onRightWall ? Vector2.left : Vector2.right;
        playerRigidBody.velocity = new Vector2(playerRigidBody.velocity.x, 0);
        playerRigidBody.velocity += (wallDir/1.5f + Vector2.up) * jumpForce;
    }

    private void GroundPound()
    {
        if(!onGround && canGroundPound && !isGroundPounding)
        {
            isGroundPounding = true;
            canJump = false;
            playerRigidBody.velocity = new Vector2(playerRigidBody.velocity.x, -groundPoundVel);
        }
        else if(onGround && isGroundPounding)
        {
            Debug.Log("Hit ground after ground pound");
            isGroundPounding = false;
            canJump = true;
        }
    }



    IEnumerator DisableMovement(float timeToDisable)
    {
        canMove = false;
        yield return new WaitForSeconds(timeToDisable);
        canMove = true;
    }



    void ResetJumpCounter()
    {
        //Reset the player jump variable to its default.
        playerDoubleJumpsRemaining = playerMaxJumpCount;
    }


    public void OnMovement(InputAction.CallbackContext context)
    {
        movementDirection = context.ReadValue<Vector2>();
        movementDirection.Normalize();
        Debug.Log(movementDirection);
    }
    public void OnJump(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Performed:
                {
                    Debug.Log("Jump button pressed");
                    Jump();
                    break;
                }
            case InputActionPhase.Started:
                break;
            case InputActionPhase.Canceled:
                break;
        }
    }

    public void OnGroundPound(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Performed:
                Debug.Log("Ground pound button pressed");
                GroundPound();
                break;
            case InputActionPhase.Started:
                break;
            case InputActionPhase.Canceled:
                break;
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Performed:
                Debug.Log("Interact button pressed");
                break;
            case InputActionPhase.Started:
                break;
            case InputActionPhase.Canceled:
                break;
        }
    }
    public void OnRoll(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Performed:
                Debug.Log("Roll button pressed");
                break;
            case InputActionPhase.Started:
                break;
            case InputActionPhase.Canceled:
                break;
        }
    }




    private void OnEnable()
    {
        playerInput.ActivateInput();
    }

    private void OnDisable()
    {
        playerInput.DeactivateInput();
    }
}

