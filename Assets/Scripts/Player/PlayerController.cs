using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, R4MovementComponent, MovingPlatformAnchor, ControlledCharacter
{
    private PlayerInput playerInput;
    [SerializeField] private GameObject playerSpriteContainer;
    public DashTrail dashTrail;
    public DashTrailObject dashTrailObject;
    private SpriteRenderer playerSprite;
    private Animator playerAnimator;
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
    [SerializeField] private LayerMask playerWalls;
    [SerializeField] private LayerMask playerGround;
    [SerializeField] private PlayerWeaponType playerWeaponType = PlayerWeaponType.None;
    [SerializeField] private GameObject frenzyIdentifierText;
    //Make laser gun work with mouse targeting
    //Laser rifle beam like EM1 from Advanced Warfare.



    //Do box trace down to figure out if the player is grounded.
    [SerializeField] private bool onGround = true;
    [SerializeField] private bool isFalling = false;
    [SerializeField] private bool isJumping = false;
    [SerializeField] private bool canJump = true;
    [SerializeField] private bool onWall = false;
    [SerializeField] private bool canMove = true;
    [SerializeField] private float lerpTime = 0.5f;
    [SerializeField] private float wallJumpTime = 0.15f;
    [SerializeField] private float dashTimer = 0.25f;
    [SerializeField] private float dashThrust = 75f;
    [SerializeField] private int maxDashes = 2;
    private int remainingDashes = 0;
    [SerializeField] private bool wallJumped = false;
    [SerializeField] private bool onLeftWall = false;
    [SerializeField] private bool onRightWall = false;
    [SerializeField] private bool isGroundPounding = false;
    [SerializeField] private bool canGroundPound = true;
    [SerializeField] private bool isDashing = false;
    [SerializeField] private bool canDash = true;
    private bool isMoving = false;
    private bool isFiringLaser = false;
    private bool isAttacking = false;   

    private bool isAlive = true;
    private Vector2 movementDirection;
    private Vector2 lastDirection = Vector2.right;
    private Rigidbody2D playerRigidBody;
    private BoxCollider2D playerCollider;
    private SpriteRenderer spriteRenderer;
    public float collisionRadius = 0.25f;
    public Vector2 bottomOffset, rightOffset, leftOffset;
    private Color debugColor = Color.red;
    [SerializeField] private Health playerHealth;
    [SerializeField] private Upgrade playerUpgrade;
    [SerializeField] private GameObject playerUIPrefab;
    public PlayerUI playerUI;
    [SerializeField] private LineRenderer laserBeam;
    private Camera cam;
    private Quaternion rotation;
    private bool iFramesActive = false;
    private Coroutine hurtCoroutine;
    [SerializeField] private bool disableAllMoves = false;
    private bool jumpButtonPressed;
    private bool interactPressed;
    public bool frenzyActivated { get; private set; } = false;
    private int frenzyCounter;
    private int frenzyCount = 3;
    private Coroutine frenzyCoroutine;
    public LayerMask enemyLayer;
    private void Awake()
    {
        playerSprite = playerSpriteContainer.GetComponent<SpriteRenderer>();
        playerAnimator = playerSpriteContainer.GetComponent<Animator>();
        playerUI  = Instantiate(playerUIPrefab).GetComponent<PlayerUI>();
        playerInput = GetComponent<PlayerInput>();
        playerRigidBody = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerHealth = GetComponent<Health>();
        playerUpgrade = GetComponent<Upgrade>();
        remainingDashes = maxDashes;
        cam = Camera.main;
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
        //IsTouchingWall();
    }

    void FixedUpdate()
    {
        CheckGrounding();
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

        isMoving = (playerRigidBody.velocity.x != 0 && movementDirection.x != 0);
        isFalling = (playerRigidBody.velocity.y < 0  && !onGround && !onWall && movingPlatform == null);

        playerAnimator.SetBool("IsFalling", isFalling);
        playerAnimator.SetBool("MovingOnGround", isMoving);
        playerAnimator.SetBool("OnGround", onGround);

        if (isFalling) isJumping = false;
        if(!disableAllMoves)
        {
            if (canMove && !wallJumped && onGround) playerRigidBody.velocity = new Vector2(movementDirection.x * playerSpeed_Game, playerRigidBody.velocity.y);
            else if (canMove && !wallJumped && onGround && movingPlatform != null && movementDirection.x == 0) playerRigidBody.velocity = playerRigidBody.velocity;
            else if (canMove && wallJumped && !onWall) playerRigidBody.velocity = Vector2.Lerp(playerRigidBody.velocity, (new Vector2(movementDirection.x * playerSpeed_Game, playerRigidBody.velocity.y)), lerpTime * Time.deltaTime);
            else if (canMove && !wallJumped && !onGround && movementDirection.x != 0) playerRigidBody.velocity = Vector2.Lerp((new Vector2(movementDirection.x * playerSpeed_Game, playerRigidBody.velocity.y)), playerRigidBody.velocity, airControlLerpTime * Time.deltaTime);
            else if (canMove && !wallJumped && !onGround && movementDirection.x == 0) playerRigidBody.velocity = Vector2.Lerp((new Vector2(playerRigidBody.velocity.x, playerRigidBody.velocity.y)), playerRigidBody.velocity, airControlLerpTime * Time.deltaTime);

            if (onWall && !onGround && canMove && !isGroundPounding)
            {
                float wallDir = onRightWall ? 2 : -2;
                if (wallDir < 0)
                {
                    playerSprite.flipX = true;
                    dashTrailObject.GetComponent<SpriteRenderer>().flipX = false;
                }
                else
                {
                    playerSprite.flipX = false;
                    dashTrailObject.GetComponent<SpriteRenderer>().flipX = false;
                }
                Debug.Log("Wall sliding");
                //Wall slide
                wallDir = Mathf.Lerp(playerRigidBody.velocity.x, wallDir, lerpTime * Time.deltaTime);
                playerRigidBody.velocity = new Vector2(wallDir, -wallSlideSpeed);
            }
        }
        
        /*
        if (movementDirection.x != 0 && !onWall && onGround && isMoving && !isJumping)
        {
            if(!iFramesActive)playerAnimator.Play("BigJoeRun", 0);
        }
        else if((movementDirection == Vector2.zero && !onWall && onGround && !isMoving) && ( playerRigidBody.velocity == Vector2.zero || movingPlatform != null))
        {
            if (!iFramesActive) playerAnimator.Play("BigJoeIdle", 0);
        }
        if(isFalling && !isGroundPounding && !isJumping) if (!iFramesActive) playerAnimator.Play("BigJoeFalling", 0);
        */

        if (movementDirection != Vector2.zero && !wallJumped)
        {
            playerSprite.flipX = movementDirection.x < 0 ? true : false;
            dashTrailObject.GetComponent<SpriteRenderer>().flipX = playerSprite.flipX;
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
            isJumping = false;
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
        if(canJump && !disableAllMoves)
        {
            if (onGround)
            {
                playerRigidBody.velocity = new Vector2(playerRigidBody.velocity.x, 0);
                playerRigidBody.velocity += Vector2.up * jumpForce_Game;
                Debug.Log("Normal jump");
                onGround = false; 
                isJumping = true;
                //playerAnimator.Play("BigJoeJump", 0);
                playerAnimator.SetTrigger("Jump");

            }
            else if (!onGround && !onWall && playerDoubleJumpsRemaining > 0)
            {
                playerRigidBody.velocity = new Vector2(playerRigidBody.velocity.x, 0);
                playerRigidBody.velocity += Vector2.up * jumpForce_Game;
                playerDoubleJumpsRemaining--;
                Debug.Log("Dbl jump");
                //playerAnimator.Play("BigJoeDJ", 0);
                playerAnimator.SetTrigger("DoubleJump");
                isJumping = true;
            }
            else if (!onGround && onWall && !disableAllMoves) WallJump();
        }
    }


    private void WallJump()
    {
        wallJumped = true;
        StopCoroutine(DisableMovement(0));
        StartCoroutine(DisableMovement(wallJumpTime));
        //check direction
        Vector2 wallDir = onRightWall ? Vector2.left : Vector2.right;
        if(wallDir == Vector2.left) playerSprite.flipX = true;
        else playerSprite.flipX = false;
        //playerAnimator.Play("BigJoeWallJump", 0);
        playerAnimator.SetTrigger("WallJump");
        playerRigidBody.velocity = new Vector2(0, 0);
        playerRigidBody.velocity += (wallDir/1.5f + Vector2.up) * jumpForce_Game;
    }

    private void GroundPound()
    {
        if(!onGround && canGroundPound && !isGroundPounding&& !disableAllMoves)
        {
            isGroundPounding = true;
            canJump = false;
            playerRigidBody.velocity = new Vector2(playerRigidBody.velocity.x, -groundPoundVel);
            //playerAnimator.Play("BigJoeGroundPound", 0);
            playerAnimator.SetTrigger("GroundPound");
        }
        else if(onGround && isGroundPounding)
        {
            cam.transform.DOComplete();
            cam.transform.DOShakePosition(.2f, .5f, 14, 90, false, true);
            Debug.Log("Hit ground after ground pound");
            //Shake camera here
            isGroundPounding = false;
            canJump = true;
            //playerAnimator.Play("BigJoeLand", 0);
            playerAnimator.SetTrigger("Land");
        }
    }


    IEnumerator ResetDashTimer()
    {
        cam.transform.DOComplete();
        cam.transform.DOShakePosition(.2f, .5f, 14, 90, false, true);
        dashTrail.SetEnabled(true);
        playerRigidBody.drag = 25; 
        canDash = false;
        frenzyCounter++;
        if(frenzyCoroutine == null) frenzyCoroutine = StartCoroutine(FrenzyTimer());
        StartCoroutine(DisableMovement(dashTimer));
        yield return new WaitForSeconds(dashTimer);
        dashTrail.SetEnabled(false);
        if(!frenzyActivated)
        {
            canDash = true;
        }
        isDashing = false;
        playerRigidBody.velocity = new Vector2(0, playerRigidBody.velocity.y);
        playerRigidBody.drag = 0f;
    }

    IEnumerator FrenzyTimer()
    {
        float frenzyTimer = 1;
        while(frenzyCounter < frenzyCount)
        {
            frenzyTimer -= Time.deltaTime;
            if (frenzyTimer <= 0) break;
            yield return null;
        }
        if (frenzyCounter < frenzyCount)
        {
            frenzyCoroutine = null;
            yield break;
        }
        //Spawn frenzy text here
        Instantiate(frenzyIdentifierText);
        canDash = false;
        frenzyActivated = true;
        yield return new WaitForSecondsRealtime(7.5f);
        canDash = true;
        frenzyActivated = false;
        frenzyCounter = 0;
        frenzyCoroutine = null;
    }

    IEnumerator DisableMovement(float timeToDisable)
    {
        canMove = false;
        yield return new WaitForSeconds(timeToDisable);
        if (movementDirection != Vector2.zero && !wallJumped) playerSprite.flipX = movementDirection.x < 0 ? true : false;
        canMove = true;
    }



    void ResetJumpCounter()
    {
        //Reset the player jump variable to its default.
        playerDoubleJumpsRemaining = playerMaxJumpCount;
        remainingDashes = maxDashes;
    }


    public void OnMovement(InputAction.CallbackContext context)
    {
        if (!disableAllMoves)
        {
            movementDirection = context.ReadValue<Vector2>();
            movementDirection.Normalize();
            if (movementDirection != Vector2.zero) playerSprite.flipX = movementDirection.x < 0 ? true : false;
            Debug.Log(movementDirection);

            if (movementDirection != Vector2.zero)
            {
                //lastDirection = movementDirection.x < 0 ? Vector2.left : Vector2.right;
                lastDirection = movementDirection;
            }
        }
        else movementDirection = Vector2.zero;

    }
        public void OnJump(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Performed:
                    {
                        jumpButtonPressed = true;
                        Debug.Log("Jump button pressed");
                        Jump();
                        break;
                    }
                case InputActionPhase.Started:
                    break;
                case InputActionPhase.Canceled:
                    jumpButtonPressed = false;
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
                interactPressed = true;
                break;
            case InputActionPhase.Started:
                break;
            case InputActionPhase.Canceled:
                interactPressed = false;
                break;
        }
    }

    public bool GetInteractPressed()
    { return interactPressed; }

    public bool GetSpacePressed()
    { return jumpButtonPressed; }

    public void DisableControls(bool disabled)
    {
        disableAllMoves = disabled;
    }

    public void OnRoll(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Performed:
                
                break;
            case InputActionPhase.Started:
                Debug.Log("Roll button pressed");
                if (!isDashing && remainingDashes > 0 && canDash && !disableAllMoves)
                {
                    if (!onGround) remainingDashes--;
                    isDashing = true;
                    playerRigidBody.AddForce(lastDirection * dashThrust, ForceMode2D.Impulse);
                    StartCoroutine(ResetDashTimer());
                    movingPlatform = null; 
                }
                break;
            case InputActionPhase.Canceled:
                break;
        }
    }




    IEnumerator UpdateLaserPos()
    {
        laserBeam.enabled = true;
        while (isFiringLaser)
        {
            Vector3 pos =  cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, cam.transform.position.z));
            laserBeam.SetPosition(0, (Vector2)transform.position);
            laserBeam.SetPosition(1, (Vector2)pos);




            Vector2 direction =  (Vector2)transform.position - (Vector2)pos;
            RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position, direction.normalized, Mathf.Infinity, enemyLayer);


            if(hit)
            {
                laserBeam.SetPosition(1, (Vector2)hit.point);
                Debug.Log("Hit");

                if (hit.collider.gameObject.GetComponent<EnemyScript>() != null)
                {
                    hit.collider.gameObject.GetComponent<EnemyScript>().GetHealth().SubtractFromHealth(playerUpgrade.GetAttackDamage(PlayerAttackType.LaserBlast));
                }



                //check if enemy here.
            }

            yield return null;  
        }
        laserBeam.enabled = false;
    }


    public void OnLazerFire(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Performed:
                break;
            case InputActionPhase.Started:
                if(!disableAllMoves)
                {
                    isFiringLaser = true;
                    StartCoroutine(UpdateLaserPos());
                    Debug.Log("pRESS");
                }
                break;
            case InputActionPhase.Canceled:
                isFiringLaser = false;
                //StopCoroutine(UpdateLaserPos());

                break;
        }
    }

    public void OnLazerAltFire(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Performed:
                break;
            case InputActionPhase.Started:
                Jump();
                break;
            case InputActionPhase.Canceled:
                break;
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if(!disableAllMoves)
        {
            switch (context.phase)
            {
                case InputActionPhase.Performed:
                    break;
                case InputActionPhase.Started:
                    //Vector2 direction = ((lastDirection) - (Vector2)transform.position);
                    RaycastHit2D hit = Physics2D.Raycast(transform.position, lastDirection.normalized, 2f, enemyLayer);
                    playerAnimator.SetTrigger("Stab");
                    if (hit)
                    {
                        Debug.Log("Hit");
                        if (hit.collider.gameObject.GetComponent<EnemyScript>() != null)
                        {
                            hit.collider.gameObject.GetComponent<EnemyScript>().GetHealth().SubtractFromHealth(playerUpgrade.GetAttackDamage(PlayerAttackType.StandardAttack));
                        }
                    }
                    break;
                case InputActionPhase.Canceled:
                    break;
            }
        }
        
    }


    //For later
    void RotateToMouse()
    {
        Vector2 direction = cam.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        rotation.eulerAngles = new Vector3(0,0, angle);
        transform.rotation = rotation;
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

    IEnumerator IFrames()
    {
        iFramesActive = true;
        yield return new WaitForSeconds(1);
        iFramesActive = false;
    }

    IEnumerator PlayHurtAnim()
    {
        iFramesActive = true;
        while (iFramesActive)
        {
           // playerAnimator.Play("BigJoeHurt", 0);
            yield return new WaitForSeconds(0.5f);
        }
        
    }

    public void PlayHurt()
    {
        hurtCoroutine = StartCoroutine(PlayHurtAnim());
    }
    public void StopHurt()
    {
        iFramesActive = false;
        if (hurtCoroutine != null) StopCoroutine(hurtCoroutine);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //Reset movement speed
        if (collision.GetComponent<R4ActivatableTrap>() != null)
        {
            Debug.Log("Left trigger");
            collision.GetComponent<R4ActivatableTrap>().DealPlayerDamage(false);
        }
        if (collision.gameObject.GetComponentInParent<MovingPlatform>() != null)
        {
            UnlinkPlatform(collision.gameObject);
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
        Debug.Log("Coll");
        if (collision.gameObject.GetComponentInParent<MovingPlatform>() != null)
        {
            SetMovingPlatform(collision.gameObject);
        }


        if((isDashing || isGroundPounding || isAttacking) && (collision.gameObject.GetComponent<EnemyScript>() != null))
        {
            Debug.Log("Coll2");
            if (isDashing)
            {
                collision.gameObject.GetComponent<EnemyScript>().GetHealth().SubtractFromHealth(playerUpgrade.GetAttackDamage(PlayerAttackType.DaggerStrike));
            }
            else if(isGroundPounding)
            {
                collision.gameObject.GetComponent<EnemyScript>().GetHealth().SubtractFromHealth(playerUpgrade.GetAttackDamage(PlayerAttackType.GroundPound));
            }
            else if(isAttacking)
            {
                collision.gameObject.GetComponent<EnemyScript>().GetHealth().SubtractFromHealth(playerUpgrade.GetAttackDamage(PlayerAttackType.StandardAttack));
            }
        }

    }

    public void PlayDamagedAnim()
    {
        playerAnimator.Play("BigJoeHurt", 0);
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

public interface ControlledCharacter
{
    public PlayerUI GetPlayerUI();
    public void RespawnPlayer();
    public void PlayDamagedAnim();
}

public enum PlayerWeaponType
{
    None,
    Dagger,
    LaserGun
}