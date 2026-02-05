using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Unity.XR.CoreUtils;
using TMPro;
using System.Collections.Generic;

/*public class ARManager : MonoBehaviour
{
    [Header("AR Components")]
    [SerializeField] private ARSession arSession;
    [SerializeField] private XROrigin xrOrigin;
    [SerializeField] private ARPlaneManager arPlaneManager;
    [SerializeField] private ARRaycastManager arRaycastManager;
    [SerializeField] private Camera arCamera;
    
    [Header("Virtual Camera")]
    [SerializeField] private Camera virtualCamera;
    
    [Header("UI")]
    [SerializeField] private GameObject arUI;
    [SerializeField] private Button exitARButton;
    [SerializeField] private TMP_Text instructionText;
    [SerializeField] private Slider scaleSlider;
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;
    [SerializeField] private Button forwardButton;
    [SerializeField] private Button backButton;
    
    [Header("AR Settings")]
    [SerializeField] private float arMoveSpeed = 0.5f;
    [SerializeField] private float maxARDistance = 3f;
    [SerializeField] private float minScale = 0.5f;
    [SerializeField] private float maxScale = 2.5f;
    [SerializeField] private float defaultScale = 1f;
    
    [Header("Enemy Settings")]
    [SerializeField] private GameObject photonEnemyPrefab;
    [SerializeField] private GameObject antiMatterEnemyPrefab;
    [SerializeField] private int minEnemies = 2;
    [SerializeField] private int maxEnemies = 5;
    [SerializeField] private float enemySpawnRadius = 2f;
    
    private bool isInARMode = false;
    private CharacterController2D currentPlayer;
    private GameObject arPlayerInstance;
    private Vector3 virtualStartPosition;
    private Vector3 arStartPosition;
    private float currentScale = 1f;
    private bool isPlayerPlaced = false;
    private List<ARRaycastHit> raycastHits = new List<ARRaycastHit>();
    private Vector2 moveInput = Vector2.zero;
    private List<GameObject> spawnedEnemies = new List<GameObject>();
    
    void Start()
    {
        if (arSession != null) arSession.enabled = false;
        if (xrOrigin != null) xrOrigin.gameObject.SetActive(false);
        if (arCamera != null) arCamera.gameObject.SetActive(false);
        if (arUI != null) arUI.SetActive(false);
        
        if (exitARButton != null) exitARButton.onClick.AddListener(ExitARMode);
        
        if (scaleSlider != null)
        {
            scaleSlider.minValue = minScale;
            scaleSlider.maxValue = maxScale;
            scaleSlider.value = defaultScale;
            scaleSlider.onValueChanged.AddListener(OnScaleChanged);
        }
        
        SetupMoveButtons();
    }
    
    void SetupMoveButtons()
    {
        if (leftButton != null)
        {
            var leftTrigger = leftButton.gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();
            var pdLeft = new UnityEngine.EventSystems.EventTrigger.Entry { eventID = UnityEngine.EventSystems.EventTriggerType.PointerDown };
            pdLeft.callback.AddListener((data) => { moveInput.x = -1f; });
            leftTrigger.triggers.Add(pdLeft);
            var puLeft = new UnityEngine.EventSystems.EventTrigger.Entry { eventID = UnityEngine.EventSystems.EventTriggerType.PointerUp };
            puLeft.callback.AddListener((data) => { moveInput.x = 0f; });
            leftTrigger.triggers.Add(puLeft);
        }
        
        if (rightButton != null)
        {
            var rightTrigger = rightButton.gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();
            var pdRight = new UnityEngine.EventSystems.EventTrigger.Entry { eventID = UnityEngine.EventSystems.EventTriggerType.PointerDown };
            pdRight.callback.AddListener((data) => { moveInput.x = 1f; });
            rightTrigger.triggers.Add(pdRight);
            var puRight = new UnityEngine.EventSystems.EventTrigger.Entry { eventID = UnityEngine.EventSystems.EventTriggerType.PointerUp };
            puRight.callback.AddListener((data) => { moveInput.x = 0f; });
            rightTrigger.triggers.Add(puRight);
        }
        
        if (forwardButton != null)
        {
            var fwdTrigger = forwardButton.gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();
            var pdFwd = new UnityEngine.EventSystems.EventTrigger.Entry { eventID = UnityEngine.EventSystems.EventTriggerType.PointerDown };
            pdFwd.callback.AddListener((data) => { moveInput.y = 1f; });
            fwdTrigger.triggers.Add(pdFwd);
            var puFwd = new UnityEngine.EventSystems.EventTrigger.Entry { eventID = UnityEngine.EventSystems.EventTriggerType.PointerUp };
            puFwd.callback.AddListener((data) => { moveInput.y = 0f; });
            fwdTrigger.triggers.Add(puFwd);
        }
        
        if (backButton != null)
        {
            var backTrigger = backButton.gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();
            var pdBack = new UnityEngine.EventSystems.EventTrigger.Entry { eventID = UnityEngine.EventSystems.EventTriggerType.PointerDown };
            pdBack.callback.AddListener((data) => { moveInput.y = -1f; });
            backTrigger.triggers.Add(pdBack);
            var puBack = new UnityEngine.EventSystems.EventTrigger.Entry { eventID = UnityEngine.EventSystems.EventTriggerType.PointerUp };
            puBack.callback.AddListener((data) => { moveInput.y = 0f; });
            backTrigger.triggers.Add(puBack);
        }
    }
    
    void Update()
    {
        if (!isInARMode) return;
        
        if (!isPlayerPlaced)
        {
            DetectPlaneAndPlacePlayer();
        }
        else
        {
            MoveARPlayer();
            CheckARDistance();
        }
    }
    
    public void EnterARMode(CharacterController2D player)
    {
        if (isInARMode) return;
        
        currentPlayer = player;
        isInARMode = true;
        isPlayerPlaced = false;
        currentScale = defaultScale;
        
        virtualStartPosition = player.transform.position;
        player.EnterARMode();
        player.gameObject.SetActive(false);
        
        if (arSession != null) arSession.enabled = true;
        if (xrOrigin != null) xrOrigin.gameObject.SetActive(true);
        if (arCamera != null) arCamera.gameObject.SetActive(true);
        if (arPlaneManager != null) arPlaneManager.enabled = true;
        
        if (virtualCamera != null) virtualCamera.gameObject.SetActive(false);
        if (arUI != null) arUI.SetActive(true);
        
        UpdateInstructionText("Point at flat surface and tap to place");
    }
    
    void DetectPlaneAndPlacePlayer()
    {
        if (arRaycastManager != null && arRaycastManager.Raycast(new Vector2(Screen.width / 2, Screen.height / 2), raycastHits, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = raycastHits[0].pose;
            
            bool shouldPlace = false;
            #if UNITY_EDITOR
            if (Input.GetMouseButtonDown(0)) shouldPlace = true;
            #else
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) shouldPlace = true;
            #endif
            
            if (shouldPlace)
            {
                PlacePlayerInAR(hitPose.position);
            }
            else
            {
                UpdateInstructionText("Tap to place robot");
            }
        }
    }
    
    void PlacePlayerInAR(Vector3 position)
    {
        if (currentPlayer == null) return;
        
        arPlayerInstance = Instantiate(currentPlayer.gameObject, position, Quaternion.identity);
        arPlayerInstance.SetActive(true);
        arPlayerInstance.transform.localScale = Vector3.one * 0.1f;
        
        var controller = arPlayerInstance.GetComponent<CharacterController2D>();
        if (controller != null) controller.enabled = false;
        
        var rb = arPlayerInstance.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }
        
        arStartPosition = position;
        isPlayerPlaced = true;
        
        if (arPlaneManager != null)
        {
            arPlaneManager.enabled = false;
            foreach (var plane in arPlaneManager.trackables)
                plane.gameObject.SetActive(false);
        }
        
        SpawnEnemies();
        UpdateInstructionText("Move and scale the robot!");
    }
    
    void MoveARPlayer()
    {
        if (arPlayerInstance == null || moveInput.magnitude < 0.01f) return;
        
        Vector3 forward = arCamera.transform.forward;
        Vector3 right = arCamera.transform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();
        
        Vector3 movement = (right * moveInput.x + forward * moveInput.y) * arMoveSpeed * Time.deltaTime;
        arPlayerInstance.transform.position += movement;
    }
    
    void CheckARDistance()
    {
        if (arPlayerInstance == null) return;
        
        float distance = Vector3.Distance(arPlayerInstance.transform.position, arStartPosition);
        if (distance > maxARDistance)
        {
            KillPlayer();
        }
    }
    
    void SpawnEnemies()
    {
        if (photonEnemyPrefab == null || antiMatterEnemyPrefab == null) return;
        
        int enemyCount = Random.Range(minEnemies, maxEnemies + 1);
        
        for (int i = 0; i < enemyCount; i++)
        {
            Vector2 randomCircle = Random.insideUnitCircle * enemySpawnRadius;
            Vector3 spawnPos = arStartPosition + new Vector3(randomCircle.x, 0.5f, randomCircle.y);
            
            GameObject enemyPrefab = Random.value > 0.5f ? photonEnemyPrefab : antiMatterEnemyPrefab;
            GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
            enemy.transform.localScale = Vector3.one * 0.1f;
            spawnedEnemies.Add(enemy);
        }
    }
    
    void OnScaleChanged(float newScale)
    {
        currentScale = newScale;
        if (arPlayerInstance != null)
        {
            arPlayerInstance.transform.localScale = Vector3.one * 0.1f * newScale;
        }
    }
    
    void KillPlayer()
    {
        UpdateInstructionText("Too far! Returning...");
        Invoke(nameof(ExitARMode), 1f);
    }
    
    public void ExitARMode()
    {
        if (!isInARMode || currentPlayer == null) return;
        
        Vector3 arMovementDelta = Vector3.zero;
        if (arPlayerInstance != null)
        {
            arMovementDelta = arPlayerInstance.transform.position - arStartPosition;
            Destroy(arPlayerInstance);
        }
        
        foreach (var enemy in spawnedEnemies)
        {
            if (enemy != null) Destroy(enemy);
        }
        spawnedEnemies.Clear();
        
        float virtualXMovement = arMovementDelta.x * 10f;
        Vector3 newVirtualPosition = virtualStartPosition + new Vector3(virtualXMovement, 0, 0);
        currentPlayer.transform.position = newVirtualPosition;
        currentPlayer.transform.localScale = Vector3.one * currentScale;
        
        currentPlayer.gameObject.SetActive(true);
        currentPlayer.ExitARMode(arMovementDelta, currentPlayer.transform.localScale);
        
        if (arSession != null) arSession.enabled = false;
        if (xrOrigin != null) xrOrigin.gameObject.SetActive(false);
        if (arCamera != null) arCamera.gameObject.SetActive(false);
        if (arPlaneManager != null) arPlaneManager.enabled = false;
        
        if (virtualCamera != null) virtualCamera.gameObject.SetActive(true);
        if (arUI != null) arUI.SetActive(false);
        
        isInARMode = false;
        isPlayerPlaced = false;
        currentPlayer = null;
        moveInput = Vector2.zero;
    }
    
    void UpdateInstructionText(string message)
    {
        if (instructionText != null) instructionText.text = message;
    }
}*/