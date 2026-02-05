using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 10f;
    [SerializeField] private float acceleration = 40f;
    [SerializeField] private float deceleration = 40f;
    
    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.3f;
    [SerializeField] private LayerMask groundLayer;
    
    [Header("Attack Settings")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private LayerMask enemyLayer;
    
    [Header("Scale Settings")]
    [SerializeField] private float scaleReturnSpeed = 0.3f;
    
    [Header("Animation")]
    [SerializeField] private Animator animator;
    
    [Header("UI Buttons")]
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;
    [SerializeField] private Button jumpButton;
    [SerializeField] private Button attackButton;
    
    private Rigidbody rb;
    private float moveInput;
    private float currentVelocity;
    private bool isRunning;
    private bool isFacingRight = true;
    private bool isGrounded;
    private float buttonMoveInput;
    private float targetScale = 1f;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX;
        
        if (animator == null) animator = GetComponent<Animator>();
        
        SetupButtons();
        
        if (GameDataManager.Instance != null && GameDataManager.Instance.returningFromAR)
        {
            Vector3 arDelta = GameDataManager.Instance.arMovementDelta;
            transform.position += new Vector3(0, 0, arDelta.x);
            targetScale = GameDataManager.Instance.scaleMultiplier;
            transform.localScale = Vector3.one * targetScale;
            GameDataManager.Instance.returningFromAR = false;
        }
    }
    
    void SetupButtons()
    {
        if (leftButton != null) AddEventTrigger(leftButton, () => buttonMoveInput = -1f, () => buttonMoveInput = 0f);
        if (rightButton != null) AddEventTrigger(rightButton, () => buttonMoveInput = 1f, () => buttonMoveInput = 0f);
        if (jumpButton != null) jumpButton.onClick.AddListener(Jump);
        if (attackButton != null) attackButton.onClick.AddListener(Attack);
    }
    
    void AddEventTrigger(Button button, System.Action onDown, System.Action onUp)
    {
        var trigger = button.gameObject.GetComponent<UnityEngine.EventSystems.EventTrigger>();
        if (trigger == null) trigger = button.gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();
        
        var downEntry = new UnityEngine.EventSystems.EventTrigger.Entry { eventID = UnityEngine.EventSystems.EventTriggerType.PointerDown };
        downEntry.callback.AddListener((data) => onDown());
        trigger.triggers.Add(downEntry);
        
        var upEntry = new UnityEngine.EventSystems.EventTrigger.Entry { eventID = UnityEngine.EventSystems.EventTriggerType.PointerUp };
        upEntry.callback.AddListener((data) => onUp());
        trigger.triggers.Add(upEntry);
    }
    
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
        
        float keyboardInput = 0f;
        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) keyboardInput = -1f;
            else if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) keyboardInput = 1f;
            isRunning = Keyboard.current.leftShiftKey.isPressed;
        }
        
        moveInput = buttonMoveInput != 0f ? buttonMoveInput : keyboardInput;
        
        if (moveInput > 0 && !isFacingRight) Flip();
        else if (moveInput < 0 && isFacingRight) Flip();
        
        if (Mathf.Abs(transform.localScale.x - 1f) > 0.01f)
        {
            float newScale = Mathf.MoveTowards(Mathf.Abs(transform.localScale.x), 1f, scaleReturnSpeed * Time.deltaTime);
            transform.localScale = new Vector3(transform.localScale.x < 0 ? -newScale : newScale, newScale, newScale);
        }
        
        if (animator != null)
        {
            animator.SetFloat("Speed", Mathf.Abs(moveInput));
            animator.SetBool("IsRunning", isRunning && moveInput != 0);
            animator.SetBool("IsGrounded", isGrounded);
        }
    }
    
    void FixedUpdate()
    {
        float targetVelocity = moveInput * (isRunning ? runSpeed : walkSpeed);
        float accel = Mathf.Abs(targetVelocity) > 0.01f ? acceleration : deceleration;
        
        currentVelocity = Mathf.MoveTowards(currentVelocity, targetVelocity, accel * Time.fixedDeltaTime);
        rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, currentVelocity);
    }
    
    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
    
    public void Jump()
    {
        if (isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
            if (animator != null) animator.SetTrigger("Jump");
        }
    }
    
    public void Attack()
    {
        if (animator != null) animator.SetTrigger("Attack");
        
        Collider[] hits = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayer);
        foreach (Collider hit in hits)
        {
            Enemy2D enemy = hit.GetComponent<Enemy2D>();
            if (enemy != null) enemy.TakeDamage(1);
        }
    }
    
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
        if (attackPoint != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }
}