using UnityEngine;
using UnityEngine.UI;

public class ARTeleporter : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject arPromptUI; // Panel with "Enter AR" button
    [SerializeField] private Button enterARButton;
    
    [Header("AR Manager")]
    [SerializeField] private ARManager arManager;
    
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
            
            Debug.Log("Player left AR teleporter zone");
        }
    }
    
    void OnEnterARButtonPressed()
    {
        if (playerInRange && playerController != null && arManager != null)
        {
            // Hide the prompt
            if (arPromptUI != null)
                arPromptUI.SetActive(false);
            
            // Enter AR mode
            arManager.EnterARMode(playerController);
            
            Debug.Log("Entering AR Mode!");
        }
    }
}