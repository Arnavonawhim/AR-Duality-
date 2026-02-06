using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using System.Collections.Generic;
using TMPro;

public class ARSessionController : MonoBehaviour
{
    [Header("AR Components")]
    [SerializeField] private ARSession arSession;
    [SerializeField] private ARPlaneManager planeManager;
    [SerializeField] private ARRaycastManager raycastManager;

    [Header("Robot")]
    [SerializeField] private GameObject robotPrefab;
    [SerializeField] private float robotScale = 0.15f;

    [Header("UI References")]
    [SerializeField] private GameObject placementUI;
    [SerializeField] private GameObject gameplayUI;
    [SerializeField] private TextMeshProUGUI instructionText;
    [SerializeField] private TextMeshProUGUI debugText;

    [Header("Plane Visualization")]
    [SerializeField] private Material planeVisualizationMaterial;

    private GameObject robotInstance;
    private bool robotPlaced = false;
    private List<ARRaycastHit> raycastHits = new List<ARRaycastHit>();
    private ARRobotController robotController;
    private string debugLog = "";
    private int frameCount = 0;

    void Awake()
    {
        Log("=== ARSessionController Awake ===");
        Log("Platform: " + Application.platform);
        Log("Unity: " + Application.unityVersion);
        
        #if UNITY_ANDROID
        Log("Requesting Camera Permission...");
        if (!UnityEngine.Android.Permission.HasUserAuthorizedPermission(UnityEngine.Android.Permission.Camera))
        {
            UnityEngine.Android.Permission.RequestUserPermission(UnityEngine.Android.Permission.Camera);
        }
        else
        {
            Log("Camera Permission: GRANTED");
        }
        #endif
    }

    void Start()
    {
        Log("=== ARSessionController Start ===");
        
        EnhancedTouchSupport.Enable();
        Log("EnhancedTouch: Enabled");
        
        if (arSession == null)
        {
            arSession = FindFirstObjectByType<ARSession>();
            Log("ARSession found: " + (arSession != null));
        }
        
        if (planeManager == null)
        {
            planeManager = FindFirstObjectByType<ARPlaneManager>();
            Log("PlaneManager found: " + (planeManager != null));
        }
        
        if (raycastManager == null)
        {
            raycastManager = FindFirstObjectByType<ARRaycastManager>();
            Log("RaycastManager found: " + (raycastManager != null));
        }
        
        if (placementUI != null) placementUI.SetActive(true);
        if (gameplayUI != null) gameplayUI.SetActive(false);
        UpdateInstruction("Move your phone to detect surfaces");
        
        if (planeManager != null)
        {
            planeManager.planesChanged += OnPlanesChanged;
            Log("PlaneManager.planesChanged subscribed");
        }
        
        if (arSession != null)
        {
            ARSession.stateChanged += OnARSessionStateChanged;
            Log("ARSession.stateChanged subscribed");
            Log("Initial AR State: " + ARSession.state);
        }
        
        Log("Start Complete");
    }

    void OnARSessionStateChanged(ARSessionStateChangedEventArgs args)
    {
        Log("AR STATE CHANGED: " + args.state);
        
        if (args.state == ARSessionState.Unsupported)
        {
            Log("ERROR: AR is NOT SUPPORTED on this device!");
            UpdateInstruction("AR not supported on this device");
        }
        else if (args.state == ARSessionState.NeedsInstall)
        {
            Log("AR needs install (ARCore/ARKit)");
            UpdateInstruction("Please install AR services");
        }
        else if (args.state == ARSessionState.Ready)
        {
            Log("AR is READY");
        }
        else if (args.state == ARSessionState.SessionInitializing)
        {
            Log("AR Session Initializing...");
        }
        else if (args.state == ARSessionState.SessionTracking)
        {
            Log("AR Session TRACKING - Camera should be working!");
            UpdateInstruction("Move phone slowly to detect surfaces");
        }
    }

