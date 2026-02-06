using UnityEngine;
using UnityEngine.XR.ARFoundation;
using TMPro;

public class ARDebugger : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI debugText;
    [SerializeField] private ARSession arSession;
    
    private ARCameraManager cameraManager;
    private ARPlaneManager planeManager;
    private float updateInterval = 0.5f;
    private float timer;

    void Start()
    {
        cameraManager = FindFirstObjectByType<ARCameraManager>();
        planeManager = FindFirstObjectByType<ARPlaneManager>();
        
        if (arSession == null) arSession = FindFirstObjectByType<ARSession>();
        
        Log("ARDebugger Started");
        Log("Unity Version: " + Application.unityVersion);
        Log("Platform: " + Application.platform);
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= updateInterval)
        {
            timer = 0;
            UpdateDebugInfo();
        }
    }

    void UpdateDebugInfo()
    {
        if (debugText == null) return;

        string info = "<b>=== AR DEBUG ===</b>\n";
        info += $"Time: {Time.time:F1}s\n\n";

        // AR Session State
        info += "<b>[AR Session]</b>\n";
        if (arSession != null)
        {
            info += $"State: {ARSession.state}\n";
            info += $"Enabled: {arSession.enabled}\n";
            info += $"NotTrackingReason: {ARSession.notTrackingReason}\n";
        }
        else
        {
            info += "<color=red>AR Session NOT FOUND!</color>\n";
        }

        // Camera Manager
        info += "\n<b>[AR Camera]</b>\n";
        if (cameraManager != null)
        {
            info += $"CameraManager: Found\n";
            info += $"Enabled: {cameraManager.enabled}\n";
            
            if (cameraManager.TryGetIntrinsics(out var intrinsics))
            {
                info += $"Camera Working: YES\n";
                info += $"Resolution: {intrinsics.resolution}\n";
            }
            else
            {
                info += "<color=yellow>Camera Intrinsics: Not Available</color>\n";
            }
        }
        else
        {
            info += "<color=red>AR Camera Manager NOT FOUND!</color>\n";
        }

        // Plane Detection
        info += "\n<b>[Planes]</b>\n";
        if (planeManager != null)
        {
            info += $"PlaneManager: Found\n";
            info += $"Enabled: {planeManager.enabled}\n";
            int planeCount = 0;
            foreach (var plane in planeManager.trackables)
            {
                planeCount++;
            }
            info += $"Planes Detected: {planeCount}\n";
        }
        else
        {
            info += "<color=red>AR Plane Manager NOT FOUND!</color>\n";
        }

        // Permission Status
        info += "\n<b>[Permissions]</b>\n";
        #if UNITY_ANDROID
        info += "Platform: Android\n";
        if (UnityEngine.Android.Permission.HasUserAuthorizedPermission(UnityEngine.Android.Permission.Camera))
        {
            info += "Camera Permission: <color=green>GRANTED</color>\n";
        }
        else
        {
            info += "Camera Permission: <color=red>NOT GRANTED</color>\n";
            info += "Requesting...\n";
            UnityEngine.Android.Permission.RequestUserPermission(UnityEngine.Android.Permission.Camera);
        }
        #elif UNITY_IOS
        info += "Platform: iOS\n";
        info += "Check Settings > Privacy > Camera\n";
        #else
        info += "Platform: Editor/Other (No actual AR)\n";
        #endif

        debugText.text = info;
    }

    void Log(string msg)
    {
        Debug.Log("[ARDebug] " + msg);
    }
}
