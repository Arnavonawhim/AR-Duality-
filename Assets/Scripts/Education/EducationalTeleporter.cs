using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EducationalTeleporter : MonoBehaviour
{
    [Header("World")]
    [SerializeField] private WorldType destinationWorld;
    [SerializeField] private WorldData worldData;
    
    [Header("UI")]
    [SerializeField] private GameObject promptPanel;
    [SerializeField] private Button enterButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private TMPro.TextMeshProUGUI worldNameText;
    [SerializeField] private TMPro.TextMeshProUGUI descriptionText;
    [SerializeField] private TMPro.TextMeshProUGUI requirementText;
    [SerializeField] private GameObject lockedOverlay;
    
    [Header("Visuals")]
    [SerializeField] private ParticleSystem portalEffect;
    [SerializeField] private Light portalLight;
    [SerializeField] private Color unlockedColor = new Color(0.3f, 0.8f, 1f);
    [SerializeField] private Color lockedColor = new Color(0.5f, 0.5f, 0.5f);
    [SerializeField] private Color completedColor = new Color(0.3f, 1f, 0.5f);
    
    [Header("Scene")]
    [SerializeField] private string arSceneName = "ARScene";
    
    private bool playerInRange;
    private PlayerController player;
    private bool isUnlocked, isCompleted;
    
    void Start()
    {
        if (worldData != null)
        {
            destinationWorld = worldData.worldType;
            arSceneName = worldData.arSceneName;
        }
        if (promptPanel) promptPanel.SetActive(false);
        if (enterButton) enterButton.onClick.AddListener(OnEnterPressed);
        if (cancelButton) cancelButton.onClick.AddListener(() => promptPanel?.SetActive(false));
        UpdatePortalState();
        
        if (WorldManager.Instance != null)
        {
            WorldManager.Instance.OnKnowledgePointsChanged += _ => UpdatePortalState();
            WorldManager.Instance.OnWorldCompleted += _ => UpdatePortalState();
        }
    }
    
    void UpdatePortalState()
    {
        if (WorldManager.Instance == null) return;
        isUnlocked = WorldManager.Instance.IsWorldUnlocked(destinationWorld);
        isCompleted = WorldManager.Instance.IsWorldCompleted(destinationWorld);
        
        Color c = isCompleted ? completedColor : (isUnlocked ? unlockedColor : lockedColor);
        if (portalLight) portalLight.color = c;
        if (portalEffect) { var m = portalEffect.main; m.startColor = c; }
    }
    
    void OnTriggerEnter(Collider other)
    {
        var p = other.GetComponent<PlayerController>();
        if (p) { player = p; playerInRange = true; ShowPrompt(); }
    }
    
    void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerController>()) { playerInRange = false; promptPanel?.SetActive(false); }
    }
    
    void ShowPrompt()
    {
        if (!promptPanel) return;
        promptPanel.SetActive(true);
        
        var data = worldData ?? WorldManager.Instance?.GetWorldData(destinationWorld);
        if (data != null)
        {
            if (worldNameText) worldNameText.text = data.worldName;
            if (descriptionText) descriptionText.text = data.worldDescription;
        }
        
        if (lockedOverlay) lockedOverlay.SetActive(!isUnlocked);
        if (enterButton) enterButton.interactable = isUnlocked;
        
        if (requirementText)
        {
            if (isCompleted) requirementText.text = "✓ Completed!";
            else if (isUnlocked) requirementText.text = "Ready to enter!";
            else
            {
                int req = worldData?.knowledgePointsRequired ?? 0;
                int curr = WorldManager.Instance?.TotalKnowledgePoints ?? 0;
                requirementText.text = $"🔒 {curr}/{req} points";
            }
        }
    }
    
    public void OnEnterPressed()
    {
        if (!isUnlocked) return;
        GameDataManager.Instance?.SetPlayerPositionBeforeAR(player.transform.position);
        WorldManager.Instance?.SetCurrentWorld(destinationWorld);
        FindObjectOfType<RobotCompanion2D>()?.OnEnterARMode();
        SceneManager.LoadScene(arSceneName);
    }
}