    void OnDestroy()
    {
        EnhancedTouchSupport.Disable();
        ARSession.stateChanged -= OnARSessionStateChanged;
        
        if (planeManager != null)
        {
            planeManager.planesChanged -= OnPlanesChanged;
        }
    }

    void OnPlanesChanged(ARPlanesChangedEventArgs args)
    {
        if (args.added.Count > 0)
        {
            Log("PLANES DETECTED: " + args.added.Count + " new planes!");
        }
        
        if (!robotPlaced && args.added.Count > 0)
        {
            UpdateInstruction("Tap on a surface to place your robot");
        }
    }

    void Update()
    {
        frameCount++;
        
        if (frameCount % 60 == 0)
        {
            UpdateDebugDisplay();
        }
        
        if (robotPlaced) return;

        bool touchDetected = false;
        Vector2 touchPosition = Vector2.zero;

#if UNITY_EDITOR
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            touchDetected = true;
            touchPosition = Mouse.current.position.ReadValue();
            Log("Editor Click at: " + touchPosition);
        }
#else
        if (UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches.Count > 0)
        {
            var touch = UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches[0];
            if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began)
            {
                touchDetected = true;
                touchPosition = touch.screenPosition;
                Log("Touch at: " + touchPosition);
            }
        }
#endif

        if (touchDetected)
        {
            Log("Attempting raycast...");
            if (raycastManager != null && raycastManager.Raycast(touchPosition, raycastHits, TrackableType.PlaneWithinPolygon))
            {
                Log("Raycast HIT! Placing robot...");
                Pose hitPose = raycastHits[0].pose;
                PlaceRobot(hitPose.position, hitPose.rotation);
            }
            else
            {
                Log("Raycast MISS - No plane at touch position");
            }
        }
    }

    void UpdateDebugDisplay()
    {
        if (debugText == null) return;
        
        string info = "<b>AR DEBUG</b>\n";
        info += "State: " + ARSession.state + "\n";
        info += "NotTracking: " + ARSession.notTrackingReason + "\n";
        info += "RobotPlaced: " + robotPlaced + "\n";
        
        if (planeManager != null)
        {
            int count = 0;
            foreach (var p in planeManager.trackables) count++;
            info += "Planes: " + count + "\n";
        }
        
        info += "\n<size=10>" + debugLog + "</size>";
        debugText.text = info;
    }

    void PlaceRobot(Vector3 position, Quaternion rotation)
    {
        if (robotPlaced)
        {
            Log("Robot already placed, ignoring");
            return;
        }
        
        if (robotPrefab == null)
        {
            Log("ERROR: Robot prefab not assigned!");
            return;
        }

        robotPlaced = true;
        
        Log("Instantiating robot at " + position);
        robotInstance = Instantiate(robotPrefab, position, Quaternion.Euler(0, rotation.eulerAngles.y, 0));
        robotInstance.transform.localScale = Vector3.one * robotScale;

        robotController = robotInstance.GetComponent<ARRobotController>();
        if (robotController == null)
        {
            robotController = robotInstance.AddComponent<ARRobotController>();
        }
        robotController.Initialize(position);

        Log("Robot PLACED successfully!");

        DisablePlaneVisualization();

        if (placementUI != null) placementUI.SetActive(false);
        if (gameplayUI != null) gameplayUI.SetActive(true);

        UpdateInstruction("");
    }

    void DisablePlaneVisualization()
    {
        if (planeManager != null)
        {
            foreach (var plane in planeManager.trackables)
            {
                plane.gameObject.SetActive(false);
            }
            planeManager.planePrefab = null;
        }
    }

    void UpdateInstruction(string message)
    {
        if (instructionText != null)
        {
            instructionText.text = message;
        }
    }
    
    void Log(string msg)
    {
        Debug.Log("[AR] " + msg);
        debugLog = msg + "\n" + debugLog;
        if (debugLog.Length > 500) debugLog = debugLog.Substring(0, 500);
    }

    public ARRobotController GetRobotController()
    {
        return robotController;
    }

    public bool IsRobotPlaced()
    {
        return robotPlaced;
    }
}

