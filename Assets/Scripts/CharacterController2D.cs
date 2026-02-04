using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class CharacterController2D : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 10f;
    
    [Header("Animation")]
    [SerializeField] private Animator animator;
    
    [Header("UI Buttons (Optional - for Mobile)")]
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;
    [SerializeField] private Button attackButton;
    
    // Private variables
    private Rigidbody rb;
    private float moveInput;
    private bool isRunning = false;
    private bool isFacingRight = true;
    
    // For UI button control
    private float buttonMoveInput = 0f;
    
    // AR mode flag
    private bool isInARMode = false;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        // Get animator if not assigned
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        
        // Setup button listeners if buttons are assigned
        SetupButtons();
        
        Debug.Log("Character Controller Started!");
    }
    
    void SetupButtons()
    {
        // Setup Left Button
        if (leftButton != null)
        {
            var leftTrigger = leftButton.gameObject.GetComponent<UnityEngine.EventSystems.EventTrigger>();
            if (leftTrigger == null)
            {
                leftTrigger = leftButton.gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();
            }
            
            var pointerDownLeft = new UnityEngine.EventSystems.EventTrigger.Entry();
            pointerDownLeft.eventID = UnityEngine.EventSystems.EventTriggerType.PointerDown;
            pointerDownLeft.callback.AddListener((data) => { OnLeftButtonDown(); });
            leftTrigger.triggers.Add(pointerDownLeft);
            
            var pointerUpLeft = new UnityEngine.EventSystems.EventTrigger.Entry();
            pointerUpLeft.eventID = UnityEngine.EventSystems.EventTriggerType.PointerUp;
            pointerUpLeft.callback.AddListener((data) => { OnMoveButtonUp(); });
            leftTrigger.triggers.Add(pointerUpLeft);
        }
        
        // Setup Right Button
        if (rightButton != null)
        {
            var rightTrigger = rightButton.gameObject.GetComponent<UnityEngine.EventSystems.EventTrigger>();
            if (rightTrigger == null)
            {
                rightTrigger = rightButton.gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();
            }
            
            var pointerDownRight = new UnityEngine.EventSystems.EventTrigger.Entry();
            pointerDownRight.eventID = UnityEngine.EventSystems.EventTriggerType.PointerDown;
            pointerDownRight.callback.AddListener((data) => { OnRightButtonDown(); });
            rightTrigger.triggers.Add(pointerDownRight);
            
            var pointerUpRight = new UnityEngine.EventSystems.EventTrigger.Entry();
            pointerUpRight.eventID = UnityEngine.EventSystems.EventTriggerType.PointerUp;
            pointerUpRight.callback.AddListener((data) => { OnMoveButtonUp(); });
            rightTrigger.triggers.Add(pointerUpRight);
        }
        
        // Setup Attack Button
        if (attackButton != null)
        {
            attackButton.onClick.AddListener(OnAttackButton);
        }
    }
    
    void Update()
    {
        // Don't process input if in AR mode
        if (isInARMode) return;
        
        // Combine keyboard and button input
        float keyboardInput = 0f;
        
        // Check for keyboard input (for PC testing)
        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
                keyboardInput = -1f;
            else if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
                keyboardInput = 1f;
                
            // Run with shift
            isRunning = Keyboard.current.leftShiftKey.isPressed;
        }
        
        // Combine both inputs
        moveInput = buttonMoveInput != 0f ? buttonMoveInput : keyboardInput;
        
        // Handle sprite/model flipping
        if (moveInput > 0 && !isFacingRight)
        {
            Flip();
        }
        else if (moveInput < 0 && isFacingRight)
        {
            Flip();
        }
        
        // Update animator parameters if animator exists
        if (animator != null)
        {
            animator.SetFloat("Speed", Mathf.Abs(moveInput));
            animator.SetBool("IsRunning", isRunning && moveInput != 0);
        }
    }
    
    void FixedUpdate()
    {
        // Don't move if in AR mode
        if (isInARMode) return;
        
        // Calculate movement speed
        float currentSpeed = isRunning ? runSpeed : walkSpeed;
        
        // Apply movement on Z axis for 2.5D
        Vector3 newVelocity = new Vector3(0f, rb.linearVelocity.y, moveInput * currentSpeed);
        rb.linearVelocity = newVelocity;
    }
    
    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.z *= -1;
        transform.localScale = scale;
    }
    
    // ===== AR MODE CALLBACKS =====
    
    public void EnterARMode()
    {
        isInARMode = true;
        
        // Stop all movement
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
        }
        
        Debug.Log("Character entering AR mode");
    }
    
    public void ExitARMode(Vector3 arMovementDelta, Vector3 newScale)
    {
        isInARMode = false;
        Debug.Log($"Character exited AR mode - Movement: {arMovementDelta}, Scale: {newScale}");
    }
    
    // ===== UI BUTTON CALLBACKS =====
    
    public void OnLeftButtonDown()
    {
        buttonMoveInput = -1f;
    }
    
    public void OnRightButtonDown()
    {
        buttonMoveInput = 1f;
    }
    
    public void OnMoveButtonUp()
    {
        buttonMoveInput = 0f;
    }
    
    public void OnAttackButton()
    {
        Attack();
    }
    
    void Attack()
    {
        Debug.Log("Attack button pressed!");
        
        if (animator != null)
        {
            // animator.SetTrigger("Attack");
        }
    }
}