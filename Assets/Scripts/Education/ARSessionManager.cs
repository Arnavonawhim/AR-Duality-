using UnityEngine;
using UnityEngine.SceneManagement;

public class ARSessionManager : MonoBehaviour
{
    [Header("Character Prefabs")]
    [SerializeField] private GameObject sciFiCharacterPrefab;
    [SerializeField] private GameObject earthCharacterPrefab;
    [SerializeField] private GameObject libraryCharacterPrefab;
    
    [Header("Session Controllers")]
    [SerializeField] private LearningSessionController learningController;
    [SerializeField] private QuizSessionController quizController;
    
    [Header("AR Setup")]
    [SerializeField] private Transform characterSpawnPoint;
    
    private GameObject currentCharacterInstance;
    
    public static ARSessionManager Instance { get; private set; }
    
    void Awake()
    {
        Instance = this;
    }
    
    void Start()
    {
        SetupSession();
    }
    
    void SetupSession()
    {
        if (WorldManager.Instance == null) { Debug.LogError("WorldManager not found!"); return; }
        
        SpawnCharacter();
        
        if (WorldManager.Instance.CurrentTeleporterType == TeleporterType.Learning)
        {
            if (learningController) learningController.gameObject.SetActive(true);
            if (quizController) quizController.gameObject.SetActive(false);
            learningController?.StartLearningSession();
        }
        else
        {
            if (quizController) quizController.gameObject.SetActive(true);
            if (learningController) learningController.gameObject.SetActive(false);
            quizController?.StartQuizSession();
        }
    }
    
    void SpawnCharacter()
    {
        var worldData = WorldManager.Instance.CurrentWorldData;
        if (worldData?.characterPrefab != null)
        {
            currentCharacterInstance = Instantiate(worldData.characterPrefab, characterSpawnPoint.position, characterSpawnPoint.rotation);
            return;
        }
        
        GameObject prefab = WorldManager.Instance.CurrentWorld switch
        {
            WorldType.SciFi => sciFiCharacterPrefab,
            WorldType.Earth => earthCharacterPrefab,
            WorldType.Library => libraryCharacterPrefab,
            _ => null
        };
        
        if (prefab) currentCharacterInstance = Instantiate(prefab, characterSpawnPoint.position, characterSpawnPoint.rotation);
    }
    
    public void OnRobotPlaced(Transform robotPosition)
    {
        if (currentCharacterInstance)
        {
            Vector3 frontOffset = robotPosition.position + robotPosition.forward * 1.5f;
            currentCharacterInstance.transform.position = frontOffset;
            currentCharacterInstance.transform.LookAt(robotPosition);
        }
    }
    
    public void ReturnToWorld()
    {
        string sceneName = SceneManager.GetActiveScene().name == "ARScene" 
            ? GetCurrentWorldScene() 
            : "ARScene";
        SceneManager.LoadScene(sceneName);
    }
    
    public void GoToNextWorld()
    {
        string nextScene = WorldManager.Instance.GetNextSceneName();
        if (!string.IsNullOrEmpty(nextScene))
            SceneManager.LoadScene(nextScene);
        else
            Debug.Log("Game Complete!");
    }
    
    string GetCurrentWorldScene()
    {
        return WorldManager.Instance.CurrentWorld switch
        {
            WorldType.SciFi => "SciFiScene",
            WorldType.Earth => "EarthScene",
            WorldType.Library => "LibraryScene",
            _ => "SciFiScene"
        };
    }
}
