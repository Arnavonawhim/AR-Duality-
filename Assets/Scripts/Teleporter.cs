using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Teleporter : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject promptUI;
    [SerializeField] private Button enterARButton;
    [SerializeField] private Button cancelButton;
    
    [Header("Settings")]
    [SerializeField] private string arSceneName = "ARScene";
    
    private bool playerInRange;
    private PlayerController player;

    void Start()
    {
        if (promptUI != null) promptUI.SetActive(false);
        
        if (enterARButton != null)
        {
            enterARButton.onClick.AddListener(OnEnterARPressed);
        }
        
        if (cancelButton != null)
        {
            cancelButton.onClick.AddListener(OnCancelPressed);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        PlayerController p = other.GetComponent<PlayerController>();
        if (p != null)
        {
            player = p;
            playerInRange = true;
            if (promptUI != null) promptUI.SetActive(true);
            Debug.Log("Player entered teleporter range");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            playerInRange = false;
            if (promptUI != null) promptUI.SetActive(false);
            Debug.Log("Player left teleporter range");
        }
    }

    public void OnEnterARPressed()
    {
        Debug.Log("Enter AR button pressed!");
        
        if (GameDataManager.Instance != null && player != null)
        {
            GameDataManager.Instance.SetPlayerPositionBeforeAR(player.transform.position);
        }
        
        SceneManager.LoadScene(arSceneName);
    }

    public void OnCancelPressed()
    {
        if (promptUI != null) promptUI.SetActive(false);
    }
}

