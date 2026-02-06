using UnityEngine;
using UnityEngine.SceneManagement;

public class Teleporter : MonoBehaviour
{
    [SerializeField] private GameObject promptUI;
    [SerializeField] private string arSceneName = "ARScene";
    
    private bool playerInRange;
    private PlayerController player;

    void Start()
    {
        if (promptUI != null) promptUI.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        PlayerController p = other.GetComponent<PlayerController>();
        if (p != null)
        {
            player = p;
            playerInRange = true;
            if (promptUI != null) promptUI.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            playerInRange = false;
            if (promptUI != null) promptUI.SetActive(false);
        }
    }

    public void OnEnterARPressed()
    {
        if (!playerInRange) return;
        
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
