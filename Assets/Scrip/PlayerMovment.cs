using UnityEngine;
using UnityEngine.InputSystem; 

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 7f;
    [SerializeField] private float normalJumpForce = 15f;   
    [SerializeField] private float highJumpForce = 22f;    
    
    // --- GROUND CHECK VARIABLES ---
    [SerializeField] private Transform groundCheck;      // Titik di kaki player untuk mendeteksi tanah
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;      // Layer khusus untuk tanah/platform
    private bool isGrounded;
    private bool isChargingJump;                         // Memastikan lompatan dimulai dari tanah

    // --- DASH VARIABLES ---
    [SerializeField] private float dashForce = 25f;       
    [SerializeField] private float dashDuration = 0.2f;    
    private float dashTimer;
    private bool isDashing;
    private float facingDirection = 1f;                    

    private Rigidbody2D body;
    private float spacePressedTime; 

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (isDashing)
        {
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0)
            {
                isDashing = false;
            }
            return; 
        }

        // Cek apakah player menyentuh tanah
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // --- MOVE RIGHT & LEFT ---
        float moveInput = 0f;

        if (Keyboard.current.dKey.isPressed)
        {
            moveInput = 1f;
            facingDirection = 1f;
        }
        else if (Keyboard.current.aKey.isPressed)
        {
            moveInput = -1f;
            facingDirection = -1f;
        }

        body.linearVelocity = new Vector2(moveInput * speed, body.linearVelocity.y);

        // --- CHARGED JUMPING ---
        // Hanya bisa mulai nge-charge kalau sedang di tanah
        if (isGrounded && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            spacePressedTime = Time.time; 
            isChargingJump = true;
        }

        // Eksekusi lompatan saat tombol dilepas
        if (Keyboard.current.spaceKey.wasReleasedThisFrame && isChargingJump)
        {
            isChargingJump = false;

            // Pastikan masih di tanah saat tombol dilepas (mencegah lompat jika jatuh dari tebing saat nge-charge)
            if (isGrounded)
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
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("DashObject"))
        {
            StartDash();
        }
    }

    private void StartDash()
    {
        isDashing = true;
        dashTimer = dashDuration;
        
        body.linearVelocity = new Vector2(facingDirection * dashForce, body.linearVelocity.y);
    }

    // --- BANTUAN VISUAL DI EDITOR ---
    // Menggambar lingkaran merah di kaki player saat di dalam Unity Editor agar mudah diatur ukurannya
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}