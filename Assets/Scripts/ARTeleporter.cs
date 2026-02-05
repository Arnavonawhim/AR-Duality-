using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ARTeleporter : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject arPromptUI;
    [SerializeField] private Button enterARButton;
    
    [Header("Scene Settings")]
    [SerializeField] private string arSceneName = "ARScene";
    
    private bool playerInRange = false;
    private CharacterController2D playerController;
    
    void Start()
    {
        if (arPromptUI != null) arPromptUI.SetActive(false);
        if (enterARButton != null) enterARButton.onClick.AddListener(OnEnterARButtonPressed);
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            playerController = other.GetComponent<CharacterController2D>();
            
            if (arPromptUI != null) arPromptUI.SetActive(true);
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            playerController = null;
            
            if (arPromptUI != null) arPromptUI.SetActive(false);
        }
    }
    
    void OnEnterARButtonPressed()
    {
        if (playerInRange && playerController != null)
        {
            if (ARDataManager.Instance != null)
            {
                ARDataManager.Instance.SetVirtualStartPosition(playerController.transform.position);
            }
            
            if (arPromptUI != null) arPromptUI.SetActive(false);
            
            SceneManager.LoadScene(arSceneName);
        }
    }
}