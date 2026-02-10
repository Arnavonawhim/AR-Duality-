using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem.EnhancedTouch;
using System.Collections.Generic;
using Convai.Scripts.Runtime.Core;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

/// <summary>
/// Manages Convai character placement and interaction in AR mode.
/// Uses New Input System for touch handling.
/// </summary>
public class ConvaiARCharacter : MonoBehaviour
{
    [Header("AR References (Drag from XR Origin)")]
    [SerializeField] private ARRaycastManager raycastManager;
    [SerializeField] private ARPlaneManager planeManager;
    
    [Header("Character Prefabs (Per World)")]
    [SerializeField] private GameObject sciFiCharacterPrefab;
    [SerializeField] private GameObject earthCharacterPrefab;
    [SerializeField] private GameObject libraryCharacterPrefab;
    
    [Header("UI References")]
    [SerializeField] private GameObject placementUI;
    [SerializeField] private GameObject learningUI;
    [SerializeField] private GameObject quizUI;
    [SerializeField] private GameObject talkButton;
    
    [Header("Session Controllers")]
    [SerializeField] private LearningSessionController learningController;
    [SerializeField] private QuizSessionController quizController;
    
    [Header("Debug")]
    [SerializeField] private bool debugMode = true;
    
    private GameObject characterInstance;
    private ConvaiNPC convaiNPC;
    private bool characterPlaced;
    private Camera arCamera;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();
    
