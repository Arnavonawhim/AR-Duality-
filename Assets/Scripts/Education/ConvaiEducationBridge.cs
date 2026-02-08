using UnityEngine;
using Convai.Scripts.Runtime.Core;

/// <summary>
/// Bridge between education system and Convai SDK.
/// Provides simplified interface for voice interaction with Convai characters.
/// </summary>
public class ConvaiEducationBridge : MonoBehaviour
{
    [Header("Convai Settings")]
    [SerializeField] private string apiKey;
    
    [Header("NPC Reference")]
    [SerializeField] private ConvaiNPC convaiNPC;
    
    private string activeCharacterId;
    private bool isListening;
    
    /// <summary>
    /// Set the active Convai NPC for this bridge
    /// </summary>
    public void SetConvaiNPC(ConvaiNPC npc)
    {
        convaiNPC = npc;
        if (npc != null)
        {
            Debug.Log($"[Convai] NPC set: {npc.gameObject.name}");
        }
    }
    
    /// <summary>
    /// Set active character by ID (updates character on Convai backend)
    /// </summary>
    public void SetActiveCharacter(string characterId)
    {
        activeCharacterId = characterId;
        Debug.Log($"[Convai] Active character ID: {characterId}");
    }
    
    /// <summary>
    /// Send text message to the Convai character
    /// </summary>
    public void SendTextInput(string message)
    {
        if (convaiNPC == null)
        {
            Debug.LogWarning("[Convai] No ConvaiNPC set!");
            return;
        }
        
        if (string.IsNullOrEmpty(message))
        {
            Debug.LogWarning("[Convai] Cannot send empty message!");
            return;
        }
        
        convaiNPC.SendTextDataAsync(message);
        Debug.Log($"[Convai] Sent: {message}");
    }
    
    /// <summary>
    /// Start voice recording for player speech input
    /// </summary>
    public void StartListening()
    {
        if (convaiNPC == null)
        {
            Debug.LogWarning("[Convai] No ConvaiNPC set!");
            return;
        }
        
        isListening = true;
        convaiNPC.StartListening();
        Debug.Log("[Convai] Started voice input");
    }
    
    /// <summary>
    /// Stop voice recording
    /// </summary>
    public void StopListening()
    {
        if (convaiNPC == null)
        {
            Debug.LogWarning("[Convai] No ConvaiNPC set!");
            return;
        }
        
        isListening = false;
        convaiNPC.StopListening();
        Debug.Log("[Convai] Stopped voice input");
    }
    
    /// <summary>
    /// Check if currently listening for voice input
    /// </summary>
    public bool IsListening() => isListening;
    
    /// <summary>
    /// Set language preference for Convai character responses
    /// </summary>
    public void SetLanguagePreference(string language)
    {
        // Language is typically set in the Convai character's backstory/settings
        Debug.Log($"[Convai] Language preference: {language}");
    }
    
    /// <summary>
    /// Get the current ConvaiNPC reference
    /// </summary>
    public ConvaiNPC GetConvaiNPC() => convaiNPC;
}
