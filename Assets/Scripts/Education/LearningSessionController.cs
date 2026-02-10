using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using Convai.Scripts.Runtime.Core;

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
    
    [Header("Settings")]
    [SerializeField] private float lessonDuration = 90f;
    
    private ConvaiNPC convaiNPC;
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
    
    /// <summary>
    /// Set the ConvaiNPC reference for direct voice interaction
    /// MUST be called before StartLearningSession()
    /// </summary>
    public void SetConvaiNPC(ConvaiNPC npc)
    {
        convaiNPC = npc;
        Debug.Log($"[Learning] ConvaiNPC set: {(npc != null ? npc.gameObject.name : "NULL")}");
    }
    
    public void StartLearningSession()
    {
        Debug.Log("[Learning] StartLearningSession called");
        
        if (convaiNPC == null)
        {
            Debug.LogError("[Learning] ConvaiNPC is NULL! Character won't speak.");
        }
        
        string topic = "General Learning";
        WorldData worldData = null;
        
        if (WorldManager.Instance != null)
        {
            topic = WorldManager.Instance.CurrentTopic;
            worldData = WorldManager.Instance.CurrentWorldData;
        }
        
        if (topicText) topicText.text = topic;
        if (learningUI) learningUI.SetActive(true);
        
        // Hide doubts buttons initially
        if (askDoubtsButton) askDoubtsButton.gameObject.SetActive(false);
        if (noDoubtsButton) noDoubtsButton.gameObject.SetActive(false);
        
        StartCoroutine(TeachingRoutine(worldData, topic));
    }
    
    IEnumerator TeachingRoutine(WorldData worldData, string topic)
    {
        isTeaching = true;
        
        // Send teaching prompt to the Convai character
        if (convaiNPC != null)
        {
            string teachingPrompt;
            
            if (worldData != null && !string.IsNullOrEmpty(worldData.teachingScript))
            {
                teachingPrompt = worldData.teachingScript;
            }
            else
            {
                // Default teaching prompt with topic
                teachingPrompt = $"Please teach me about {topic}. Give a clear, educational explanation suitable for a student. Keep it engaging and informative for about 1-2 minutes.";
            }
            
            Debug.Log($"[Learning] Sending teaching prompt: {teachingPrompt}");
            convaiNPC.SendTextDataAsync(teachingPrompt);
        }
        else
        {
            Debug.LogError("[Learning] Cannot teach - ConvaiNPC is null!");
            if (subtitleText) subtitleText.text = "Error: AI Tutor not available";
        }
        
        // Wait for lesson duration
        float elapsed = 0f;
        while (elapsed < lessonDuration && isTeaching)
        {
            elapsed += Time.deltaTime;
            if (subtitleText) subtitleText.text = $"Learning: {Mathf.CeilToInt(lessonDuration - elapsed)}s remaining";
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
        Debug.Log("[Learning] Lesson complete, showing doubts prompt");
    }
    
    void OnAskDoubts()
    {
        isAskingDoubts = true;
        if (doubtsPanel) doubtsPanel.SetActive(true);
        if (askDoubtsButton) askDoubtsButton.gameObject.SetActive(false);
        if (noDoubtsButton) noDoubtsButton.gameObject.SetActive(false);
        
        if (subtitleText) subtitleText.text = "Hold the Talk button to ask your question...";
        Debug.Log("[Learning] Doubts mode enabled - ready for voice input");
    }
    
    void OnFinishDoubts()
    {
        isAskingDoubts = false;
        if (doubtsPanel) doubtsPanel.SetActive(false);
        
        if (convaiNPC != null)
            convaiNPC.StopListening();
        
        EndSession();
    }
    
    void OnNoDoubts()
    {
        EndSession();
    }
    
    void EndSession()
    {
        Debug.Log("[Learning] Session ended");
        WorldManager.Instance?.AddKnowledgePoints(10);
        
        // Return to game world
        if (ARSessionManager.Instance != null)
        {
            ARSessionManager.Instance.ReturnToWorld();
        }
        else
        {
            // Fallback - load previous scene
            string currentWorld = WorldManager.Instance?.CurrentWorld.ToString() ?? "SciFi";
            UnityEngine.SceneManagement.SceneManager.LoadScene(currentWorld);
        }
    }
    
    // For external talk button control
    public void StartListening()
    {
        if (convaiNPC != null)
        {
            convaiNPC.StartListening();
            Debug.Log("[Learning] Started listening for voice");
        }
    }
    
    public void StopListening()
    {
        if (convaiNPC != null)
        {
            convaiNPC.StopListening();
            Debug.Log("[Learning] Stopped listening");
        }
    }
}
