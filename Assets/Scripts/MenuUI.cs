using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuUI : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("ARScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
