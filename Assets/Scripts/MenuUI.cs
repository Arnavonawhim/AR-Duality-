using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuUI : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("levels");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