    void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }
    
    void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }
    
    void Start()
    {
        Debug.Log("[ConvaiAR] Start called");
        arCamera = Camera.main;
        
        // Auto-find AR references if not assigned
        if (raycastManager == null)
        {
            raycastManager = FindObjectOfType<ARRaycastManager>();
            if (raycastManager != null)
                Debug.Log("[ConvaiAR] Found ARRaycastManager automatically");
            else
                Debug.LogError("[ConvaiAR] ARRaycastManager not found!");
        }
        
        if (planeManager == null)
        {
            planeManager = FindObjectOfType<ARPlaneManager>();
            if (planeManager != null)
                Debug.Log("[ConvaiAR] Found ARPlaneManager automatically");
            else
                Debug.LogError("[ConvaiAR] ARPlaneManager not found!");
        }
        
        // Check camera
        if (arCamera == null)
            Debug.LogError("[ConvaiAR] No main camera found!");
        else
            Debug.Log($"[ConvaiAR] Main camera: {arCamera.name}");
        
        // Hide session UIs at start
        SetUIState(true, false, false, false);
        
        Debug.Log("[ConvaiAR] Ready for character placement");
    }
    
    void SetUIState(bool placement, bool learning, bool quiz, bool talk)
    {
        if (placementUI) placementUI.SetActive(placement);
        if (learningUI) learningUI.SetActive(learning);
        if (quizUI) quizUI.SetActive(quiz);
        if (talkButton) talkButton.SetActive(talk);
    }
    
    void Update()
    {
        if (characterPlaced) 
        {
            // Make character face the camera
            if (characterInstance != null && arCamera != null)
            {
                Vector3 lookPos = arCamera.transform.position;
                lookPos.y = characterInstance.transform.position.y;
                characterInstance.transform.LookAt(lookPos);
            }
            return;
        }
        
        // Handle touch input using New Input System
        if (raycastManager != null && Touch.activeTouches.Count > 0)
        {
            Touch touch = Touch.activeTouches[0];
            
            if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began)
            {
                if (debugMode) Debug.Log($"[ConvaiAR] Touch detected at {touch.screenPosition}");
                
                hits.Clear();
                if (raycastManager.Raycast(touch.screenPosition, hits, TrackableType.PlaneWithinPolygon))
                {
                    if (debugMode) Debug.Log($"[ConvaiAR] Plane hit at {hits[0].pose.position}");
                    PlaceCharacter(hits[0].pose.position);
                }
                else
                {
                    if (debugMode) Debug.Log("[ConvaiAR] No plane hit - point camera at a flat surface");
                }
            }
        }
    }
    
    void PlaceCharacter(Vector3 position)
    {
        Debug.Log($"[ConvaiAR] Placing character at {position}");
        
        GameObject prefab = GetCharacterPrefabForWorld();
        if (prefab == null)
        {
            Debug.LogError("[ConvaiAR] No character prefab! Cannot spawn.");
            return;
        }
        
        characterInstance = Instantiate(prefab, position, Quaternion.identity);
        characterPlaced = true;
        Debug.Log($"[ConvaiAR] Character spawned: {characterInstance.name}");
        
        convaiNPC = characterInstance.GetComponent<ConvaiNPC>();
        if (convaiNPC == null)
            Debug.LogError("[ConvaiAR] Character prefab missing ConvaiNPC component!");
        else
            Debug.Log($"[ConvaiAR] ConvaiNPC found with ID: {convaiNPC.characterID}");
        
        if (placementUI) placementUI.SetActive(false);
        
        if (planeManager != null)
        {
            planeManager.enabled = false;
            foreach (var plane in planeManager.trackables)
                plane.gameObject.SetActive(false);
        }
        
        StartSession();
    }
    
    GameObject GetCharacterPrefabForWorld()
    {
        if (WorldManager.Instance?.CurrentWorldData?.characterPrefab != null)
            return WorldManager.Instance.CurrentWorldData.characterPrefab;
        
        WorldType world = WorldManager.Instance?.CurrentWorld ?? WorldType.SciFi;
        
        return world switch
        {
            WorldType.SciFi => sciFiCharacterPrefab,
            WorldType.Earth => earthCharacterPrefab,
            WorldType.Library => libraryCharacterPrefab,
            _ => sciFiCharacterPrefab
        };
    }
    
    void StartSession()
    {
        TeleporterType teleporterType = WorldManager.Instance?.CurrentTeleporterType ?? TeleporterType.Learning;
        Debug.Log($"[ConvaiAR] Starting {teleporterType} session");
        
        if (teleporterType == TeleporterType.Learning)
            StartLearningMode();
        else
            StartQuizMode();
    }
    
    void StartLearningMode()
    {
        Debug.Log("[ConvaiAR] Starting Learning Mode");
        SetUIState(false, true, false, true);
        
        if (learningController != null)
        {
            learningController.SetConvaiNPC(convaiNPC);
            learningController.gameObject.SetActive(true);
            learningController.StartLearningSession();
        }
        else
            Debug.LogError("[ConvaiAR] LearningSessionController not assigned!");
    }
    
    void StartQuizMode()
    {
        Debug.Log("[ConvaiAR] Starting Quiz Mode");
        SetUIState(false, false, true, false);
        
        if (quizController != null)
        {
            var worldData = WorldManager.Instance?.CurrentWorldData;
            if (worldData?.questionDatabase != null)
                quizController.SetQuestionDatabase(worldData.questionDatabase);
            
            quizController.gameObject.SetActive(true);
            quizController.StartQuizSession();
        }
        else
            Debug.LogError("[ConvaiAR] QuizSessionController not assigned!");
    }
    
    public void OnTalkButtonDown()
    {
        if (convaiNPC != null)
        {
            convaiNPC.StartListening();
            Debug.Log("[ConvaiAR] Started listening");
        }
    }
    
    public void OnTalkButtonUp()
    {
        if (convaiNPC != null)
        {
            convaiNPC.StopListening();
            Debug.Log("[ConvaiAR] Stopped listening");
        }
    }
    
    public void SendMessage(string message)
    {
        if (convaiNPC != null)
            convaiNPC.SendTextDataAsync(message);
    }
    
    public ConvaiNPC GetConvaiNPC() => convaiNPC;
    public bool IsCharacterPlaced() => characterPlaced;
    
    public void ExitAR()
    {
        string sceneName = WorldManager.Instance?.CurrentWorld.ToString() ?? "SciFi";
        SceneManager.LoadScene(sceneName);
    }
}
