using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;
using Convai.Scripts.Runtime.Core;

/// <summary>
/// Manages Convai character placement and interaction in AR mode.
/// Replaces the robot - the character IS the placed AR object.
/// </summary>
[RequireComponent(typeof(ARRaycastManager), typeof(ARPlaneManager))]
public class ConvaiARCharacter : MonoBehaviour
{
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
    
    private ARRaycastManager raycastManager;
    private ARPlaneManager planeManager;
    private GameObject characterInstance;
    private ConvaiNPC convaiNPC;
    private bool characterPlaced;
    private Camera arCamera;
    
    void Awake()
    {
        raycastManager = GetComponent<ARRaycastManager>();
        planeManager = GetComponent<ARPlaneManager>();
        arCamera = Camera.main;
    }
    
    void Start()
    {
        // Hide session UIs at start
        if (learningUI) learningUI.SetActive(false);
        if (quizUI) quizUI.SetActive(false);
        if (talkButton) talkButton.SetActive(false);
        if (placementUI) placementUI.SetActive(true);
    }
    
    void Update()
    {
        if (!characterPlaced && Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                List<ARRaycastHit> hits = new List<ARRaycastHit>();
                if (raycastManager.Raycast(touch.position, hits, TrackableType.PlaneWithinPolygon))
                {
                    PlaceCharacter(hits[0].pose.position);
                }
            }
        }
        
        // Make character face the camera
        if (characterPlaced && characterInstance != null && arCamera != null)
        {
            Vector3 lookPos = arCamera.transform.position;
            lookPos.y = characterInstance.transform.position.y;
            characterInstance.transform.LookAt(lookPos);
        }
    }
    
    void PlaceCharacter(Vector3 position)
    {
        // Get world-specific prefab
        GameObject prefab = GetCharacterPrefabForWorld();
        if (prefab == null)
        {
            Debug.LogError("No character prefab found for current world!");
            return;
        }
        
        // Spawn character at position
        characterInstance = Instantiate(prefab, position, Quaternion.identity);
        characterPlaced = true;
        
        // Get ConvaiNPC component
        convaiNPC = characterInstance.GetComponent<ConvaiNPC>();
        
        // Hide placement UI, disable plane detection
        if (placementUI) placementUI.SetActive(false);
        planeManager.enabled = false;
        foreach (var plane in planeManager.trackables)
        {
            plane.gameObject.SetActive(false);
        }
        
        // Start session based on teleporter type
        StartSession();
    }
    
    GameObject GetCharacterPrefabForWorld()
    {
        // First try to get from WorldData
        if (WorldManager.Instance?.CurrentWorldData?.characterPrefab != null)
        {
            return WorldManager.Instance.CurrentWorldData.characterPrefab;
        }
        
        // Fallback to per-world prefabs
        return WorldManager.Instance?.CurrentWorld switch
        {
            WorldType.SciFi => sciFiCharacterPrefab,
            WorldType.Earth => earthCharacterPrefab,
            WorldType.Library => libraryCharacterPrefab,
            _ => sciFiCharacterPrefab
        };
    }
    
    void StartSession()
    {
        if (WorldManager.Instance == null) return;
        
        var teleporterType = WorldManager.Instance.CurrentTeleporterType;
        
        if (teleporterType == TeleporterType.Learning)
        {
            StartLearningMode();
        }
        else
        {
            StartQuizMode();
        }
    }
    
    void StartLearningMode()
    {
        if (learningUI) learningUI.SetActive(true);
        if (quizUI) quizUI.SetActive(false);
        
        // Enable talk button for voice interaction
        if (talkButton) talkButton.SetActive(true);
        
        // Start the learning session
        if (learningController)
        {
            learningController.SetConvaiNPC(convaiNPC);
            learningController.gameObject.SetActive(true);
            learningController.StartLearningSession();
        }
    }
    
    void StartQuizMode()
    {
        if (quizUI) quizUI.SetActive(true);
        if (learningUI) learningUI.SetActive(false);
        if (talkButton) talkButton.SetActive(false);
        
        // Start the quiz session
        if (quizController)
        {
            // Get the correct question database for current world
            var worldData = WorldManager.Instance?.CurrentWorldData;
            if (worldData?.questionDatabase != null)
            {
                quizController.SetQuestionDatabase(worldData.questionDatabase);
            }
            
            quizController.gameObject.SetActive(true);
            quizController.StartQuizSession();
        }
    }
    
    // Called by Talk button (hold)
    public void OnTalkButtonDown()
    {
        if (convaiNPC != null)
        {
            convaiNPC.StartListening();
            Debug.Log("[ConvaiAR] Started listening");
        }
    }
    
    // Called by Talk button (release)
    public void OnTalkButtonUp()
    {
        if (convaiNPC != null)
        {
            convaiNPC.StopListening();
            Debug.Log("[ConvaiAR] Stopped listening");
        }
    }
    
    // Send text message to Convai character
    public void SendMessage(string message)
    {
        if (convaiNPC != null)
        {
            convaiNPC.SendTextDataAsync(message);
            Debug.Log($"[ConvaiAR] Sent: {message}");
        }
    }
    
    public ConvaiNPC GetConvaiNPC() => convaiNPC;
    public bool IsCharacterPlaced() => characterPlaced;
}
