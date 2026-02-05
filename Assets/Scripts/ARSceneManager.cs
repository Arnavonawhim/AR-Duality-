using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

[RequireComponent(typeof(ARRaycastManager), typeof(ARPlaneManager))]
public class ARSceneManager : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject robotPrefab;
    [SerializeField] private GameObject photonEnemyPrefab;
    [SerializeField] private GameObject antimatterEnemyPrefab;

    [Header("Spawn Settings")]
    [SerializeField] private int maxPhotonEnemies = 5;
    [SerializeField] private int maxAntimatterEnemies = 3;
    [SerializeField] private float enemySpawnRadius = 2f;

    [Header("UI")]
    [SerializeField] private GameObject placementUI;
    [SerializeField] private GameObject gameplayUI;

    private ARRaycastManager raycastManager;
    private ARPlaneManager planeManager;
    private GameObject robotInstance;
    private bool robotPlaced;

    void Awake()
    {
        raycastManager = GetComponent<ARRaycastManager>();
        planeManager = GetComponent<ARPlaneManager>();
    }

    void Update()
    {
        if (!robotPlaced && Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                List<ARRaycastHit> hits = new List<ARRaycastHit>();
                if (raycastManager.Raycast(touch.position, hits, TrackableType.PlaneWithinPolygon))
                {
                    PlaceRobot(hits[0].pose.position);
                }
            }
        }
    }

    void PlaceRobot(Vector3 position)
    {
        robotInstance = Instantiate(robotPrefab, position, Quaternion.identity);
        robotPlaced = true;
        placementUI.SetActive(false);
        gameplayUI.SetActive(true);
        
        planeManager.enabled = false;
        foreach (var plane in planeManager.trackables)
        {
            plane.gameObject.SetActive(false);
        }

        SpawnEnemies(position);
    }

    void SpawnEnemies(Vector3 center)
    {
        for (int i = 0; i < maxPhotonEnemies; i++)
        {
            Vector3 pos = center + new Vector3(Random.Range(-enemySpawnRadius, enemySpawnRadius), 0, Random.Range(-enemySpawnRadius, enemySpawnRadius));
            Instantiate(photonEnemyPrefab, pos, Quaternion.identity);
        }

        for (int i = 0; i < maxAntimatterEnemies; i++)
        {
            Vector3 pos = center + new Vector3(Random.Range(-enemySpawnRadius, enemySpawnRadius), 0, Random.Range(-enemySpawnRadius, enemySpawnRadius));
            Instantiate(antimatterEnemyPrefab, pos, Quaternion.identity);
        }
    }

    public void ExitARWorld()
    {
        if (robotInstance != null)
        {
            ARRobotController robot = robotInstance.GetComponent<ARRobotController>();
            if (robot != null && GameDataManager.Instance != null)
            {
                GameDataManager.Instance.ExitAR(robot.GetTotalMovement(), robot.transform.localScale.x);
            }
        }
        SceneManager.LoadScene("SidescrollerScene");
    }
}