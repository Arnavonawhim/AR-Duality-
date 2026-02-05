using UnityEngine;
using UnityEngine.SceneManagement;

public class ARTeleporter : MonoBehaviour
{
    public GameObject promptUI;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            promptUI.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            promptUI.SetActive(false);
        }
    }

    public void EnterARWorld()
    {
        PlayerController player = FindObjectOfType<PlayerController>();
        if (GameDataManager.Instance != null)
        {
            GameDataManager.Instance.EnterAR(player.transform.position);
        }
        SceneManager.LoadScene("ARScene");
    }
}
