using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float acceleration = 50f;
    [SerializeField] private float deceleration = 50f;
    [SerializeField] private float airAcceleration = 15f;
    [SerializeField] private float maxAirSpeed = 10f;

    [Header("Jumping")]
    [SerializeField] private float jumpForce = 14f;
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 2f;
    [SerializeField] private float coyoteTime = 0.15f;
    [SerializeField] private float jumpBufferTime = 0.1f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.4f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    [Header("UI Buttons (Mobile)")]
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;
    [SerializeField] private Button jumpButton;
    [SerializeField] private Button attackButton;

    [Header("Scale UI")]
    [SerializeField] private GameObject scaleTimerUI;
    [SerializeField] private UnityEngine.UI.Image scaleTimerFill;

    private Rigidbody rb;
    private float moveInput;
    private float buttonInput;
    private bool isGrounded;
    private bool wasGroundedLastFrame;
    private bool isFacingRight = true;
    private float coyoteTimeCounter;
    private float jumpBufferCounter;
    private bool jumpHeld;
    private bool hasJumped;
    private Vector3 baseScale;
    private float currentScaleMultiplier = 1f;
    private float scaleTimer;
    private bool hasAppliedARData;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        baseScale = transform.localScale;
        
        if (animator == null) animator = GetComponent<Animator>();
        
        SetupButtons();
        ApplyARData();
        
        if (groundLayer == 0)
        {
            groundLayer = LayerMask.GetMask("Ground", "Default");
            Debug.LogWarning("Ground Layer not set! Using Default layer.");
        }
        
        Debug.Log("PlayerController Started. GroundLayer: " + groundLayer.value);
    }

    void ApplyARData()
    {
        if (GameDataManager.Instance == null || !GameDataManager.Instance.ReturningFromAR || hasAppliedARData)
            return;

        hasAppliedARData = true;
        
        Vector3 savedPosition = GameDataManager.Instance.PlayerPositionBeforeAR;
        Vector3 arMovement = GameDataManager.Instance.ARMovementDelta;
        float arScale = GameDataManager.Instance.ARScaleMultiplier;
        
        // Scene is 180 degrees rotated, so invert the AR movement:
        // Left in AR (negative X/Z) = +X in game
        // Right in AR (positive X/Z) = -X in game
        float invertedMovementX = -(arMovement.x + arMovement.z);
        
        // Apply scale first
        currentScaleMultiplier = arScale;
        scaleTimer = GameDataManager.Instance.ScaleTimer;
        transform.localScale = baseScale * currentScaleMultiplier;
        
        // Calculate new X position with inverted AR movement
        float newX = savedPosition.x + invertedMovementX;
        
        // If scaled, drop from y=200 so the player lands on the ground properly
        // Otherwise, use the saved Y position
        float newY = (currentScaleMultiplier != 1f) ? 200f : savedPosition.y;
        
        transform.position = new Vector3(newX, newY, savedPosition.z);
        
        if (scaleTimerUI != null && currentScaleMultiplier != 1f)
            scaleTimerUI.SetActive(true);
        
        GameDataManager.Instance.ClearARData();
        
        Debug.Log($"Applied AR Data: Position={savedPosition}, Movement={arMovement}, InvertedX={invertedMovementX}, Scale={arScale}, DropY={newY}");
    }
    
    void ApplyScaleWithGroundAdjustment()
    {
        Vector3 oldScale = transform.localScale;
        Vector3 newScale = baseScale * currentScaleMultiplier;
        
        float scaleDiff = newScale.y - oldScale.y;
        float heightAdjustment = scaleDiff * 0.5f;
        
        transform.localScale = newScale;
        transform.position += new Vector3(0, heightAdjustment, 0);
    }

    void SetupButtons()
    {
        SetupButton(leftButton, () => buttonInput = -1f, () => { if (buttonInput < 0) buttonInput = 0; });
        SetupButton(rightButton, () => buttonInput = 1f, () => { if (buttonInput > 0) buttonInput = 0; });
        
        if (jumpButton != null)
        {
            SetupButton(jumpButton, () => { jumpBufferCounter = jumpBufferTime; jumpHeld = true; }, () => jumpHeld = false);
        }
        
        if (attackButton != null)
        {
            attackButton.onClick.AddListener(Attack);
        }
    }

    void SetupButton(Button button, System.Action onDown, System.Action onUp)
    {
        if (button == null) return;
        
        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>();
        if (trigger == null) trigger = button.gameObject.AddComponent<EventTrigger>();
        trigger.triggers.Clear();
        
        var down = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
        down.callback.AddListener(_ => onDown());
        trigger.triggers.Add(down);
        
        var up = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
        up.callback.AddListener(_ => onUp());
        trigger.triggers.Add(up);
    }

    void Update()
    {
        HandleInput();
        HandleGroundCheck();
        HandleJumpBuffer();
        HandleCoyoteTime();
        HandleScaleTimer();
        HandleFlip();
        UpdateAnimator();
    }

    void HandleInput()
    {
        float keyboardInput = 0f;
        
        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
                keyboardInput = -1f;
            else if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
                keyboardInput = 1f;
            
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                jumpBufferCounter = jumpBufferTime;
                jumpHeld = true;
            }
            if (Keyboard.current.spaceKey.wasReleasedThisFrame)
            {
                jumpHeld = false;
            }
        }
        
        moveInput = buttonInput != 0 ? buttonInput : keyboardInput;
    }

    void HandleGroundCheck()
    {
        wasGroundedLastFrame = isGrounded;
        
        if (groundCheck != null)
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
        }
        else
        {
            isGrounded = Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, 0.7f, groundLayer);
            if (!isGrounded)
            {
                isGrounded = Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, 0.7f);
            }
        }
        
        if (!wasGroundedLastFrame && isGrounded)
        {
            hasJumped = false;
            Debug.Log("Landed - jump reset");
        }

    }

    void HandleCoyoteTime()
    {
        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }
    }

    void HandleJumpBuffer()
    {
        if (jumpBufferCounter > 0)
        {
            jumpBufferCounter -= Time.deltaTime;
            
            if (coyoteTimeCounter > 0 && !hasJumped)
            {
                Jump();
                jumpBufferCounter = 0;
                coyoteTimeCounter = 0;
                hasJumped = true;
            }
        }
    }

    void HandleScaleTimer()
    {
        if (scaleTimer > 0)
        {
            float oldScale = currentScaleMultiplier;
            scaleTimer -= Time.deltaTime;
            
            if (scaleTimerFill != null)
                scaleTimerFill.fillAmount = scaleTimer / 10f;
            
            if (scaleTimer <= 0)
            {
                currentScaleMultiplier = 1f;
                ApplyScaleWithGroundAdjustment();
                if (scaleTimerUI != null) scaleTimerUI.SetActive(false);
            }
        }
        
        if (GameDataManager.Instance != null)
        {
            GameDataManager.Instance.UpdateScaleTimer(Time.deltaTime);
        }
    }

    void HandleFlip()
    {
        if (moveInput > 0 && !isFacingRight)
            Flip();
        else if (moveInput < 0 && isFacingRight)
            Flip();
    }

    void FixedUpdate()
    {
        HandleMovement();
        HandleGravity();
    }

    void HandleMovement()
    {
        float targetSpeed = moveInput * moveSpeed;
        float speedDiff = targetSpeed - rb.linearVelocity.x;
        float accelRate = isGrounded ? 
            (Mathf.Abs(targetSpeed) > 0.01f ? acceleration : deceleration) : 
            airAcceleration;
        
        float movement = speedDiff * accelRate * Time.fixedDeltaTime;
        float newVelocityX = rb.linearVelocity.x + movement;
        
        if (!isGrounded)
        {
            newVelocityX = Mathf.Clamp(newVelocityX, -maxAirSpeed, maxAirSpeed);
        }
        
        rb.linearVelocity = new Vector3(
            newVelocityX,
            rb.linearVelocity.y,
            0
        );
    }

    void HandleGravity()
    {
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if (rb.linearVelocity.y > 0 && !jumpHeld)
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
        }
    }

    void Jump()
    {
        Debug.Log("JUMP! Force: " + jumpForce);
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, 0);
        if (animator != null) animator.SetTrigger("Jump");
    }
    
    public void TryJump()
    {
        jumpBufferCounter = jumpBufferTime;
        jumpHeld = true;
    }
    
    public void ReleaseJump()
    {
        jumpHeld = false;
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 rotation = transform.eulerAngles;
        rotation.y = isFacingRight ? -90 : 90;
        transform.eulerAngles = rotation;
    }

    void UpdateAnimator()
    {
        if (animator == null) return;
        animator.SetFloat("Speed", Mathf.Abs(moveInput));
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetFloat("VelocityY", rb.linearVelocity.y);
    }

    void Attack()
    {
        if (animator != null) animator.SetTrigger("Attack");
    }

    public void ApplyJumpPadForce(float force)
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, force, 0);
        coyoteTimeCounter = 0;
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}