using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EducationHUD : MonoBehaviour
{
    [Header("Knowledge Points")]
    [SerializeField] private TextMeshProUGUI knowledgePointsText;
    
    [Header("Power Display")]
    [SerializeField] private GameObject powerPanel;
    [SerializeField] private Image powerTimerFill;
    [SerializeField] private TextMeshProUGUI powerNameText;
    [SerializeField] private Button togglePowerButton;
    
    [Header("Info")]
    [SerializeField] private TextMeshProUGUI difficultyText;
    [SerializeField] private TextMeshProUGUI worldProgressText;
    
    void Start()
    {
        if (powerPanel) powerPanel.SetActive(false);
        
        if (WorldManager.Instance != null)
        {
            WorldManager.Instance.OnKnowledgePointsChanged += UpdatePoints;
            UpdatePoints(WorldManager.Instance.TotalKnowledgePoints);
            UpdateWorldProgress();
        }
        
        if (PowerManager.Instance != null)
        {
            PowerManager.Instance.OnPowerActivated += OnPowerActivated;
            PowerManager.Instance.OnPowerDeactivated += OnPowerDeactivated;
            PowerManager.Instance.OnPowerTimerUpdate += (c, m) => { if (powerTimerFill) powerTimerFill.fillAmount = c / m; };
        }
        
        if (EducationSettings.Instance != null)
        {
            EducationSettings.Instance.OnDifficultyChanged += d => { if (difficultyText) difficultyText.text = EducationSettings.Instance.GetAgeRange(d); };
            if (difficultyText) difficultyText.text = EducationSettings.Instance.GetAgeRange(EducationSettings.Instance.CurrentDifficulty);
        }
        
        if (togglePowerButton)
        {
            togglePowerButton.onClick.AddListener(() => PowerManager.Instance?.ToggleGravityManipulation());
            togglePowerButton.gameObject.SetActive(false);
        }
    }
    
    void UpdatePoints(int pts) { if (knowledgePointsText) knowledgePointsText.text = $"⭐ {pts}"; }
    
    void UpdateWorldProgress()
    {
        if (!worldProgressText || WorldManager.Instance == null) return;
        int done = 0;
        if (WorldManager.Instance.IsWorldCompleted(WorldType.Earth)) done++;
        if (WorldManager.Instance.IsWorldCompleted(WorldType.Library)) done++;
        if (WorldManager.Instance.IsWorldCompleted(WorldType.Space)) done++;
        worldProgressText.text = $"Worlds: {done}/3";
    }
    
    void OnPowerActivated(PowerType p)
    {
        if (powerPanel) powerPanel.SetActive(true);
        if (powerNameText) powerNameText.text = PowerManager.Instance?.GetPowerName(p);
        if (togglePowerButton) togglePowerButton.gameObject.SetActive(p == PowerType.GravityManipulation);
    }
    
    void OnPowerDeactivated(PowerType p)
    {
        if (powerPanel) powerPanel.SetActive(false);
        if (togglePowerButton) togglePowerButton.gameObject.SetActive(false);
    }
}
