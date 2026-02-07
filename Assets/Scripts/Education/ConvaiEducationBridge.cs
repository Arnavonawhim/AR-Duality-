using UnityEngine;

public class ConvaiEducationBridge : MonoBehaviour
{
    [Header("Convai Settings")]
    [SerializeField] private string apiKey;
    
    private string activeCharacterId;
    private bool isListening;
    
    // Convai SDK components (add when SDK works)
    // private ConvaiNPC convaiNPC;
    
    public void SetActiveCharacter(string characterId)
    {
        activeCharacterId = characterId;
        // Configure Convai NPC with character ID
        Debug.Log($"[Convai] Active character: {characterId}");
    }
    
    public void SendTextInput(string message)
    {
        if (string.IsNullOrEmpty(activeCharacterId))
        {
            Debug.LogWarning("[Convai] No active character set!");
            return;
        }
        
        // Send to Convai API
        Debug.Log($"[Convai] Sending: {message}");
        
        // TODO: Use Convai SDK to send message
        // convaiNPC.SendTextData(message);
    }
    
    public void StartListening()
    {
        isListening = true;
        Debug.Log("[Convai] Started voice input");
        
        // TODO: Enable Convai microphone input
        // convaiNPC.StartRecording();
    }
    
    public void StopListening()
    {
        isListening = false;
        Debug.Log("[Convai] Stopped voice input");
        
        // TODO: Disable Convai microphone input
        // convaiNPC.StopRecording();
    }
    
    public void SetLanguagePreference(string language)
    {
        // Set Convai language preference (English/Hindi)
        Debug.Log($"[Convai] Language set to: {language}");
    }
    
    // Callback from Convai when response received
    public void OnConvaiResponse(string response)
    {
        Debug.Log($"[Convai] Response: {response}");
    }
}
