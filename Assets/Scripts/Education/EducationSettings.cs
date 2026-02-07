using UnityEngine;

public class EducationSettings : MonoBehaviour
{
    public static EducationSettings Instance { get; private set; }
    
    [SerializeField] private DifficultyLevel currentDifficulty = DifficultyLevel.Medium;
    
    public DifficultyLevel CurrentDifficulty 
    { 
        get => currentDifficulty;
        private set
        {
            currentDifficulty = value;
            PlayerPrefs.SetInt("EducationDifficulty", (int)value);
            PlayerPrefs.Save();
            OnDifficultyChanged?.Invoke(value);
        }
    }
    
    public System.Action<DifficultyLevel> OnDifficultyChanged;
    
    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        if (PlayerPrefs.HasKey("EducationDifficulty"))
            currentDifficulty = (DifficultyLevel)PlayerPrefs.GetInt("EducationDifficulty");
    }
    
    public void SetDifficulty(DifficultyLevel difficulty) => CurrentDifficulty = difficulty;
    
    public void SetDifficultyByIndex(int index) => SetDifficulty((DifficultyLevel)Mathf.Clamp(index, 0, 2));
    
    public string GetAgeRange(DifficultyLevel difficulty) => difficulty switch
    {
        DifficultyLevel.Easy => "Ages 6-10",
        DifficultyLevel.Medium => "Ages 11-14",
        DifficultyLevel.Hard => "Ages 15-18",
        _ => "Unknown"
    };
}
