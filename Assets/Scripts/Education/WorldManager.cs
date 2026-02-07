using UnityEngine;
using System.Collections.Generic;

public class WorldManager : MonoBehaviour
{
    public static WorldManager Instance { get; private set; }
    
    [SerializeField] private WorldData[] allWorlds;
    [SerializeField] private WorldType currentWorld = WorldType.Earth;
    [SerializeField] private int totalKnowledgePoints = 0;
    
    private HashSet<WorldType> completedWorlds = new HashSet<WorldType>();
    private HashSet<PowerType> unlockedPowers = new HashSet<PowerType>();
    private Dictionary<WorldType, int> worldKnowledgePoints = new Dictionary<WorldType, int>();
    
    public System.Action<int> OnKnowledgePointsChanged;
    public System.Action<WorldType> OnWorldCompleted;
    public System.Action<PowerType> OnPowerUnlocked;
    
    public int TotalKnowledgePoints => totalKnowledgePoints;
    public WorldType CurrentWorld => currentWorld;
    public WorldData CurrentWorldData => GetWorldData(currentWorld);
    
    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        InitializeWorldPoints();
        LoadProgress();
    }
    
    private void InitializeWorldPoints()
    {
        foreach (WorldType world in System.Enum.GetValues(typeof(WorldType)))
            if (!worldKnowledgePoints.ContainsKey(world))
                worldKnowledgePoints[world] = 0;
    }
    
    public WorldData GetWorldData(WorldType worldType)
    {
        foreach (var world in allWorlds)
            if (world.worldType == worldType) return world;
        return null;
    }
    
    public bool IsWorldUnlocked(WorldType worldType)
    {
        if (worldType == WorldType.Earth) return true;
        var worldData = GetWorldData(worldType);
        return worldData != null && totalKnowledgePoints >= worldData.knowledgePointsRequired;
    }
    
    public bool IsWorldCompleted(WorldType worldType) => completedWorlds.Contains(worldType);
    public bool IsPowerUnlocked(PowerType powerType) => unlockedPowers.Contains(powerType);
    
    public void AddKnowledgePoints(int points, WorldType world)
    {
        totalKnowledgePoints += points;
        if (worldKnowledgePoints.ContainsKey(world)) worldKnowledgePoints[world] += points;
        OnKnowledgePointsChanged?.Invoke(totalKnowledgePoints);
        SaveProgress();
    }
    
    public int GetWorldPoints(WorldType worldType) => 
        worldKnowledgePoints.TryGetValue(worldType, out int points) ? points : 0;
    
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
    
    public void SetCurrentWorld(WorldType worldType) => currentWorld = worldType;
    public List<PowerType> GetUnlockedPowers() => new List<PowerType>(unlockedPowers);
    public WorldData[] GetAllWorlds() => allWorlds;
    
    private void SaveProgress()
    {
        PlayerPrefs.SetInt("TotalKnowledgePoints", totalKnowledgePoints);
        PlayerPrefs.SetString("CompletedWorlds", string.Join(",", completedWorlds));
        PlayerPrefs.SetString("UnlockedPowers", string.Join(",", unlockedPowers));
        foreach (var kvp in worldKnowledgePoints)
            PlayerPrefs.SetInt($"WorldPoints_{kvp.Key}", kvp.Value);
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
        
        foreach (WorldType world in System.Enum.GetValues(typeof(WorldType)))
            worldKnowledgePoints[world] = PlayerPrefs.GetInt($"WorldPoints_{world}", 0);
    }
    
    [ContextMenu("Reset Progress")]
    public void ResetProgress()
    {
        totalKnowledgePoints = 0;
        completedWorlds.Clear();
        unlockedPowers.Clear();
        InitializeWorldPoints();
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }
}
