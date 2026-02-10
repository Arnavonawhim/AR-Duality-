using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuUI : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("Scifii");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
