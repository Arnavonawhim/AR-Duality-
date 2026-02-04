using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class ARTeleporter : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject arPromptUI; // Panel with "Enter AR" button
    [SerializeField] private Button enterARButton;
    
    [Header("AR Manager")]
    [SerializeField] private ARManager arManager;
    
    [Header("Visual Feedback (Optional)")]
    [SerializeField] private GameObject teleporterVisual; // Glowing effect, particles, etc.
    [SerializeField] private Color highlightColor = Color.cyan;
    private Color originalColor;
    private Renderer teleporterRenderer;
    
    private bool playerInRange = false;
    private CharacterController2D playerController;
    
    void Start()
    {
        // Hide prompt at start
        if (arPromptUI != null)
            arPromptUI.SetActive(false);
        
        // Setup button listener
        if (enterARButton != null)
            enterARButton.onClick.AddListener(OnEnterARButtonPressed);
        
        // Get renderer for visual feedback
        if (teleporterVisual != null)
        {
            teleporterRenderer = teleporterVisual.GetComponent<Renderer>();
            if (teleporterRenderer != null)
            {
                originalColor = teleporterRenderer.material.color;
            }
        }
        
        Debug.Log("AR Teleporter initialized");
    }
    
    void OnTriggerEnter(Collider other)
    {
        // Check if player entered the teleporter
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            playerController = other.GetComponent<CharacterController2D>();
            
            // Show AR prompt
            if (arPromptUI != null)
                arPromptUI.SetActive(true);
            
            // Visual feedback - highlight teleporter
            if (teleporterRenderer != null)
            {
                teleporterRenderer.material.color = highlightColor;
            }
            
            Debug.Log("Player entered AR teleporter zone");
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        // Check if player left the teleporter
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            playerController = null;
            
            // Hide AR prompt
            if (arPromptUI != null)
                arPromptUI.SetActive(false);
            
            // Reset visual feedback
            if (teleporterRenderer != null)
            {
                teleporterRenderer.material.color = originalColor;
            }
            
            Debug.Log("Player left AR teleporter zone");
        }
    }
    
    void Update()
    {
        // Optional: Allow 'E' key to enter AR mode when in range (for PC testing)
        #if UNITY_EDITOR
        if (playerInRange && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            OnEnterARButtonPressed();
        }
        #endif
    }
    
    void OnEnterARButtonPressed()
    {
        if (playerInRange && playerController != null && arManager != null)
        {
            // Check if not already in AR mode
            if (!arManager.IsInARMode())
            {
                // Hide the prompt
                if (arPromptUI != null)
                    arPromptUI.SetActive(false);
                
                // Enter AR mode
                arManager.EnterARMode(playerController);
                
                Debug.Log("Entering AR Mode from teleporter!");
            }
            else
            {
                Debug.LogWarning("Already in AR mode!");
            }
        }
        else
        {
            Debug.LogWarning("Cannot enter AR mode - missing references or player not in range");
        }
    }
    
    // Optional: Draw gizmo in editor to see teleporter trigger zone
    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            Gizmos.DrawWireCube(transform.position, col.bounds.size);
        }
    }
}