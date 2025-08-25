using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadSceneAsync("Map");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
