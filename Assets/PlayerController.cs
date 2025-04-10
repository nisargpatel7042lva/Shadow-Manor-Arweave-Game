using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5.5f;
    public float walkSpeed = 5.5f; // Normal speed
    public float sneakSpeed = 2.2f; // Slower speed when sneaking (Valorant's walk speed)
    
    [Header("Camera Settings")]
    [Range(0.001f, 0.5f)]
    public float mouseSensitivity = 0.05f;
    public Transform playerCamera;
    
    [Header("Jump Settings")]
    public float initialJumpVelocity = 5.2f; // Valorant's initial jump velocity
    public float maxJumpHeight = 1.05f; // Valorant's jump height
    public float jumpDuration = 0.5f; // Time to reach peak of jump
    public float gravity = -22f; // Calculated for Valorant-like jump arc
    
    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.2f;
    public LayerMask groundMask;
    
    [Header("Audio")]
    public AudioSource jumpAudio;
    public AudioSource landingAudio;
    
    // Private variables
    private CharacterController controller;
    private float xRotation = 0f;
    private Vector3 playerVelocity;
    private bool isGrounded;
    private bool wasGrounded;
    private bool isSneaking = false;
    private bool isJumping = false;
    private float cameraOriginalHeight;
    private float cameraDip = 0f;
    
    // Mouse input variables
    private Vector2 previousMousePosition;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        // If groundCheck wasn't assigned, create one
        if (groundCheck == null)
        {
            GameObject check = new GameObject("GroundCheck");
            check.transform.parent = transform;
            check.transform.localPosition = new Vector3(0, -0.9f, 0);
            groundCheck = check.transform;
        }
        
        // Save original camera height
        if (playerCamera != null)
        {
            cameraOriginalHeight = playerCamera.localPosition.y;
        }
        
        // Store initial mouse position
        previousMousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        
        // Calculate gravity based on desired jump height and duration (Valorant-like)
        // g = -2 * jumpHeight / (jumpDuration/2)^2
        gravity = (-2 * maxJumpHeight) / Mathf.Pow(jumpDuration/2, 2);
        
        // Calculate initial jump velocity based on gravity and jump height
        // v0 = sqrt(-2 * g * jumpHeight)
        initialJumpVelocity = Mathf.Sqrt(-2 * gravity * maxJumpHeight);

        void Start()
{
    if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 5f, NavMesh.AllAreas))
    {
        transform.position = hit.position;
        Debug.Log("Player repositioned to NavMesh!");
    }
    else
    {
        Debug.LogError("Player could not find a valid NavMesh position!");
    }
}
    }

    void Update()
    {
        // Track previous grounded state (for landing detection)
        wasGrounded = isGrounded;
        
        CheckGround();
        HandleJump();
        ApplyGravity();
        HandleSneaking();
        PlayerMovement();
        CameraLook();
        UpdateJumpVisuals();
        
        // Detect landing
        if (!wasGrounded && isGrounded)
        {
            OnLanding();
        }
    }
    
    void CheckGround()
    {
        // Check if the player is grounded using a sphere cast
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
    }
    
    void HandleJump()
    {
        // Reset velocity when on ground
        if (isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = -2f; // Keep a small downward force for better grounding
            isJumping = false;
        }
        
        // Jump when Space is pressed and player is grounded
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            // Apply Valorant-like jump force
            playerVelocity.y = initialJumpVelocity;
            isJumping = true;
            
            // Apply camera dip effect on jump start (Valorant style)
            cameraDip = 0.06f; // Amount of initial camera dip
            
            // Play jump sound
            if (jumpAudio != null)
            {
                jumpAudio.pitch = Random.Range(0.97f, 1.03f); // Slight pitch variation
                jumpAudio.Play();
            }
        }
    }
    
    void ApplyGravity()
    {
        // Apply gravity - using Valorant-tuned gravity
        playerVelocity.y += gravity * Time.deltaTime;
        
        // Apply a maximum fall speed (Valorant caps fall speed)
        if (playerVelocity.y < -20f)
            playerVelocity.y = -20f;
        
        // Apply vertical movement
        controller.Move(playerVelocity * Time.deltaTime);
    }
    
    void OnLanding()
    {
        // Apply camera dip on landing (Valorant style)
        cameraDip = 0.04f;
        
        // Play landing sound if falling fast enough
        if (landingAudio != null && playerVelocity.y < -8f)
        {
            // Adjust volume based on fall speed
            float volume = Mathf.InverseLerp(-8f, -20f, playerVelocity.y);
            landingAudio.volume = Mathf.Clamp(volume, 0.3f, 1.0f);
            landingAudio.pitch = Random.Range(0.95f, 1.05f);
            landingAudio.Play();
        }
    }
    
    void HandleSneaking()
    {
        // Toggle sneaking when Shift is held (Valorant walk)
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            isSneaking = true;
            moveSpeed = sneakSpeed;
        }
        
        if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift))
        {
            isSneaking = false;
            moveSpeed = walkSpeed;
        }
    }

    void PlayerMovement()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        
        // Normalize movement vector (Valorant has consistent speed in all directions)
        if (move.magnitude > 1f)
            move.Normalize();
        
        // Move the player
        controller.Move(move * moveSpeed * Time.deltaTime);
    }

    void CameraLook()
    {
        // Get raw mouse position
        Vector2 currentMousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        
        // Calculate mouse delta
        Vector2 mouseDelta = currentMousePosition - previousMousePosition;
        
        // Apply sensitivity
        mouseDelta *= mouseSensitivity;
        
        // Update rotation values
        xRotation -= mouseDelta.y;
        xRotation = Mathf.Clamp(xRotation, -89f, 89f);
        
        // Apply vertical rotation
        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        
        // Apply horizontal rotation
        transform.Rotate(Vector3.up * mouseDelta.x);
        
        // Update previous position
        previousMousePosition = currentMousePosition;
        
        // Unlock cursor with Escape key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ? 
                               CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = !Cursor.visible;
        }
    }
    
    void UpdateJumpVisuals()
    {
        if (playerCamera == null)
            return;
        
        // Get current camera position
        Vector3 cameraPos = playerCamera.localPosition;
        
        // Handle camera dip effects (Valorant style)
        if (cameraDip > 0)
        {
            // Apply dip for jump start or landing
            cameraPos.y = cameraOriginalHeight - cameraDip;
            
            // Reduce dip over time (recover)
            cameraDip = Mathf.Lerp(cameraDip, 0, Time.deltaTime * 12f);
            if (cameraDip < 0.001f)
                cameraDip = 0;
        }
        else if (isJumping)
        {
            // Very subtle movement while in air (Valorant has minimal camera movement in air)
            float airborneOffset = Mathf.Sin(Time.time * 2f) * 0.005f;
            cameraPos.y = cameraOriginalHeight + airborneOffset;
        }
        else if (isGrounded)
        {
            // Return to normal when on ground
            cameraPos.y = Mathf.Lerp(cameraPos.y, cameraOriginalHeight, Time.deltaTime * 10f);
        }
        
        // Apply camera position
        playerCamera.localPosition = cameraPos;
    }
}