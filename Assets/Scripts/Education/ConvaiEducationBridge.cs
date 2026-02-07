using UnityEngine;

public class ConvaiEducationBridge : MonoBehaviour
{
    [Header("Convai Settings")]
    [SerializeField] private string characterId = "";
    
    [Header("References")]
    [SerializeField] private AREducationController educationController;
    
    [Header("Settings")]
    [SerializeField] private bool useVoiceInput = true;
    [SerializeField] private bool debugMode = true;
    
    // Add Convai SDK components here after importing
    // private ConvaiNPC convaiNPC;
    
    private bool isInitialized = false;
    private bool isListening = false;
    
    public bool IsInitialized => isInitialized;
    public bool IsListening => isListening;
    
    void Start() => InitializeConvai();
    
    public void InitializeConvai()
    {
        // After importing Convai SDK, uncomment:
        // convaiNPC = GetComponent<ConvaiNPC>();
        // convaiNPC.OnUserTranscript += OnUserSpeech;
        // convaiNPC.OnCharacterResponse += OnCharacterResponse;
        
        isInitialized = true;
        if (debugMode) Debug.Log($"[ConvaiBridge] Ready with character: {characterId}");
    }
    
    public void SetCharacter(string newId)
    {
        characterId = newId;
        if (isInitialized) InitializeConvai();
    }
    
    public void StartListening()
    {
        if (!useVoiceInput) return;
        isListening = true;
        // convaiNPC?.StartRecording();
        if (debugMode) Debug.Log("[ConvaiBridge] Listening...");
    }
    
    public void StopListening()
    {
        if (!isListening) return;
        isListening = false;
        // convaiNPC?.StopRecording();
    }
    
    public void SendTextInput(string text)
    {
        if (string.IsNullOrEmpty(text)) return;
        // convaiNPC?.SendTextInput(text);
        if (debugMode) Debug.Log($"[ConvaiBridge] User: {text}");
        OnUserSpeech(text);
    }
    
    private void OnUserSpeech(string transcript)
    {
        if (educationController != null)
            educationController.ValidateAnswer(transcript);
    }
    
    private void OnCharacterResponse(string response)
    {
        if (debugMode) Debug.Log($"[ConvaiBridge] AI: {response}");
    }
    
    public void SpeakText(string text)
    {
        // convaiNPC?.Speak(text);
        if (debugMode) Debug.Log($"[ConvaiBridge] Speaking: {text}");
    }
}
