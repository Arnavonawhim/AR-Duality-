using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WorldMapUI : MonoBehaviour
{
    [SerializeField] private GameObject mapPanel;
    [SerializeField] private Button closeButton;
    [SerializeField] private WorldMapEntry earthEntry;
    [SerializeField] private WorldMapEntry libraryEntry;
    [SerializeField] private WorldMapEntry spaceEntry;
    [SerializeField] private TextMeshProUGUI totalPointsText;
    [SerializeField] private TextMeshProUGUI powersText;
    
    void Start()
    {
        if (mapPanel) mapPanel.SetActive(false);
        if (closeButton) closeButton.onClick.AddListener(Hide);
    }
    
    public void Show()
    {
        if (mapPanel) mapPanel.SetActive(true);
        RefreshUI();
        Time.timeScale = 0f;
    }
    
    public void Hide()
    {
        if (mapPanel) mapPanel.SetActive(false);
        Time.timeScale = 1f;
    }
    
    public void Toggle() { if (mapPanel?.activeSelf == true) Hide(); else Show(); }
    
    void RefreshUI()
    {
        if (WorldManager.Instance == null) return;
        
        UpdateEntry(earthEntry, WorldType.Earth);
        UpdateEntry(libraryEntry, WorldType.Library);
        UpdateEntry(spaceEntry, WorldType.Space);
        
        if (totalPointsText) totalPointsText.text = $"Knowledge: {WorldManager.Instance.TotalKnowledgePoints}";
        if (powersText) powersText.text = $"Powers: {WorldManager.Instance.GetUnlockedPowers().Count}/3";
    }
    
    void UpdateEntry(WorldMapEntry entry, WorldType type)
    {
        if (entry == null || WorldManager.Instance == null) return;
        bool unlocked = WorldManager.Instance.IsWorldUnlocked(type);
        bool completed = WorldManager.Instance.IsWorldCompleted(type);
        var data = WorldManager.Instance.GetWorldData(type);
        entry.SetData(data?.worldName ?? type.ToString(), unlocked, completed,
            data?.knowledgePointsRequired ?? 0, WorldManager.Instance.TotalKnowledgePoints);
    }
}

[System.Serializable]
public class WorldMapEntry
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI statusText;
    public GameObject lockIcon;
    public GameObject completedIcon;
    public Button enterButton;
    
    public void SetData(string name, bool unlocked, bool completed, int req, int curr)
    {
        if (nameText) nameText.text = name;
        if (lockIcon) lockIcon.SetActive(!unlocked);
        if (completedIcon) completedIcon.SetActive(completed);
        if (enterButton) enterButton.interactable = unlocked;
        if (statusText)
        {
            if (completed) statusText.text = "✓ Done";
            else if (unlocked) statusText.text = "Ready";
            else statusText.text = $"{curr}/{req}";
        }
    }
}
