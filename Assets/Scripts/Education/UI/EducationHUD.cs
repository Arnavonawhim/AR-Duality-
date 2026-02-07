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
    [SerializeField] private Button usePowerButton;
    
    [Header("Info")]
    [SerializeField] private TextMeshProUGUI difficultyText;
    [SerializeField] private TextMeshProUGUI worldProgressText;
    
    private float maxPowerDuration;
    
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
            PowerManager.Instance.OnPowerTimerUpdate += UpdatePowerTimer;
        }
        
        if (EducationSettings.Instance != null)
        {
            EducationSettings.Instance.OnDifficultyChanged += d => { if (difficultyText) difficultyText.text = EducationSettings.Instance.GetAgeRange(d); };
            if (difficultyText) difficultyText.text = EducationSettings.Instance.GetAgeRange(EducationSettings.Instance.CurrentDifficulty);
        }
        
        if (usePowerButton)
        {
            usePowerButton.onClick.AddListener(OnUsePowerClicked);
            usePowerButton.gameObject.SetActive(false);
        }
    }
    
    void UpdatePoints(int pts) { if (knowledgePointsText) knowledgePointsText.text = $"⭐ {pts}"; }
    
    void UpdateWorldProgress()
    {
        if (!worldProgressText || WorldManager.Instance == null) return;
        int done = 0;
        if (WorldManager.Instance.IsWorldCompleted(WorldType.SciFi)) done++;
        if (WorldManager.Instance.IsWorldCompleted(WorldType.Earth)) done++;
        if (WorldManager.Instance.IsWorldCompleted(WorldType.Library)) done++;
        worldProgressText.text = $"Worlds: {done}/3";
    }
    
    void OnPowerActivated(PowerType p, float duration)
    {
        maxPowerDuration = duration;
        if (powerPanel) powerPanel.SetActive(true);
        if (powerNameText) powerNameText.text = GetPowerName(p);
    }
    
    void OnPowerDeactivated()
    {
        if (powerPanel) powerPanel.SetActive(false);
        if (usePowerButton) usePowerButton.gameObject.SetActive(false);
    }
    
    void UpdatePowerTimer(float remaining)
    {
        if (powerTimerFill && maxPowerDuration > 0)
            powerTimerFill.fillAmount = remaining / maxPowerDuration;
    }
    
    void OnUsePowerClicked()
    {
        var powers = WorldManager.Instance?.GetUnlockedPowers();
        if (powers != null && powers.Count > 0)
            PowerManager.Instance?.ActivatePower(powers[0]);
    }
    
    string GetPowerName(PowerType p) => p switch
    {
        PowerType.Jetpack => "Jetpack",
        PowerType.GravityPull => "Gravity Pull",
        _ => "None"
    };
}
