using Cinemachine;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour, R4MovementComponent, MovingPlatformAnchor, ControlledCharacter
{
    [SerializeField] private PauseMenu pauseMenu;
    SceneTransitionerManager sceneTransitionerManager;
    private PlayerInput playerInput;
    [SerializeField] private GameObject playerSpriteContainer;
    [SerializeField] private GameObject playerSpriteContainerRubberMode;
    public DashTrail dashTrailRubber;
    public DashTrailObject dashTrailObjectRubber;

    public DashTrail dashTrail;
    public DashTrailObject dashTrailObject;
    private SpriteRenderer playerSprite;
    private Animator playerAnimator;
    private SpriteRenderer playerSpriteRubber;
    private Animator playerAnimatorRubber;
    [SerializeField] private float playerSpeed = 500f;
    [SerializeField] private float jumpForce = 15f;
    [SerializeField] private float wallSlideSpeed = 3f;
    [SerializeField] private float groundPoundVel = 20f;
    [SerializeField] private float airControlLerpTime = 0.75f;
    [SerializeField] private int playerMaxJumpCount = 1;
    public GameObject laserStartPosRight;
    public GameObject laserStartPosLeft;

    private float playerSpeed_Game;
    private float jumpForce_Game;
    private int playerDoubleJumpsRemaining;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] public GameObject movingPlatform;
    [SerializeField] private SpawnPoint currentSpawn;
    [SerializeField] private LayerMask playerWalls;
    [SerializeField] private LayerMask playerGround;
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
    public bool isFlipped { get;  private set; } = false;
    [SerializeField] private float lerpTime = 0.5f;
    [SerializeField] private float wallJumpTime = 0.15f;
    [SerializeField] private float dashTimer = 0.25f;
    [SerializeField] private float dashThrust = 75f;
    [SerializeField] private int maxDashes = 2;
    private int remainingDashes = 0;
    [SerializeField] private bool wallJumped = false;
    [SerializeField] private bool onLeftWall = false;
    [SerializeField] private bool onRightWall = false;
    public bool isGroundPounding { get; private set; } = false;
    
    public bool isDashing { get; private set; } = false;
    
    private bool isMoving = false;
    private bool isFiringLaser = false;
    private bool isAttacking = false;   

    private bool isAlive = true;
    private Vector2 movementDirection;
    private Vector2 lastDirection = Vector2.right;
    private Rigidbody2D playerRigidBody;
    private BoxCollider2D playerCollider;
    //private SpriteRenderer spriteRenderer;
    public float collisionRadius = 0.25f;
    public float collisionRadiusTopBottom = 0.25f;

    public Vector2 bottomOffset, rightOffset, leftOffset, topOffset;
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
    [SerializeField] public bool disableAllMoves { get; private set; } = false;
    private bool jumpButtonPressed;
    private bool interactPressed;
    public bool frenzyActivated { get; private set; } = false;
    //Note activatable switches are considered enemies.
    public LayerMask enemyLayer;
    public ParticleSystem frenzyLines;
    private FrenzyManager frenzyManager;



    //Ability Unlocks
    [SerializeField] private bool canWallClimb = true;
    [SerializeField] private bool canDash = true;
    [SerializeField] private bool canGroundPound = true;
    [SerializeField] private bool canDoubleJump = true;
    [SerializeField] private bool canWallJump = true;
    [SerializeField] private bool canEnterRageMode = true;
    [SerializeField] private bool canPhaseShift = true;

    //Audio Manager Class
    [SerializeField] private CharacterSoundManager characterSoundManager;
    private Coroutine hurtSound;
    private Coroutine respawnJoe;
    private Coroutine latentAttack;
    public bool playerIsAttacking { get; private set; } = false;
    private PlayerWeaponType playerWeapon;
    private bool attackCooldownActive = false;
    [SerializeField] private EventSystem playerEventSystem;
    public static PlayerController instance;
    private float groundPoundDelay = 0.25f;
    private bool groundPoundDelayActive = false;    
    /// <summary>
    /// Mode switch notes. To enter rubber mode the player must have a portion of their rage meter. In rubber mode the bar slowly drains overtime with the player being kicked out of the mode alltogether if it runs out.
    /// Rubber mode has the increased emphasis on movement with more opportunites being available to the player. 
    /// The player can also enter rage mode with this bar instead staying in rat mode.
    /// Question that needs to be answered is, should it be possible to enter rage mode while in rubber mode?
    /// Moving platforms move in rubber mode when they are powered. Will add a new bool to the activatable item interface that dictates how they should function in each mode. For now a circle transition will suffice with a more detailed transition worked on later.
    /// Specific moves that are disabled in rubber/rat mode can be disabled by checking if the inRubberMode bool from the frenzy manager is true or false.
    /// </summary>





    private void Awake()
    {
        if (instance == null)
        {
            Debug.Log("Creating new player instance");
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.Log("Found more than one player in the scene. Deleting the copy");
            DestroyImmediate(gameObject);
            return;
        }
        playerSprite = playerSpriteContainer.GetComponent<SpriteRenderer>();
        playerAnimator = playerSpriteContainer.GetComponent<Animator>();
        playerSpriteRubber = playerSpriteContainerRubberMode.GetComponent<SpriteRenderer>();
        playerAnimatorRubber = playerSpriteContainerRubberMode.GetComponent<Animator>();
        playerUI  = Instantiate(playerUIPrefab).GetComponent<PlayerUI>();
        playerUI.transform.SetParent(this.transform, false);
        playerInput = GetComponent<PlayerInput>();
        playerRigidBody = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<BoxCollider2D>();
        //spriteRenderer = GetComponent<SpriteRenderer>();
        playerHealth = GetComponent<Health>();
        playerUpgrade = GetComponent<Upgrade>();
        remainingDashes = maxDashes;
        frenzyManager = GetComponent<FrenzyManager>();  
        cam = Camera.main;
    }
    // Start is called before the first frame update
    void Start()
    {
        playerDoubleJumpsRemaining = playerMaxJumpCount;
        playerSpeed_Game = playerSpeed;
        jumpForce_Game = jumpForce;
        SceneManager.sceneLoaded += OnSceneChange;
        //sceneTransitionerManager = SceneTransitionerManager.instance;
        playerWeapon = playerUpgrade.playerWeaponType;
    }

    private void OnSceneChange(Scene arg0, LoadSceneMode arg1)
    {
        if(this != null)
        {
            if (SceneManager.GetActiveScene().buildIndex == 0)
            {
                StopAllCoroutines();
                DestroyImmediate(gameObject);
            }
        }
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

    public void ToggleGravityFlip()
    {
        playerSprite.flipY = !isFlipped;
        playerSpriteRubber.flipY = !isFlipped;
        if (!isFlipped)
        {
            dashTrailObject.GetComponent<SpriteRenderer>().flipY = false;
            dashTrailObjectRubber.GetComponent<SpriteRenderer>().flipY = false;
            isFlipped = true;
            playerRigidBody.gravityScale = -2;
            
        }
        else
        {
            isFlipped = false;
            playerRigidBody.gravityScale = 2;
            dashTrailObject.GetComponent<SpriteRenderer>().flipY = true;
            dashTrailObjectRubber.GetComponent<SpriteRenderer>().flipY = true;
        }
    }


    void HandleInput()
    {
        //https://www.youtube.com/watch?v=STyY26a_dPY
        //Make player have the ability to "claw" into a wall and fall slowly. By doing this they can then jump which will cause them to do the up wall jump.
        //x velocity should be unchanged unless the player is pressing a key

        isMoving = (playerRigidBody.velocity.x != 0 && movementDirection.x != 0);
        isFalling = ((playerRigidBody.velocity.y < 0 || playerRigidBody.velocity.y > 0 && isFlipped)  && !onGround && !onWall && movingPlatform == null);

        playerAnimator.SetBool("IsFalling", isFalling);
        playerAnimator.SetBool("MovingOnGround", isMoving);
        playerAnimator.SetBool("OnGround", onGround);
        playerAnimatorRubber.SetBool("IsFalling", isFalling);
        playerAnimatorRubber.SetBool("MovingOnGround", isMoving);
        playerAnimatorRubber.SetBool("OnGround", onGround);

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
                    playerSpriteRubber.flipX = true;
                    dashTrailObject.GetComponent<SpriteRenderer>().flipX = false;
                    dashTrailObjectRubber.GetComponent<SpriteRenderer>().flipX = false;
                }
                else
                {
                    playerSprite.flipX = false;
                    playerSpriteRubber.flipX = false;
                    dashTrailObject.GetComponent<SpriteRenderer>().flipX = false;
                    dashTrailObjectRubber.GetComponent<SpriteRenderer>().flipX = false;
                }
                Debug.Log("Wall sliding");
                playerAnimator.SetBool("OnWall", true);
                playerAnimatorRubber.SetBool("OnWall", true);
                //Prevent the bump but allow the player to exit a wall slide without wall jumping.
                if(movementDirection.x == -1 || movementDirection.x == 1)
                {
                    if(movementDirection.x < 0 && onRightWall)
                    {
                        playerRigidBody.velocity = new Vector2(-1, playerRigidBody.velocity.y);
                        onWall = false;
                    }
                    else if(movementDirection.x > 0 && !onRightWall)
                    {
                        playerRigidBody.velocity = new Vector2(1, playerRigidBody.velocity.y);
                        onWall = false;
                    }
                    else playerRigidBody.velocity = new Vector2(0, playerRigidBody.velocity.y);
                }
                else playerRigidBody.velocity = new Vector2(0, playerRigidBody.velocity.y);

                //Wall slide
                wallDir = Mathf.Lerp(playerRigidBody.velocity.x, wallDir, lerpTime * Time.deltaTime);
                if (isFlipped == false) playerRigidBody.velocity = new Vector2(wallDir, -wallSlideSpeed);
                else playerRigidBody.velocity = new Vector2(wallDir, wallSlideSpeed);
            }
            else
            {
                playerAnimator.SetBool("OnWall", false);
                playerAnimatorRubber.SetBool("OnWall", false);
            }
        }
        else
        {
            //Stop movement during dialouge
            playerRigidBody.velocity = Vector2.zero;
            movementDirection = Vector2.zero;
        }
       
        if (movementDirection != Vector2.zero && !wallJumped)
        {
            playerSprite.flipX = movementDirection.x < 0 ? true : false;
            playerSpriteRubber.flipX = movementDirection.x < 0 ? true : false;
            dashTrailObject.GetComponent<SpriteRenderer>().flipX = playerSprite.flipX;
            dashTrailObjectRubber.GetComponent<SpriteRenderer>().flipX = playerSpriteRubber.flipX;
        }

        //Activate sound when the player moves
        if (isMoving && onGround)
        {
            //Debug.Log("Moving");
            if(characterSoundManager.movementPlaying == false) characterSoundManager.ActivateMovementLoopSound(true);
        }
        else
        {
            //Debug.Log("Not moving");
            if (characterSoundManager.movementPlaying == true) characterSoundManager.ActivateMovementLoopSound(false);
        }
    }

    void CheckGrounding()
    {
        if(isFlipped == false) onGround = Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset, collisionRadiusTopBottom, groundLayer);
        else onGround = Physics2D.OverlapCircle((Vector2)transform.position + topOffset, collisionRadiusTopBottom, groundLayer);
        //Prevent wall climb if player lacks the ability.
        if (canWallClimb)
        {
            onWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, collisionRadius, playerWalls) || Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, collisionRadius, playerWalls);
            onRightWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, collisionRadius, playerWalls);
            onLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, collisionRadius, playerWalls);
            if(onGround)
            {
                //Player cannot be on wall if they are grounded
                onWall = false; onRightWall = false; onLeftWall = false;
            }
        }
        else
        {
            onWall = false; onRightWall = false; onRightWall = false;
        }
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
        
        Gizmos.color = new Color(0, 1, 0, 0.5f);

        var positions = new Vector2[] {bottomOffset, rightOffset, leftOffset, topOffset};

        Gizmos.DrawSphere((Vector2)transform.position + bottomOffset, collisionRadiusTopBottom);
        Gizmos.DrawSphere((Vector2)transform.position + rightOffset, collisionRadius);
        Gizmos.DrawSphere((Vector2)transform.position + leftOffset, collisionRadius);
        Gizmos.DrawSphere((Vector2)transform.position + topOffset, collisionRadiusTopBottom);


        Gizmos.color = debugColor;
        Gizmos.DrawSphere((Vector2)transform.position - new Vector2(2f, 0), 0.5f);
        Gizmos.DrawSphere((Vector2)transform.position + new Vector2(2f, 0), 0.5f);


    }


    private void Jump()
    {
        if(canJump && !disableAllMoves)
        {
            if (onGround)
            {
                playerRigidBody.velocity = new Vector2(playerRigidBody.velocity.x, 0);
                if(isFlipped == false) playerRigidBody.velocity += Vector2.up * jumpForce_Game;
                else playerRigidBody.velocity += Vector2.down * jumpForce_Game;
                //Debug.Log("Normal jump");
                onGround = false; 
                isJumping = true;
                //playerAnimator.Play("BigJoeJump", 0);
                playerAnimator.SetTrigger("Jump");
                playerAnimatorRubber.SetTrigger("Jump");

            }
            else if (!onGround && !onWall && playerDoubleJumpsRemaining > 0 && canDoubleJump)
            {
                playerRigidBody.velocity = new Vector2(playerRigidBody.velocity.x, 0);
                if (isFlipped == false) playerRigidBody.velocity += Vector2.up * jumpForce_Game;
                else playerRigidBody.velocity += Vector2.down * jumpForce_Game;
                playerDoubleJumpsRemaining--;
                //Debug.Log("Dbl jump");
                //playerAnimator.Play("BigJoeDJ", 0);
                playerAnimator.SetTrigger("DoubleJump");
                playerAnimatorRubber.SetTrigger("DoubleJump");
                /*
                //Rare play of flip anim
                if (Random.Range(0, 5) == 1)
                {
                    playerAnimator.SetTrigger("Flip");
                    playerAnimatorRubber.SetTrigger("Flip");
                }
                else
                {
                    playerAnimator.SetTrigger("DoubleJump");
                    playerAnimatorRubber.SetTrigger("DoubleJump");
                }
                */
                isJumping = true;
            }
            else if (!onGround && onWall && !disableAllMoves && canWallJump) WallJump();
        }
    }


    private void WallJump()
    {
        wallJumped = true;
        StopCoroutine(DisableMovement(0));
        StartCoroutine(DisableMovement(wallJumpTime));
        //check direction
        Vector2 wallDir = onRightWall ? Vector2.left : Vector2.right;
        if (wallDir == Vector2.left)
        {
            playerSprite.flipX = true;
            playerSpriteRubber.flipX = true;
        }
        else
        {
            playerSprite.flipX = false;
            playerSpriteRubber.flipX = false;
        }
        //playerAnimator.Play("BigJoeWallJump", 0);
        playerAnimator.SetTrigger("WallJump");
        playerAnimatorRubber.SetTrigger("WallJump");
        playerRigidBody.velocity = new Vector2(0, 0);
        if(isFlipped == false) playerRigidBody.velocity += (wallDir/1.5f + Vector2.up) * jumpForce_Game;
        else playerRigidBody.velocity += (wallDir / 1.5f + Vector2.down) * jumpForce_Game;
    }

    private void GroundPound()
    {
        if(!onGround && canGroundPound && !isGroundPounding&& !disableAllMoves)
        {
            isGroundPounding = true;
            canJump = false;
            if(isFlipped == false) playerRigidBody.velocity = new Vector2(playerRigidBody.velocity.x, -groundPoundVel);
            else playerRigidBody.velocity = new Vector2(playerRigidBody.velocity.x, groundPoundVel);
            //playerAnimator.Play("BigJoeGroundPound", 0);
            playerAnimator.SetTrigger("GroundPound");
            playerAnimatorRubber.SetTrigger("GroundPound");
        }
        else if(onGround && isGroundPounding && !groundPoundDelayActive)
        {
            cam.transform.DOComplete();
            cam.transform.DOShakePosition(.2f, .5f, 14, 90, false, true);
            //Debug.Log("Hit ground after ground pound");
            //Ground pound delay
            StartCoroutine(GroundPoundDelay());
            //Shake camera here
            //isGroundPounding = false;
            //canJump = true;
            //playerAnimator.Play("BigJoeLand", 0);
            playerAnimatorRubber.SetTrigger("Land");
            playerAnimator.SetTrigger("Land");
            characterSoundManager.PlayAudioCallout(CharacterAudioCallout.Land);
        }
    }
    IEnumerator GroundPoundDelay()
    {
        //Added this to make button activation more consistent and prevent spam
        isGroundPounding = true;
        groundPoundDelayActive = true;
        yield return new WaitForSecondsRealtime(groundPoundDelay);
        isGroundPounding = false;
        groundPoundDelayActive = false;
        canJump = true;
    }

    IEnumerator ResetDashTimer()
    {
        cam.transform.DOComplete();
        cam.transform.DOShakePosition(.2f, .5f, 14, 90, false, true);
        dashTrail.SetEnabled(true);
        dashTrailRubber.SetEnabled(true);
        playerRigidBody.drag = 25; 
        canDash = false;
        DashWallHandler(true);
        StartCoroutine(DisableMovement(dashTimer));
        yield return new WaitForSeconds(dashTimer);
        dashTrail.SetEnabled(false);
        dashTrailRubber.SetEnabled(false);
        canDash = true;
        DashWallHandler(false);
        isDashing = false;
        playerRigidBody.velocity = new Vector2(0, playerRigidBody.velocity.y);
        playerRigidBody.drag = 0f;
    }

    private void DashWallHandler(bool wallPassable)
    {
        DashWall[] dashWalls = GameObject.FindObjectsOfType<DashWall>();
        //Make change to the dash wall script so audio from dash walls near the player will play.
        foreach (DashWall dashWall in dashWalls)
        {
            dashWall.TogglePassableState(wallPassable);
        }
    }



    public void ActivateFrenzy(bool activate)
    {
        if(activate)
        {
            //Spawn frenzy text here
            Instantiate(frenzyIdentifierText);
            frenzyLines.Play();
            canDash = false;
            frenzyActivated = true;
            dashTrail.SetEnabled(true);
            playerAnimator.SetBool("FrenzyActive", true);
            playerAnimator.SetTrigger("ActivateFrenzy");
            
        }
        else
        {
            frenzyLines.Stop();
            canDash = true;
            frenzyActivated = false;
            dashTrail.SetEnabled(false);
            playerAnimator.SetBool("FrenzyActive", false);
            playerAnimator.SetTrigger("DeactivateFrenzy");
            
        }
    }

 




    IEnumerator DisableMovement(float timeToDisable)
    {
        canMove = false;
        yield return new WaitForSeconds(timeToDisable);
        if (movementDirection != Vector2.zero && !wallJumped)
        {
            playerSprite.flipX = movementDirection.x < 0 ? true : false;
            playerSpriteRubber.flipX = movementDirection.x < 0 ? true : false;
        }
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
            if (movementDirection != Vector2.zero)
            {
                playerSprite.flipX = movementDirection.x < 0 ? true : false;
                playerSpriteRubber.flipX = movementDirection.x < 0 ? true : false;
            }
            //Debug.Log(movementDirection);

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
                        //Debug.Log("Jump button pressed");
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
    public void OnWeaponSwap(InputAction.CallbackContext context)
    {
        float scrollAmount = 0;
        switch (context.phase)
        {
            case InputActionPhase.Performed:

                if(!disableAllMoves && !playerIsAttacking)
                {
                    scrollAmount = context.ReadValue<float>();
                    if (scrollAmount > 0)
                    {
                        playerUpgrade.SwapWeapon(false);
                        Debug.Log("swaped down");
                    }
                    else if (scrollAmount < 0)
                    {
                        playerUpgrade.SwapWeapon();
                        Debug.Log("swaped up");
                    }
                    if (playerWeapon != playerUpgrade.playerWeaponType)
                    {
                        characterSoundManager.PlayAudioCallout(CharacterAudioCallout.Weapon1);
                        playerWeapon = playerUpgrade.playerWeaponType;
                    }
                }
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



    public void OnRubberModeToggle(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Performed:
                //Attempt to activate frenzy mode.
                if (!disableAllMoves && canPhaseShift) frenzyManager.ToggleRubberMode();
                break;
        }
    }

    public bool GetInteractPressed()
    { return interactPressed; }

    public bool GetSpacePressed()
    {

        //Change to use new input system later
        // return jumpButtonPressed; 
        return Input.GetKeyDown(KeyCode.Space);
    }

    public void DisableControls(bool disabled)
    {
        disableAllMoves = disabled;
    }

    public void OnRoll(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Performed:
                //Attempt to activate frenzy mode.
                if(!disableAllMoves && canEnterRageMode) frenzyManager.ActivateFrenzyMeter();
                break;
            case InputActionPhase.Started:
                break;
            case InputActionPhase.Canceled:
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
        }
    }




    IEnumerator UpdateLaserPos()
    {
        laserBeam.enabled = true;
        playerAnimator.SetTrigger("Laser");
        playerAnimatorRubber.SetTrigger("Laser");
        Vector3 pos =  cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 20));
        float laserDir = (transform.position - pos).normalized.x;
        Vector2 laserFireDirection;
        if (laserDir < 0)
        {
            playerSprite.flipX = false;
            playerSpriteRubber.flipX = false;
            laserFireDirection = (Vector2)laserStartPosRight.transform.position;
        }
        else
        {
            playerSprite.flipX = true;
            playerSpriteRubber.flipX = true;
            laserFireDirection = (Vector2)laserStartPosLeft.transform.position;
            
        }
            laserBeam.SetPosition(0, new Vector2(laserFireDirection.x, laserFireDirection.y));
            laserBeam.SetPosition(1, (Vector2)pos);
            Vector2 direction =  (Vector2)transform.position - (Vector2)pos;
            RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position, -direction.normalized, Mathf.Infinity, enemyLayer);
            if (hit)
            {
                laserBeam.SetPosition(1, hit.point);
                //Debug.Log("Hit");
                if (!GameObject.ReferenceEquals(hit.collider.gameObject.GetComponent<EnemyAI>(), null))
                {
                    hit.collider.gameObject.GetComponent<EnemyAI>().GetHealth().SubtractFromHealth(playerUpgrade.GetAttackDamage(PlayerAttackType.LaserBlast));
                }
                if (!GameObject.ReferenceEquals(hit.collider.gameObject.GetComponent<Switch>(), null))
                {
                    hit.collider.gameObject.GetComponent<Switch>().ToggleSwitch(PlayerAttackType.LaserBlast);
                }
                if (!GameObject.ReferenceEquals(hit.collider.gameObject.GetComponent<OneHitHealthEnemy>(), null))
                {
                    hit.collider.gameObject.GetComponent<OneHitHealthEnemy>().OnOneHitEnemyDeath();
                }
                if(hit.collider.gameObject.tag == "Shield")
                {
                    hit.collider.gameObject.GetComponentInParent<EnemyAI>().GetHealth().SubtractFromHealth(playerUpgrade.GetAttackDamage(PlayerAttackType.LaserBlast));
                }
            //check if enemy here.
            }
            else
            {
                laserBeam.SetPosition(1, hit.point);
            }
            yield return new WaitForSecondsRealtime(0.125f);
   
        isFiringLaser = false;
        laserBeam.enabled = false;
    }


    public void OnLazerFire(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Performed:
                break;
            case InputActionPhase.Started:
                if(!disableAllMoves && playerUpgrade.playerWeaponType == PlayerWeaponType.LaserGun)
                {
                    //play lazer sound
                    characterSoundManager.PlayAudioCallout(CharacterAudioCallout.Weapon2);
                    isFiringLaser = true;
                    StartCoroutine(UpdateLaserPos());
                    //Debug.Log("pRESS");
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
               // Jump();
                break;
            case InputActionPhase.Canceled:
                break;
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if(!disableAllMoves && !attackCooldownActive)
        {
            switch (context.phase)
            {
                case InputActionPhase.Performed:
                    break;
                case InputActionPhase.Started:
                    if(Coroutine.ReferenceEquals(latentAttack, null)) latentAttack = StartCoroutine(LatentAttack());
                    break;
                case InputActionPhase.Canceled:
                    break;
            }
        }
    }


    IEnumerator LatentAttack()
    {
        playerIsAttacking = true;
        //Vector2 direction = ((lastDirection) - (Vector2)transform.position);
        //Detect which weapon a player has to determine their damage
        if (playerUpgrade.playerWeaponType == PlayerWeaponType.Dagger)
        {
            //Seperate line casts for each weapon
            RaycastHit2D hit = Physics2D.Raycast(transform.position, lastDirection.normalized, 1.2f, enemyLayer);
            characterSoundManager.PlayAudioCallout(CharacterAudioCallout.Attack);
            characterSoundManager.PlayAudioCallout(CharacterAudioCallout.Weapon1);
            playerAnimator.SetTrigger("Stab");
            playerAnimatorRubber.SetTrigger("Stab");
            if (hit && hit.collider.tag != "Shield")
            {
                //Debug.Log("Hit");
                if (!GameObject.ReferenceEquals(hit.collider.gameObject.GetComponent<EnemyAI>(), null))
                {
                    hit.collider.gameObject.GetComponent<EnemyAI>().GetHealth().SubtractFromHealth(playerUpgrade.GetAttackDamage(PlayerAttackType.DaggerStrike));
                }
                if (!GameObject.ReferenceEquals(hit.collider.gameObject.GetComponent<Switch>(), null))
                {
                    hit.collider.gameObject.GetComponent<Switch>().ToggleSwitch(PlayerAttackType.DaggerStrike);
                }
                if (!GameObject.ReferenceEquals(hit.collider.gameObject.GetComponent<OneHitHealthEnemy>(), null))
                {
                    hit.collider.gameObject.GetComponent<OneHitHealthEnemy>().OnOneHitEnemyDeath();
                }
            }
        }
        else if (playerUpgrade.playerWeaponType == PlayerWeaponType.None)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, lastDirection.normalized, 1f, enemyLayer);
            characterSoundManager.PlayAudioCallout(CharacterAudioCallout.Attack);
            characterSoundManager.PlayAudioCallout(CharacterAudioCallout.NoWeapon);
            //replace me with standard attack
            playerAnimator.SetTrigger("StandardAttack");
            playerAnimatorRubber.SetTrigger("StandardAttack");
            if (hit && hit.collider.tag != "Shield")
            {
                //Debug.Log("Hit");
                if (!GameObject.ReferenceEquals(hit.collider.gameObject.GetComponent<EnemyAI>(), null))
                {
                    hit.collider.gameObject.GetComponent<EnemyAI>().GetHealth().SubtractFromHealth(playerUpgrade.GetAttackDamage(PlayerAttackType.StandardAttack));
                }
                if (!GameObject.ReferenceEquals(hit.collider.gameObject.GetComponent<Switch>(), null))
                {
                    hit.collider.gameObject.GetComponent<Switch>().ToggleSwitch(PlayerAttackType.StandardAttack);
                }
                if (!GameObject.ReferenceEquals(hit.collider.gameObject.GetComponent<OneHitHealthEnemy>(), null))
                {
                    hit.collider.gameObject.GetComponent<OneHitHealthEnemy>().OnOneHitEnemyDeath();
                }
            }
        }
        else if (playerUpgrade.playerWeaponType == PlayerWeaponType.ChainWhip)
        {
            float chainWhipDistance = 2.5f;
            float chainWhipSphereSize = 0.5f;
            float chainWhipDeployTime = 0.25f;
            Collider2D[] hitObjects;
            //Chain whip does no damage until it hits delayed sphere cast.
            yield return new WaitForSeconds(chainWhipDeployTime);
            bool isFacingLeft = lastDirection.x < 0 ? true : false;
            bool hitWallOnTheWay = false;   
            if (isFacingLeft)
            {
                hitObjects = Physics2D.OverlapCircleAll((Vector2)transform.position - new Vector2(chainWhipDistance, 0), chainWhipSphereSize, enemyLayer);
                hitWallOnTheWay = Physics2D.Linecast(transform.position, (Vector2)transform.position - new Vector2(chainWhipDistance/2, 0), groundLayer);
            }
            else
            {
                hitObjects = Physics2D.OverlapCircleAll((Vector2)transform.position + new Vector2(chainWhipDistance,0) , chainWhipSphereSize, enemyLayer);
                hitWallOnTheWay = Physics2D.Linecast(transform.position, (Vector2)transform.position + new Vector2(chainWhipDistance/2, 0), groundLayer);
            }
            // RaycastHit2D hit = Physics2D.LinecastAll(transform.position, lastDirection.normalized, 5f, enemyLayer);
            characterSoundManager.PlayAudioCallout(CharacterAudioCallout.Attack);
            characterSoundManager.PlayAudioCallout(CharacterAudioCallout.NoWeapon);
            //replace me with chain whip anim
            playerAnimator.SetTrigger("StandardAttack");
            playerAnimatorRubber.SetTrigger("StandardAttack");

            if(hitObjects.Length > 0 && !hitWallOnTheWay)
            {
                foreach(Collider2D hit in hitObjects)
                {
                    if(hit.gameObject.tag != "Shield")
                    {
                        //Debug.Log("Hit");
                        if (!GameObject.ReferenceEquals(hit.gameObject.GetComponent<EnemyAI>(), null))
                        {
                            hit.gameObject.GetComponent<EnemyAI>().GetHealth().SubtractFromHealth(playerUpgrade.GetAttackDamage(PlayerAttackType.ChainWhipAttack));
                        }
                        if (!GameObject.ReferenceEquals(hit.gameObject.GetComponent<Switch>(), null))
                        {
                            hit.gameObject.GetComponent<Switch>().ToggleSwitch(PlayerAttackType.ChainWhipAttack);
                        }
                        if (!GameObject.ReferenceEquals(hit.gameObject.GetComponent<OneHitHealthEnemy>(), null))
                        {
                            hit.gameObject.GetComponent<OneHitHealthEnemy>().OnOneHitEnemyDeath();
                        }
                    }
                }
            }
            else
            {
                Debug.Log("No enemies or chainwhip hit wall");
            }
        }
        //Prevent player from spamming attack
        attackCooldownActive = true;
        yield return new WaitForSeconds(0.15f);
        attackCooldownActive = false;
        playerIsAttacking = false;
        latentAttack = null;
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
        if (!GameObject.ReferenceEquals(collision.GetComponent<SpawnPoint>(), null) && collision.GetComponent<SpawnPoint>() != currentSpawn && collision.GetComponent<SpawnPoint>().GetIsActive())
        {
            SpawnPoint spawnPoint = collision.GetComponent<SpawnPoint>();
            SetSpawn(spawnPoint);
        }
        
        //Trigger the trap if it exists.
        if (!GameObject.ReferenceEquals(collision.GetComponent<R4ActivatableTrap>(), null)) collision.GetComponent<R4ActivatableTrap>().TriggerTrap();
    }
    

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(!GameObject.ReferenceEquals(collision.GetComponent<R4ActivatableTrap>(), null) && isAlive)
        {
                collision.GetComponent<R4ActivatableTrap>().DealPlayerDamage(true);                
        }
    }






    private void OnTriggerExit2D(Collider2D collision)
    {
        //Reset movement speed
        if (!GameObject.ReferenceEquals(collision.GetComponent<R4ActivatableTrap>(), null))
        {
            Debug.Log("Left trigger");
            collision.GetComponent<R4ActivatableTrap>().DealPlayerDamage(false);
        }
        //if (collision.gameObject.GetComponentInParent<MovingPlatform>() != null)
        //{
            //UnlinkPlatform(collision.gameObject);
       // }
        
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

    

    public void RespawnPlayer()
    {
        BigJoeRespawn();
    }


    public void BigJoeRespawn(bool returnToCheckpoint = false)
    {
        if (isAlive && !returnToCheckpoint)
        {
            //Unlink the platform on a player death.
            UnlinkPlatform(movingPlatform);
            //Play the death sound
            characterSoundManager.PlayAudioCallout(CharacterAudioCallout.Death);
            //If the player dies we respawn them. 
            isAlive = false;
            StopCoroutine(DisableMovement(0));
            if(!Coroutine.ReferenceEquals(latentAttack, null)) StopCoroutine(latentAttack);
            attackCooldownActive = false;
            playerIsAttacking = false;
            StartCoroutine(DisableMovement(1f));
            playerRigidBody.velocity = Vector3.zero;
            if (respawnJoe == null) StartCoroutine(RespawnJoe());
        }
        else if (returnToCheckpoint && respawnJoe == null)
        {
            StartCoroutine(RespawnJoe(true));
        }
    }

    IEnumerator RespawnJoe(bool returnToCheckpoint = false)
    {
        disableAllMoves = true;
        playerInput.DeactivateInput();
        if (returnToCheckpoint)
        {
            playerHealth.isInvincible = true;
            playerSprite.DOFade(0, 0.25f);
            playerSpriteRubber.DOFade(0, 0.25f);
            yield return new WaitForSeconds(0.25f);
            playerAnimator.Play("BigJoeIdle");
            playerAnimatorRubber.Play("BigJoeIdle");
            playerSprite.DOFade(1, .25f);
            playerSpriteRubber.DOFade(1, .25f);
        }
        else
        {
            playerAnimator.SetTrigger("Death");
            playerAnimatorRubber.SetTrigger("Death");
            yield return new WaitForSeconds(1);
            playerSprite.DOFade(0, 1);
            playerSpriteRubber.DOFade(0, 1);
            yield return new WaitForSeconds(1);
            playerAnimator.Play("BigJoeIdle");
            playerAnimatorRubber.Play("BigJoeIdle");
            playerSprite.DOFade(1, .5f);
            playerSpriteRubber.DOFade(1, .5f);
            playerHealth.HealthToMax();
        }
        isAlive = true;
        disableAllMoves = false;
        gameObject.transform.position = currentSpawn.transform.position;
        playerHealth.isInvincible = false;
        playerInput.ActivateInput();
    }

    public Health GetHealthComponent()
    {
        return playerHealth;
    }


    public void RadiationCanisterPickup(UpgradeType upgradeType)
    {
        //Handle radiation canister upgrades/pickups.
        //in retrospect this is a bad way to handle this but since we lack time I will keep it this way
        switch (upgradeType)
        {
            case UpgradeType.Health_Upgrade:
                playerHealth.IncreaseHealthCap(0.15f);
                break;
            case UpgradeType.Attack_Upgrade:
                playerUpgrade.UpgradeAttackDamage(0.15f);
                break;
            case UpgradeType.GroundPound_Ability:
                canGroundPound = true;
                break;
            case UpgradeType.WallClimb_Ability: 
                canWallClimb = true;
                canWallJump = true;
                break;
            case UpgradeType.DoubleJump_Ability: 
                canDoubleJump = true;
                break;
            case UpgradeType.WallJump_Ability:
                canWallClimb = true;
                canWallJump = true;
                break;
            case UpgradeType.Dagger_Weapon:
                playerUpgrade.UnlockWeapon(PlayerWeaponType.Dagger);
                break;
            case UpgradeType.LaserGun_Weapon:
                playerUpgrade.UnlockWeapon(PlayerWeaponType.LaserGun);
                break;
            case UpgradeType.ChainWhip_Weapon:
                playerUpgrade.UnlockWeapon(PlayerWeaponType.ChainWhip);
                break;
            case UpgradeType.Dash_Ability:
                canDash = true;
                break;
            case UpgradeType.PhaseShift_Ability:
                canPhaseShift = true;
                break;
            case UpgradeType.RageMode_Ability:
                canEnterRageMode = true;
                break;
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
        //Debug.Log("Set player speed");
        playerSpeed_Game = setSpeed;

        if (playerSpeed_Game < playerSpeed) canJump = false;
        else canJump = true;    
    }

    public void SetMovingPlatform(GameObject platform)
    {
       movingPlatform = platform;
       gameObject.transform.SetParent(movingPlatform.transform, true);
    }

    public void UnlinkPlatform(GameObject platform)
    {
        movingPlatform = null;
        gameObject.transform.SetParent(null, true);
        Debug.Log("Creating new player instance");
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Coll");
        if (collision.gameObject.GetComponentInParent<MovingPlatform>() != null)
        {
            SetMovingPlatform(collision.gameObject);
        }


        if((isDashing || isGroundPounding) && (!GameObject.ReferenceEquals(collision.gameObject.GetComponent<EnemyAI>(), null)))
        {
            Debug.Log("Coll2");
            if (isDashing)
            {
                collision.gameObject.GetComponent<EnemyAI>().GetHealth().SubtractFromHealth(playerUpgrade.GetAttackDamage(PlayerAttackType.DaggerStrike));
            }
            else if(isGroundPounding)
            {
                collision.gameObject.GetComponent<EnemyAI>().GetHealth().SubtractFromHealth(playerUpgrade.GetAttackDamage(PlayerAttackType.GroundPound));
            }
            if (!GameObject.ReferenceEquals(collision.gameObject.GetComponent<OneHitHealthEnemy>(), null))
            {
                collision.gameObject.GetComponent<OneHitHealthEnemy>().OnOneHitEnemyDeath();
            }
        }

        //To activate switches
        if ((isDashing))
        {
            if ((!GameObject.ReferenceEquals(collision.gameObject.GetComponent<Switch>(), null)))
            {
                collision.collider.gameObject.GetComponent<Switch>().ToggleSwitch(PlayerAttackType.DaggerStrike);
            }
            if (!GameObject.ReferenceEquals(collision.gameObject.GetComponent<OneHitHealthEnemy>(), null))
            {
                collision.gameObject.GetComponent<OneHitHealthEnemy>().OnOneHitEnemyDeath();
            }
        }



        

    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponentInParent<MovingPlatform>() != null)
        {
            UnlinkPlatform(collision.gameObject);
        }
    }


    public void OnBack(InputAction.CallbackContext context)
    {
        Debug.Log("HI");
        switch (context.phase)
        {
            case InputActionPhase.Started:
                Debug.Log("HI");
                /*
                if (sceneTransitionerManager != null)
                {
                    sceneTransitionerManager.StartTransition();
                }
                */
                //Pull up pause menu here

                pauseMenu.ToggleMenu();
                break;
        }
    }

    public void PlayDamagedAnim()
    {
        playerAnimator.Play("BigJoeHurt", 0);
        playerAnimatorRubber.Play("BigJoeHurt", 0);
        if (hurtCoroutine == null) hurtCoroutine = StartCoroutine(PlayHurtSound());
    }

    IEnumerator PlayHurtSound()
    {
        //We use coroutines to prevent a sound playing every frame and absoloutely obliterating the player's ears
        yield return new WaitForSeconds(characterSoundManager.PlayAudioCallout(CharacterAudioCallout.Hurt).length + 0.5f);
        hurtCoroutine = null;
    }

    public PlayerController GetPlayerController()
    {
        return this;
    }

    public Health GetHealth()
    {
        return GetHealthComponent();
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
    public PlayerController GetPlayerController();

    public Health GetHealth();
}

