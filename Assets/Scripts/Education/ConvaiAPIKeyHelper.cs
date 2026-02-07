using UnityEngine;

public class ConvaiAPIKeyHelper : MonoBehaviour
{
    [Header("Paste Your Convai API Key Here")]
    [SerializeField] private string convaiAPIKey = "PASTE_YOUR_API_KEY_HERE";
    
    void Awake()
    {
        if (!string.IsNullOrEmpty(convaiAPIKey) && convaiAPIKey != "PASTE_YOUR_API_KEY_HERE")
        {
            PlayerPrefs.SetString("Convai_API_Key", convaiAPIKey);
            PlayerPrefs.Save();
            Debug.Log("[Convai] API Key set successfully!");
        }
        else
        {
            Debug.LogWarning("[Convai] Please set your API key in ConvaiAPIKeyHelper!");
        }
    }
    
    [ContextMenu("Clear API Key")]
    void ClearKey()
    {
        PlayerPrefs.DeleteKey("Convai_API_Key");
        Debug.Log("[Convai] API Key cleared");
    }
}
