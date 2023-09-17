using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private PlayerInput playerInput;
    [SerializeField] private float playerSpeed = 500f;
    [SerializeField] private float jumpForce = 15f;


    //Do box trace down to figure out if the player is grounded.
    private bool isGrounded = true;
    private bool isJumping = false;





    private Vector2 movementDirection;
    private Rigidbody2D playerRigidBody;
    private BoxCollider2D playerCollider;
    private SpriteRenderer spriteRenderer;


    
    // Start is called before the first frame update
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        playerRigidBody = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
    }



    void HandleInput()
    {
        if(movementDirection !=  Vector2.zero)
        {
            playerRigidBody.velocity = new Vector2(movementDirection.x * playerSpeed * Time.deltaTime, playerRigidBody.velocity.y);
        }
        else playerRigidBody.velocity = new Vector2(0, playerRigidBody.velocity.y);
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
                Debug.Log("Jump button pressed");
                if (isGrounded) playerRigidBody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                break;
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

