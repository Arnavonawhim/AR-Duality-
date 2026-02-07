using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DifficultySelector : MonoBehaviour
{
    [SerializeField] private Button easyButton;
    [SerializeField] private Button mediumButton;
    [SerializeField] private Button hardButton;
    [SerializeField] private Color selectedColor = new Color(0.2f, 0.8f, 0.2f);
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private TextMeshProUGUI currentDifficultyText;
    
    private Image easyImg, mediumImg, hardImg;
    
    void Start()
    {
        if (easyButton) { easyImg = easyButton.GetComponent<Image>(); easyButton.onClick.AddListener(() => SetDiff(DifficultyLevel.Easy)); }
        if (mediumButton) { mediumImg = mediumButton.GetComponent<Image>(); mediumButton.onClick.AddListener(() => SetDiff(DifficultyLevel.Medium)); }
        if (hardButton) { hardImg = hardButton.GetComponent<Image>(); hardButton.onClick.AddListener(() => SetDiff(DifficultyLevel.Hard)); }
        
        if (EducationSettings.Instance != null)
        {
            EducationSettings.Instance.OnDifficultyChanged += UpdateVisuals;
            UpdateVisuals(EducationSettings.Instance.CurrentDifficulty);
        }
    }
    
    void OnDestroy()
    {
        if (EducationSettings.Instance != null)
            EducationSettings.Instance.OnDifficultyChanged -= UpdateVisuals;
    }
    
    void SetDiff(DifficultyLevel d) => EducationSettings.Instance?.SetDifficulty(d);
    
    void UpdateVisuals(DifficultyLevel current)
    {
        if (easyImg) easyImg.color = current == DifficultyLevel.Easy ? selectedColor : normalColor;
        if (mediumImg) mediumImg.color = current == DifficultyLevel.Medium ? selectedColor : normalColor;
        if (hardImg) hardImg.color = current == DifficultyLevel.Hard ? selectedColor : normalColor;
        if (currentDifficultyText && EducationSettings.Instance != null)
            currentDifficultyText.text = EducationSettings.Instance.GetAgeRange(current);
    }
}
