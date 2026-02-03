using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

public class ARManager : MonoBehaviour
{
    [Header("AR Components")]
    [SerializeField] private ARSession arSession;
    [SerializeField] private ARSessionOrigin arSessionOrigin;
    [SerializeField] private ARPlaneManager arPlaneManager;
    [SerializeField] private ARRaycastManager arRaycastManager;
    
    [Header("AR Camera")]
    [SerializeField] private Camera arCamera;
    
    [Header("Virtual Camera")]
    [SerializeField] private Camera virtualCamera; // Your normal game camera
    
    [Header("UI")]
    [SerializeField] private GameObject arUI; // UI for AR mode (instructions, exit button, etc)
    [SerializeField] private Button exitARButton;
    [SerializeField] private Text instructionText;
    [SerializeField] private Slider scaleSlider; // To scale the player in AR
    
    [Header("AR Settings")]
    [SerializeField] private float defaultARScale = 0.1f; // Scale of player in AR (smaller for table-top)
    [SerializeField] private float minScale = 0.05f;
    [SerializeField] private float maxScale = 0.5f;
    
    // Private variables
    private bool isInARMode = false;
    private CharacterController2D currentPlayer;
    private GameObject arPlayerInstance;
    
    private Vector3 virtualStartPosition;
    private Vector3 arStartPosition;
    private Vector3 virtualStartScale;
    
    private bool isPlayerPlaced = false;
    private List<ARRaycastHit> raycastHits = new List<ARRaycastHit>();
    
    void Start()
    {
        // Disable AR components at start
        if (arSession != null) arSession.enabled = false;
        if (arSessionOrigin != null) arSessionOrigin.gameObject.SetActive(false);
        if (arCamera != null) arCamera.gameObject.SetActive(false);
        if (arUI != null) arUI.SetActive(false);
        
        // Setup exit button
        if (exitARButton != null)
            exitARButton.onClick.AddListener(ExitARMode);
        
        // Setup scale slider
        if (scaleSlider != null)
        {
            scaleSlider.minValue = minScale;
            scaleSlider.maxValue = maxScale;
            scaleSlider.value = defaultARScale;
            scaleSlider.onValueChanged.AddListener(OnScaleChanged);
        }
    }
    
    void Update()
    {
        if (!isInARMode) return;
        
        // If player not placed yet, show plane detection instructions
        if (!isPlayerPlaced)
        {
            DetectPlaneAndPlacePlayer();
        }
    }
    
    public void EnterARMode(CharacterController2D player)
    {
        if (isInARMode) return;
        
        currentPlayer = player;
        isInARMode = true;
        isPlayerPlaced = false;
        
        // Save virtual world state
        virtualStartPosition = player.transform.position;
        virtualStartScale = player.transform.localScale;
        
        // Notify player controller
        player.EnterARMode();
        
        // Hide player in virtual world
        player.gameObject.SetActive(false);
        
        // Enable AR
        if (arSession != null) arSession.enabled = true;
        if (arSessionOrigin != null) arSessionOrigin.gameObject.SetActive(true);
        if (arCamera != null) arCamera.gameObject.SetActive(true);
        if (arPlaneManager != null) arPlaneManager.enabled = true;
        
        // Switch cameras
        if (virtualCamera != null) virtualCamera.gameObject.SetActive(false);
        if (arCamera != null) arCamera.gameObject.SetActive(true);
        
        // Show AR UI
        if (arUI != null) arUI.SetActive(true);
        UpdateInstructionText("Point camera at a flat surface to place player");
        
        Debug.Log("AR Mode Activated - Detecting planes...");
    }
    
    void DetectPlaneAndPlacePlayer()
    {
        // Raycast from center of screen
        if (arRaycastManager != null && arRaycastManager.Raycast(new Vector2(Screen.width / 2, Screen.height / 2), raycastHits, TrackableType.PlaneWithinPolygon))
        {
            // Get the first hit
            Pose hitPose = raycastHits[0].pose;
            
            // Check for touch/tap to place
            bool shouldPlace = false;
            
            #if UNITY_EDITOR
            // For testing in editor
            if (Input.GetMouseButtonDown(0))
                shouldPlace = true;
            #else
            // For mobile
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
                shouldPlace = true;
            #endif
            
            if (shouldPlace)
            {
                PlacePlayerInAR(hitPose.position, hitPose.rotation);
            }
            else
            {
                UpdateInstructionText("Tap to place player here");
            }
        }
        else
        {
            UpdateInstructionText("Point camera at a flat surface");
        }
    }
    
