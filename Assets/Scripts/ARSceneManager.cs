using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using Unity.XR.CoreUtils;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;

public class ARSceneManager : MonoBehaviour
{
    [Header("AR Components")]
    [SerializeField] private ARSession arSession;
    [SerializeField] private XROrigin xrOrigin;
    [SerializeField] private ARPlaneManager arPlaneManager;
    [SerializeField] private ARRaycastManager arRaycastManager;
    [SerializeField] private Camera arCamera;
    
    [Header("Player")]
    [SerializeField] private GameObject playerPrefab;
    
    [Header("UI")]
    [SerializeField] private TMP_Text instructionText;
    [SerializeField] private Slider scaleSlider;
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;
    [SerializeField] private Button exitARButton;
    
    [Header("AR Settings")]
    [SerializeField] private float arMoveSpeed = 0.5f;
    [SerializeField] private float maxARDistance = 3f;
    [SerializeField] private float minScale = 0.5f;
    [SerializeField] private float maxScale = 2.5f;
    [SerializeField] private float defaultScale = 1f;
    [SerializeField] private string virtualSceneName = "MainGame";
    
    [Header("Enemy Settings")]
    [SerializeField] private GameObject photonEnemyPrefab;
    [SerializeField] private GameObject antiMatterEnemyPrefab;
    [SerializeField] private int minEnemies = 2;
    [SerializeField] private int maxEnemies = 5;
    [SerializeField] private float enemySpawnRadius = 2f;
    
    private GameObject arPlayerInstance;
    private Vector3 arStartPosition;
    private float currentScale = 1f;
    private bool isPlayerPlaced = false;
    private List<ARRaycastHit> raycastHits = new List<ARRaycastHit>();
    private float moveInputX = 0f;
    private List<GameObject> spawnedEnemies = new List<GameObject>();
    private bool isFacingRight = true;
    
    void Start()
    {
        currentScale = defaultScale;
        
        if (scaleSlider != null)
        {
            scaleSlider.minValue = minScale;
            scaleSlider.maxValue = maxScale;
            scaleSlider.value = defaultScale;
            scaleSlider.onValueChanged.AddListener(OnScaleChanged);
        }
        
        SetupMoveButtons();
        
        if (exitARButton != null) exitARButton.onClick.AddListener(ExitARMode);
        
        UpdateInstructionText("Point at flat surface and tap to place");
    }
    
    void SetupMoveButtons()
    {
        if (leftButton != null)
        {
            var leftTrigger = leftButton.gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();
            var pdLeft = new UnityEngine.EventSystems.EventTrigger.Entry { eventID = UnityEngine.EventSystems.EventTriggerType.PointerDown };
            pdLeft.callback.AddListener((data) => { moveInputX = -1f; });
            leftTrigger.triggers.Add(pdLeft);
            var puLeft = new UnityEngine.EventSystems.EventTrigger.Entry { eventID = UnityEngine.EventSystems.EventTriggerType.PointerUp };
            puLeft.callback.AddListener((data) => { moveInputX = 0f; });
            leftTrigger.triggers.Add(puLeft);
        }
        
        if (rightButton != null)
        {
            var rightTrigger = rightButton.gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();
            var pdRight = new UnityEngine.EventSystems.EventTrigger.Entry { eventID = UnityEngine.EventSystems.EventTriggerType.PointerDown };
            pdRight.callback.AddListener((data) => { moveInputX = 1f; });
            rightTrigger.triggers.Add(pdRight);
            var puRight = new UnityEngine.EventSystems.EventTrigger.Entry { eventID = UnityEngine.EventSystems.EventTriggerType.PointerUp };
            puRight.callback.AddListener((data) => { moveInputX = 0f; });
            rightTrigger.triggers.Add(puRight);
        }
    }
    
    void Update()
    {
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
        if (playerPrefab == null) return;
        
        arPlayerInstance = Instantiate(playerPrefab, position, Quaternion.identity);
        arPlayerInstance.transform.localScale = Vector3.one * 0.1f;
        
        var controller = arPlayerInstance.GetComponent<CharacterController2D>();
        if (controller != null) Destroy(controller);
        
        var rb = arPlayerInstance.GetComponent<Rigidbody>();
        if (rb != null) Destroy(rb);
        
        var colliders = arPlayerInstance.GetComponentsInChildren<Collider>();
        foreach (var col in colliders)
        {
            if (!col.isTrigger) Destroy(col);
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
        if (arPlayerInstance == null || Mathf.Abs(moveInputX) < 0.01f) return;
        
        Vector3 right = arCamera.transform.right;
        right.y = 0f;
        right.Normalize();
        
        Vector3 movement = right * moveInputX * arMoveSpeed * Time.deltaTime;
        arPlayerInstance.transform.position += movement;
        
        if (moveInputX > 0 && !isFacingRight)
        {
            FlipPlayer();
        }
        else if (moveInputX < 0 && isFacingRight)
        {
            FlipPlayer();
        }
    }
    
    void FlipPlayer()
    {
        if (arPlayerInstance == null) return;
        
        isFacingRight = !isFacingRight;
        Vector3 scale = arPlayerInstance.transform.localScale;
        scale.x *= -1;
        arPlayerInstance.transform.localScale = scale;
    }
    
    void CheckARDistance()
    {
        if (arPlayerInstance == null) return;
        
        float distance = Vector3.Distance(arPlayerInstance.transform.position, arStartPosition);
        if (distance > maxARDistance)
        {
            UpdateInstructionText("Too far! Returning...");
            Invoke(nameof(ExitARMode), 1f);
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
            Vector3 scale = arPlayerInstance.transform.localScale;
            scale.y = 0.1f * newScale;
            scale.z = 0.1f * newScale;
            arPlayerInstance.transform.localScale = scale;
        }
    }
    
    public void ExitARMode()
    {
        Vector3 arMovementDelta = Vector3.zero;
        if (arPlayerInstance != null)
        {
            arMovementDelta = arPlayerInstance.transform.position - arStartPosition;
        }
        
        if (ARDataManager.Instance != null)
        {
            ARDataManager.Instance.StoreARData(arMovementDelta, currentScale);
        }
        
        SceneManager.LoadScene(virtualSceneName);
    }
    
    void UpdateInstructionText(string message)
    {
        if (instructionText != null) instructionText.text = message;
    }
}