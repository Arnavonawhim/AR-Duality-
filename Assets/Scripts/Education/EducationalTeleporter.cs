using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class EducationalTeleporter : MonoBehaviour
{
    [Header("Teleporter Type")]
    [SerializeField] private TeleporterType teleporterType = TeleporterType.Learning;
    [SerializeField] private WorldType sourceWorld;
    [SerializeField] private string topicName;
    
    [Header("References")]
    [SerializeField] private WorldData worldData;
    
    [Header("UI")]
    [SerializeField] private GameObject promptPanel;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Button enterButton;
    [SerializeField] private Button cancelButton;
    
    [Header("Quiz Settings")]
    [SerializeField] private string quizPromptText = "To access interdimensional travel, you must pass this quiz!";
    
    private bool playerInRange;
    private PlayerController player;
    
    void Start()
    {
        if (promptPanel) promptPanel.SetActive(false);
        if (enterButton) enterButton.onClick.AddListener(OnEnterPressed);
        if (cancelButton) cancelButton.onClick.AddListener(() => promptPanel?.SetActive(false));
    }
    
    void OnTriggerEnter(Collider other)
    {
        var p = other.GetComponent<PlayerController>();
        if (p) { player = p; playerInRange = true; ShowPrompt(); }
    }
    
    void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerController>()) 
        { 
            playerInRange = false; 
            promptPanel?.SetActive(false); 
        }
    }
    
    void ShowPrompt()
    {
        if (!promptPanel) return;
        promptPanel.SetActive(true);
        
        if (teleporterType == TeleporterType.Learning)
        {
            if (titleText) titleText.text = $"Learn: {topicName}";
            if (descriptionText) descriptionText.text = "Enter to start a lesson with your AI tutor. (Optional)";
        }
        else
        {
            if (titleText) titleText.text = "Dimensional Quiz";
            if (descriptionText) descriptionText.text = quizPromptText;
        }
    }
    
    public void OnEnterPressed()
    {
        if (!playerInRange) return;
        
        GameDataManager.Instance?.SetPlayerPositionBeforeAR(player.transform.position);
        
        if (WorldManager.Instance != null)
        {
            WorldManager.Instance.SetCurrentWorld(sourceWorld);
            WorldManager.Instance.SetTeleporterType(teleporterType);
            WorldManager.Instance.SetCurrentTopic(topicName);
        }
        
        FindObjectOfType<RobotCompanion2D>()?.OnEnterARMode();
        SceneManager.LoadScene("ARScene");
    }
}
