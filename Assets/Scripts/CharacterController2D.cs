using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class CharacterController2D : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float walkSpeed = 7f;
    [SerializeField] private float runSpeed = 12f;
    [SerializeField] private float acceleration = 60f;
    [SerializeField] private float deceleration = 60f;
    [SerializeField] private float airAcceleration = 40f;
    
    [Header("Jump")]
    [SerializeField] private float jumpForce = 15f;
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 2f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.3f;
    [SerializeField] private float coyoteTime = 0.15f;
    [SerializeField] private float jumpBufferTime = 0.2f;
    
    [Header("Attack")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float attackCooldown = 0.35f;
    
    [Header("Animation")]
    [SerializeField] private Animator animator;
    
    [Header("UI Buttons")]
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;
    [SerializeField] private Button jumpButton;
    [SerializeField] private Button attackButton;
    
    [Header("Scale Reset")]
    [SerializeField] private float scaleResetTime = 5f;
    
    private Rigidbody rb;
    private float moveInput;
    private float buttonMoveInput = 0f;
    private float currentVelocityX = 0f;
    private bool isGrounded;
    private bool isRunning = false;
    private bool isFacingRight = true;
    private bool canAttack = true;
    private float attackTimer = 0f;
    private float scaleTimer = 0f;
    private float targetScale = 1f;
    private bool needsScaleReset = false;
    
    private float coyoteTimeCounter;
    private float jumpBufferCounter;
    private bool jumpPressed = false;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        
        if (animator == null) animator = GetComponent<Animator>();
        
        SetupButtons();
        CheckARReturn();
    }
    
    void CheckARReturn()
    {
        if (ARDataManager.Instance != null && ARDataManager.Instance.HasARData)
        {
            Vector3 arMovement = ARDataManager.Instance.ArMovementDelta;
            float arScale = ARDataManager.Instance.ArScale;
            
            float virtualXMovement = arMovement.x * 10f;
            Vector3 newPosition = ARDataManager.Instance.VirtualStartPosition + new Vector3(virtualXMovement, 0, 0);
            transform.position = newPosition;
            
            if (arScale != 1f)
            {
                transform.localScale = Vector3.one * arScale;
                targetScale = arScale;
                needsScaleReset = true;
                scaleTimer = 0f;
            }
            
            ARDataManager.Instance.ClearARData();
        }
    }
    
    void SetupButtons()
    {
        if (leftButton != null)
        {
            var leftTrigger = leftButton.gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();
            var pdLeft = new UnityEngine.EventSystems.EventTrigger.Entry { eventID = UnityEngine.EventSystems.EventTriggerType.PointerDown };
            pdLeft.callback.AddListener((data) => { buttonMoveInput = -1f; });
            leftTrigger.triggers.Add(pdLeft);
            var puLeft = new UnityEngine.EventSystems.EventTrigger.Entry { eventID = UnityEngine.EventSystems.EventTriggerType.PointerUp };
            puLeft.callback.AddListener((data) => { buttonMoveInput = 0f; });
            leftTrigger.triggers.Add(puLeft);
        }
        
        if (rightButton != null)
        {
            var rightTrigger = rightButton.gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();
            var pdRight = new UnityEngine.EventSystems.EventTrigger.Entry { eventID = UnityEngine.EventSystems.EventTriggerType.PointerDown };
            pdRight.callback.AddListener((data) => { buttonMoveInput = 1f; });
            rightTrigger.triggers.Add(pdRight);
            var puRight = new UnityEngine.EventSystems.EventTrigger.Entry { eventID = UnityEngine.EventSystems.EventTriggerType.PointerUp };
            puRight.callback.AddListener((data) => { buttonMoveInput = 0f; });
            rightTrigger.triggers.Add(puRight);
        }
        
        if (jumpButton != null) jumpButton.onClick.AddListener(() => jumpPressed = true);
        if (attackButton != null) attackButton.onClick.AddListener(Attack);
    }
    
    void Update()
    {
        CheckGrounded();
        
        float keyboardInput = 0f;
        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) keyboardInput = -1f;
            else if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) keyboardInput = 1f;
            
            isRunning = Keyboard.current.leftShiftKey.isPressed;
            
            if (Keyboard.current.spaceKey.wasPressedThisFrame) jumpPressed = true;
            if (Keyboard.current.jKey.wasPressedThisFrame) Attack();
        }
        
        moveInput = buttonMoveInput != 0f ? buttonMoveInput : keyboardInput;
        
        if (moveInput > 0 && !isFacingRight) Flip();
        else if (moveInput < 0 && isFacingRight) Flip();
        
        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }
        
        if (jumpPressed)
        {
            jumpBufferCounter = jumpBufferTime;
            jumpPressed = false;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }
        
        if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f)
        {
            Jump();
            jumpBufferCounter = 0f;
        }
        
        if (animator != null)
        {
            animator.SetFloat("Speed", Mathf.Abs(currentVelocityX));
            animator.SetBool("IsGrounded", isGrounded);
            animator.SetBool("IsRunning", isRunning && moveInput != 0);
            animator.SetFloat("VelocityY", rb.linearVelocity.y);
        }
        
        if (!canAttack)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer >= attackCooldown)
            {
                canAttack = true;
                attackTimer = 0f;
            }
        }
        
        if (needsScaleReset)
        {
            scaleTimer += Time.deltaTime;
            float progress = scaleTimer / scaleResetTime;
            float currentScaleValue = Mathf.Lerp(targetScale, 1f, progress);
            transform.localScale = new Vector3(isFacingRight ? currentScaleValue : -currentScaleValue, currentScaleValue, currentScaleValue);
            
            if (scaleTimer >= scaleResetTime)
            {
                transform.localScale = new Vector3(isFacingRight ? 1f : -1f, 1f, 1f);
                targetScale = 1f;
                needsScaleReset = false;
                scaleTimer = 0f;
            }
        }
    }
    
    void FixedUpdate()
    {
        float targetSpeed = moveInput * (isRunning ? runSpeed : walkSpeed);
        float accelRate = Mathf.Abs(targetSpeed) > 0.01f ? acceleration : deceleration;
        
        if (!isGrounded) accelRate = airAcceleration;
        
        float speedDiff = targetSpeed - currentVelocityX;
        float movement = speedDiff * accelRate;
        
        currentVelocityX = Mathf.MoveTowards(currentVelocityX, targetSpeed, Mathf.Abs(movement) * Time.fixedDeltaTime);
        
        rb.linearVelocity = new Vector3(currentVelocityX, rb.linearVelocity.y, 0f);
        
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if (rb.linearVelocity.y > 0)
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
        }
    }
    
    void Jump()
    {
        if (!isGrounded) return;
        
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, 0f);
        coyoteTimeCounter = 0f;
    }
    
    public void Attack()
    {
        if (!canAttack) return;
        
        canAttack = false;
        
        if (animator != null) animator.SetTrigger("Attack");
        
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayer);
        
        foreach (Collider enemy in hitEnemies)
        {
            Debug.Log("Hit enemy: " + enemy.name);
        }
    }
    
    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
    
    void CheckGrounded()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
    }
    
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
        
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }
}