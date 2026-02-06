using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Rendering.Universal;

public class ARBackgroundFix : MonoBehaviour
{
    private ARCameraManager cameraManager;
    private ARCameraBackground cameraBackground;
    private Camera arCamera;

    void Awake()
    {
        arCamera = GetComponent<Camera>();
        cameraManager = GetComponent<ARCameraManager>();
        cameraBackground = GetComponent<ARCameraBackground>();
        
        Debug.Log("[ARBackgroundFix] Awake - Checking components");
        
        if (arCamera == null)
        {
            Debug.LogError("[ARBackgroundFix] No Camera component found!");
            return;
        }
        
        arCamera.clearFlags = CameraClearFlags.SolidColor;
        arCamera.backgroundColor = Color.black;
        
        if (cameraManager == null)
        {
            Debug.LogError("[ARBackgroundFix] No ARCameraManager found! Adding one...");
            cameraManager = gameObject.AddComponent<ARCameraManager>();
        }
        
        if (cameraBackground == null)
        {
            Debug.LogError("[ARBackgroundFix] No ARCameraBackground found! Adding one...");
            cameraBackground = gameObject.AddComponent<ARCameraBackground>();
        }
        
        cameraBackground.useCustomMaterial = false;
        
        Debug.Log("[ARBackgroundFix] Setup complete");
    }

    void Start()
    {
        if (cameraManager != null)
        {
            cameraManager.frameReceived += OnCameraFrameReceived;
        }
        
        var urpCameraData = arCamera.GetUniversalAdditionalCameraData();
        if (urpCameraData != null)
        {
            urpCameraData.renderType = CameraRenderType.Base;
            urpCameraData.requiresColorTexture = false;
            urpCameraData.requiresDepthTexture = false;
            Debug.Log("[ARBackgroundFix] URP camera configured");
        }
    }

    void OnCameraFrameReceived(ARCameraFrameEventArgs args)
    {
        Debug.Log("[ARBackgroundFix] Camera frame received! Textures: " + args.textures.Count);
    }

    void OnDestroy()
    {
        if (cameraManager != null)
        {
            cameraManager.frameReceived -= OnCameraFrameReceived;
        }
    }
}
