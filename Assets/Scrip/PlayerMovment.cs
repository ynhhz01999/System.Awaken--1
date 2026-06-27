using UnityEngine;
using UnityEngine.InputSystem; 

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 6f;
    [SerializeField] private float normalJumpForce = 10f;   
    [SerializeField] private float highJumpForce = 16f;    
    
    // --- DASH VARIABLES ---
    [SerializeField] private float dashForce = 25f;       // Speed of the dash
    [SerializeField] private float dashDuration = 0.2f;    // How long the dash lasts
    private float dashTimer;
    private bool isDashing;
    private float facingDirection = 1f;                    // 1 = Right, -1 = Left

    private Rigidbody2D body;
    private float spacePressedTime; 

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // If we are currently dashing, stop regular movement code and just handle the dash countdown
        if (isDashing)
        {
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0)
            {
                isDashing = false;
                body.gravityScale = 1f; // Restore normal gravity after dash ends
            }
            return; // Exit update early so movement/jumping keys are ignored while dashing
        }

        // --- MOVE RIGHT & LEFT ---
        float moveInput = 0f;

        if (Keyboard.current.dKey.isPressed)
        {
            moveInput = 1f;
            facingDirection = 1f; // Facing Right
        }
        else if (Keyboard.current.aKey.isPressed)
        {
            moveInput = -1f;
            facingDirection = -1f; // Facing Left
        }

        body.linearVelocity = new Vector2(moveInput * speed, body.linearVelocity.y);

        // --- CHARGED JUMPING ---
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            spacePressedTime = Time.time; 
        }

        if (Keyboard.current.spaceKey.wasReleasedThisFrame)
        {
            float holdDuration = Time.time - spacePressedTime;

            if (holdDuration >= 1f)
            {
                body.linearVelocity = new Vector2(body.linearVelocity.x, highJumpForce);
            }
            else
            {
                body.linearVelocity = new Vector2(body.linearVelocity.x, normalJumpForce);
            }
        }
    }

    // --- TRIGGER DETECTION FOR THE DASH OBJECT ---
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the object we hit has the "DashObject" tag
        if (collision.CompareTag("DashObject"))
        {
            StartDash();
        }
    }

    private void StartDash()
    {
        isDashing = true;
        dashTimer = dashDuration;
        
        // Turn off gravity temporarily so the player doesn't fall downwards mid-dash
        body.gravityScale = 0f; 
        
        // Launch the player horizontally in the direction they are facing
        body.linearVelocity = new Vector2(facingDirection * dashForce, 0f);
    }
}


//Skills isu Bro