    void PlacePlayerInAR(Vector3 position, Quaternion rotation)
    {
        if (currentPlayer == null) return;
        
        // Create AR instance of player
        arPlayerInstance = Instantiate(currentPlayer.gameObject, position, rotation);
        arPlayerInstance.SetActive(true);
        
        // Scale down for AR
        arPlayerInstance.transform.localScale = virtualStartScale * defaultARScale;
        
        // Disable the character controller script on AR instance (we'll move it manually)
        var arPlayerController = arPlayerInstance.GetComponent<CharacterController2D>();
        if (arPlayerController != null)
            arPlayerController.enabled = false;
        
        // Enable rigidbody but make it kinematic
        var arRb = arPlayerInstance.GetComponent<Rigidbody>();
        if (arRb != null)
            arRb.isKinematic = true;
        
        // Save AR start position
        arStartPosition = position;
        isPlayerPlaced = true;
        
        // Disable plane detection
        if (arPlaneManager != null)
        {
            arPlaneManager.enabled = false;
            // Hide all detected planes
            foreach (var plane in arPlaneManager.trackables)
            {
                plane.gameObject.SetActive(false);
            }
        }
        
        UpdateInstructionText("Move around! Your movement will transfer to the game");
        Debug.Log("Player placed in AR at: " + position);
    }
    
    void OnScaleChanged(float newScale)
    {
        if (arPlayerInstance != null)
        {
            arPlayerInstance.transform.localScale = virtualStartScale * newScale;
        }
    }
    
    public void ExitARMode()
    {
        if (!isInARMode || currentPlayer == null) return;
        
        // Calculate movement delta in AR
        Vector3 arMovementDelta = Vector3.zero;
        float scaleFactor = 1f;
        
        if (arPlayerInstance != null)
        {
            // Calculate how much the player moved in AR (X and Z)
            arMovementDelta = arPlayerInstance.transform.position - arStartPosition;
            
            // Get scale ratio
            scaleFactor = arPlayerInstance.transform.localScale.magnitude / virtualStartScale.magnitude;
            
            // Destroy AR instance
            Destroy(arPlayerInstance);
        }
        
        // Convert AR movement to virtual world movement
        // AR X and Z map to virtual Z (forward/backward in 2.5D)
        float virtualZMovement = arMovementDelta.z / defaultARScale;
        
        // Apply movement to virtual player
        Vector3 newVirtualPosition = virtualStartPosition + new Vector3(0, 0, virtualZMovement);
        currentPlayer.transform.position = newVirtualPosition;
        
        // Apply scale changes if any
        currentPlayer.transform.localScale = virtualStartScale * scaleFactor;
        
        // Show player in virtual world
        currentPlayer.gameObject.SetActive(true);
        
        // Notify player controller
        currentPlayer.ExitARMode(arMovementDelta, currentPlayer.transform.localScale);
        
        // Disable AR
        if (arSession != null) arSession.enabled = false;
        if (arSessionOrigin != null) arSessionOrigin.gameObject.SetActive(false);
        if (arCamera != null) arCamera.gameObject.SetActive(false);
        if (arPlaneManager != null) arPlaneManager.enabled = false;
        
        // Switch cameras back
        if (virtualCamera != null) virtualCamera.gameObject.SetActive(true);
        if (arCamera != null) arCamera.gameObject.SetActive(false);
        
        // Hide AR UI
        if (arUI != null) arUI.SetActive(false);
        
        isInARMode = false;
        isPlayerPlaced = false;
        currentPlayer = null;
        
        Debug.Log($"Exited AR Mode - Player moved {virtualZMovement} units in virtual world");
    }
    
    void UpdateInstructionText(string message)
    {
        if (instructionText != null)
            instructionText.text = message;
    }
}