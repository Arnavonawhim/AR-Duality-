using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class LearningSessionController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject learningUI;
    [SerializeField] private TextMeshProUGUI topicText;
    [SerializeField] private TextMeshProUGUI subtitleText;
    [SerializeField] private Button askDoubtsButton;
    [SerializeField] private Button noDoubtsButton;
    [SerializeField] private GameObject doubtsPanel;
    [SerializeField] private Button finishDoubtsButton;
    
    [Header("Convai Integration")]
    [SerializeField] private ConvaiEducationBridge convaiBridge;
    
    [Header("Settings")]
    [SerializeField] private float lessonDuration = 90f;
    
    private bool isTeaching;
    private bool isAskingDoubts;
    
    void Start()
    {
        if (askDoubtsButton) askDoubtsButton.onClick.AddListener(OnAskDoubts);
        if (noDoubtsButton) noDoubtsButton.onClick.AddListener(OnNoDoubts);
        if (finishDoubtsButton) finishDoubtsButton.onClick.AddListener(OnFinishDoubts);
        
        if (learningUI) learningUI.SetActive(false);
        if (doubtsPanel) doubtsPanel.SetActive(false);
    }
    
    public void StartLearningSession()
    {
        if (WorldManager.Instance == null) return;
        
        string topic = WorldManager.Instance.CurrentTopic;
        var worldData = WorldManager.Instance.CurrentWorldData;
        
        if (topicText) topicText.text = topic;
        if (learningUI) learningUI.SetActive(true);
        
        StartCoroutine(TeachingRoutine(worldData));
    }
    
    IEnumerator TeachingRoutine(WorldData worldData)
    {
        isTeaching = true;
        
        // Start Convai teaching
        if (convaiBridge && worldData != null)
        {
            string characterId = worldData.convaiCharacterId;
            if (!string.IsNullOrEmpty(characterId))
                convaiBridge.SetActiveCharacter(characterId);
            
            // Send teaching prompt
            if (!string.IsNullOrEmpty(worldData.teachingScript))
                convaiBridge.SendTextInput(worldData.teachingScript);
        }
        
        // Wait for lesson duration
        float elapsed = 0f;
        while (elapsed < lessonDuration && isTeaching)
        {
            elapsed += Time.deltaTime;
            if (subtitleText) subtitleText.text = $"Lesson: {Mathf.CeilToInt(lessonDuration - elapsed)}s remaining";
            yield return null;
        }
        
        // Show ask doubts buttons
        ShowDoubtsPrompt();
    }
    
    void ShowDoubtsPrompt()
    {
        isTeaching = false;
        if (askDoubtsButton) askDoubtsButton.gameObject.SetActive(true);
        if (noDoubtsButton) noDoubtsButton.gameObject.SetActive(true);
        if (subtitleText) subtitleText.text = "Lesson complete! Any doubts?";
    }
    
    void OnAskDoubts()
    {
        isAskingDoubts = true;
        if (doubtsPanel) doubtsPanel.SetActive(true);
        if (askDoubtsButton) askDoubtsButton.gameObject.SetActive(false);
        if (noDoubtsButton) noDoubtsButton.gameObject.SetActive(false);
        
        // Enable voice input
        if (convaiBridge) convaiBridge.StartListening();
        if (subtitleText) subtitleText.text = "Ask your question...";
    }
    
    void OnFinishDoubts()
    {
        isAskingDoubts = false;
        if (doubtsPanel) doubtsPanel.SetActive(false);
        if (convaiBridge) convaiBridge.StopListening();
        
        EndSession();
    }
    
    void OnNoDoubts()
    {
        EndSession();
    }
    
    void EndSession()
    {
        WorldManager.Instance?.AddKnowledgePoints(10);
        ARSessionManager.Instance?.ReturnToWorld();
    }
}
