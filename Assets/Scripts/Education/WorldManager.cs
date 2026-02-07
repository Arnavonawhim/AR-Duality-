using UnityEngine;
using System.Collections.Generic;

public class WorldManager : MonoBehaviour
{
    public static WorldManager Instance { get; private set; }
    
    [SerializeField] private WorldData[] allWorlds;
    [SerializeField] private WorldType currentWorld = WorldType.SciFi;
    [SerializeField] private TeleporterType currentTeleporterType = TeleporterType.Learning;
    [SerializeField] private string currentTopic;
    [SerializeField] private int totalKnowledgePoints = 0;
    
    private HashSet<WorldType> completedWorlds = new HashSet<WorldType>();
    private HashSet<PowerType> unlockedPowers = new HashSet<PowerType>();
    
    public System.Action<int> OnKnowledgePointsChanged;
    public System.Action<WorldType> OnWorldCompleted;
    public System.Action<PowerType> OnPowerUnlocked;
    
    public int TotalKnowledgePoints => totalKnowledgePoints;
    public WorldType CurrentWorld => currentWorld;
    public TeleporterType CurrentTeleporterType => currentTeleporterType;
    public string CurrentTopic => currentTopic;
    public WorldData CurrentWorldData => GetWorldData(currentWorld);
    
    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadProgress();
    }
    
    public WorldData GetWorldData(WorldType worldType)
    {
        foreach (var world in allWorlds)
            if (world.worldType == worldType) return world;
        return null;
    }
    
    public void SetCurrentWorld(WorldType world) => currentWorld = world;
    public void SetTeleporterType(TeleporterType type) => currentTeleporterType = type;
    public void SetCurrentTopic(string topic) => currentTopic = topic;
    
    public bool IsWorldUnlocked(WorldType worldType)
    {
        if (worldType == WorldType.SciFi) return true;
        var worldData = GetWorldData(worldType);
        return worldData != null && totalKnowledgePoints >= worldData.knowledgePointsRequired;
    }
    
    public bool IsWorldCompleted(WorldType worldType) => completedWorlds.Contains(worldType);
    public bool IsPowerUnlocked(PowerType powerType) => unlockedPowers.Contains(powerType);
    
    public void AddKnowledgePoints(int points)
    {
        totalKnowledgePoints += points;
        OnKnowledgePointsChanged?.Invoke(totalKnowledgePoints);
        SaveProgress();
    }
    
    public void CompleteWorld(WorldType worldType)
    {
        if (completedWorlds.Contains(worldType)) return;
        completedWorlds.Add(worldType);
        OnWorldCompleted?.Invoke(worldType);
        
        var worldData = GetWorldData(worldType);
        if (worldData != null && worldData.powerReward != PowerType.None)
            UnlockPower(worldData.powerReward);
        SaveProgress();
    }
    
    public void UnlockPower(PowerType powerType)
    {
        if (powerType == PowerType.None || unlockedPowers.Contains(powerType)) return;
        unlockedPowers.Add(powerType);
        OnPowerUnlocked?.Invoke(powerType);
        SaveProgress();
    }
    
    public List<PowerType> GetUnlockedPowers() => new List<PowerType>(unlockedPowers);
    public WorldData[] GetAllWorlds() => allWorlds;
    
    public string GetNextSceneName()
    {
        var worldData = GetWorldData(currentWorld);
        return worldData?.nextSceneName ?? "";
    }
    
    private void SaveProgress()
    {
        PlayerPrefs.SetInt("TotalKnowledgePoints", totalKnowledgePoints);
        PlayerPrefs.SetString("CompletedWorlds", string.Join(",", completedWorlds));
        PlayerPrefs.SetString("UnlockedPowers", string.Join(",", unlockedPowers));
        PlayerPrefs.Save();
    }
    
    private void LoadProgress()
    {
        totalKnowledgePoints = PlayerPrefs.GetInt("TotalKnowledgePoints", 0);
        
        string completedStr = PlayerPrefs.GetString("CompletedWorlds", "");
        if (!string.IsNullOrEmpty(completedStr))
            foreach (string s in completedStr.Split(','))
                if (System.Enum.TryParse(s, out WorldType w)) completedWorlds.Add(w);
        
        string powersStr = PlayerPrefs.GetString("UnlockedPowers", "");
        if (!string.IsNullOrEmpty(powersStr))
            foreach (string s in powersStr.Split(','))
                if (System.Enum.TryParse(s, out PowerType p)) unlockedPowers.Add(p);
    }
    
    [ContextMenu("Reset Progress")]
    public void ResetProgress()
    {
        totalKnowledgePoints = 0;
        completedWorlds.Clear();
        unlockedPowers.Clear();
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }
}
