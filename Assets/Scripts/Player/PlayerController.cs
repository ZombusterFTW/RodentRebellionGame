using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, R4MovementComponent, MovingPlatformAnchor
{
    private PlayerInput playerInput;
    [SerializeField] private float playerSpeed = 500f;
    [SerializeField] private float jumpForce = 15f;
    [SerializeField] private float wallSlideSpeed = 3f;
    [SerializeField] private float groundPoundVel = 20f;
    [SerializeField] private float airControlLerpTime = 0.75f;
    [SerializeField] private int playerMaxJumpCount = 1;

    private float playerSpeed_Game;
    private float jumpForce_Game;
    private int playerDoubleJumpsRemaining;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private GameObject movingPlatform;
    [SerializeField] private SpawnPoint currentSpawn;


    //Make laser gun work with mouse targeting
    //Laser rifle beam like EM1 from Advanced Warfare.



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


    private bool isAlive = true;
    private Vector2 movementDirection;
    private Rigidbody2D playerRigidBody;
    private BoxCollider2D playerCollider;
    private SpriteRenderer spriteRenderer;
    public float collisionRadius = 0.25f;
    public Vector2 bottomOffset, rightOffset, leftOffset;
    private Color debugColor = Color.red;
    [SerializeField] private Health playerHealth;
    [SerializeField] private GameObject playerUIPrefab;
    public PlayerUI playerUI;


    private void Awake()
    {
        playerUI  = Instantiate(playerUIPrefab).GetComponent<PlayerUI>();
        playerInput = GetComponent<PlayerInput>();
        playerRigidBody = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerHealth = GetComponent<Health>();
    }
    // Start is called before the first frame update
    void Start()
    {
        playerDoubleJumpsRemaining = playerMaxJumpCount;
        playerSpeed_Game = playerSpeed;
        jumpForce_Game = jumpForce;
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
        CheckGrounding();
        //IsTouchingWall();
    }

    public PlayerUI GetPlayerUI()
    {
        return playerUI;    
    }


    void HandleInput()
    {
        //https://www.youtube.com/watch?v=STyY26a_dPY
        //Make player have the ability to "claw" into a wall and fall slowly. By doing this they can then jump which will cause them to do the up wall jump.
        //x velocity should be unchanged unless the player is pressing a key

        if (canMove && !wallJumped && onGround) playerRigidBody.velocity = new Vector2(movementDirection.x * playerSpeed_Game, playerRigidBody.velocity.y);
        else if (canMove && !wallJumped && onGround && movingPlatform != null && movementDirection.x == 0) playerRigidBody.velocity = playerRigidBody.velocity;
        else if(canMove && wallJumped && !onWall) playerRigidBody.velocity = Vector2.Lerp(playerRigidBody.velocity, (new Vector2(movementDirection.x * playerSpeed_Game, playerRigidBody.velocity.y)), lerpTime * Time.deltaTime);
        else if (canMove && !wallJumped && !onGround && movementDirection.x != 0) playerRigidBody.velocity = Vector2.Lerp((new Vector2(movementDirection.x * playerSpeed_Game, playerRigidBody.velocity.y)), playerRigidBody.velocity, airControlLerpTime * Time.deltaTime);
        else if (canMove && !wallJumped && !onGround && movementDirection.x == 0) playerRigidBody.velocity = Vector2.Lerp((new Vector2(playerRigidBody.velocity.x, playerRigidBody.velocity.y)), playerRigidBody.velocity, airControlLerpTime * Time.deltaTime);

        if (onWall && !onGround && canMove && !isGroundPounding)
        {
            float wallDir = onRightWall ? -2 : 2;

            //Wall slide
            wallDir = Mathf.Lerp(playerRigidBody.velocity.x, wallDir, lerpTime * Time.deltaTime);
            playerRigidBody.velocity = new Vector2(wallDir, -wallSlideSpeed);
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
                playerRigidBody.velocity += Vector2.up * jumpForce_Game;
                Debug.Log("Normal jump");
            }
            else if (!onGround && !onWall && playerDoubleJumpsRemaining > 0)
            {
                playerRigidBody.velocity = new Vector2(playerRigidBody.velocity.x, 0);
                playerRigidBody.velocity += Vector2.up * jumpForce_Game;
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
        playerRigidBody.velocity = new Vector2(0, 0);
        playerRigidBody.velocity += (wallDir/1.5f + Vector2.up) * jumpForce_Game;
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

    public BoxCollider2D GetPlayerCollider()
    {
        return playerCollider;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<SpawnPoint>() != null && collision.GetComponent<SpawnPoint>() != currentSpawn && collision.GetComponent<SpawnPoint>().GetIsActive())
        {
            SpawnPoint spawnPoint = collision.GetComponent<SpawnPoint>();
            SetSpawn(spawnPoint);
        }
        //Trigger the trap if it exists.
        if (collision.GetComponent<R4ActivatableTrap>() != null) collision.GetComponent<R4ActivatableTrap>().TriggerTrap();
    }
    

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.GetComponent<R4ActivatableTrap>() != null && isAlive)
        {
            collision.GetComponent<R4ActivatableTrap>().DealPlayerDamage(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //Reset movement speed
        if (collision.GetComponent<R4ActivatableTrap>() != null)
        {
            Debug.Log("Left trigger");
            collision.GetComponent<R4ActivatableTrap>().DealPlayerDamage(false);
        }
    }

    public void SetSpawn(SpawnPoint spawn)
    {
        Debug.Log("Set spawn");

        if(currentSpawn != null) currentSpawn.UnlinkSpawn();
        currentSpawn = spawn;
        currentSpawn.LinkSpawn();
    }

    public SpawnPoint GetSpawn()
    {
        return currentSpawn;
    }


    public bool GetIsGroundPounding()
    {
        return isGroundPounding;
    }

    private void OnEnable()
    {
        playerInput.ActivateInput();
    }

    private void OnDisable()
    {
        playerInput.DeactivateInput();
    }


    public void RespawnPlayer()
    {
        //If the player dies we respawn them. 
        isAlive = false;
        StopCoroutine(DisableMovement(0));
        StartCoroutine(DisableMovement(1f));
        playerRigidBody.velocity = Vector3.zero;
        gameObject.transform.position = currentSpawn.transform.position;
        playerHealth.HealthToMax();
        isAlive = true;
    }

    public Health GetHealthComponent()
    {
        return playerHealth;
    }


    public void RadiationCanisterPickup(UpgradeType upgradeType)
    {
        //Handle radiation canister upgrades/pickups.
        switch (upgradeType) 
        {
            case UpgradeType.Health_Upgrade:
                playerHealth.IncreaseHealthCap(0.15f);
                break;
            case UpgradeType.Attack_Upgrade: break; 
            case UpgradeType.GroundPound_Ability: break;
            case UpgradeType.WallClimb_Ability: break;
            case UpgradeType.DoubleJump_Ability: break; 
        }
    }

    public float GetActiveMovementSpeed()
    {
        return playerSpeed_Game;
    }
    public float GetMovementSpeed()
    {
        return playerSpeed;
    }
    public void SetMovementSpeed(float setSpeed)
    {
        Debug.Log("Set player speed");
        playerSpeed_Game = setSpeed;

        if (playerSpeed_Game < playerSpeed) canJump = false;
        else canJump = true;    
    }

    public void SetMovingPlatform(GameObject platform)
    {
       movingPlatform = platform;
       gameObject.transform.parent = movingPlatform.transform;
    }

    public void UnlinkPlatform(GameObject platform)
    {
        movingPlatform = null;
        gameObject.transform.parent = null;
    }



    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.GetComponentInParent<MovingPlatform>() != null)
        {
            SetMovingPlatform(collision.gameObject);
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponentInParent<MovingPlatform>() != null)
        {
            UnlinkPlatform(collision.gameObject);
        }
    }
}

public interface R4MovementComponent
{
    public float GetMovementSpeed();
    public float GetActiveMovementSpeed();
    public void SetMovementSpeed(float setSpeed);
}

public interface MovingPlatformAnchor
{
    public void SetMovingPlatform(GameObject platform);
    public void UnlinkPlatform(GameObject platform);
}