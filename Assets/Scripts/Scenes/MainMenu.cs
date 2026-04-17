using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private string selectCharScene = "SelectChar";

    public void StartNewGame()
    {
        Settings.IsNewGame = true;
        Settings.IsWarping = false;
        SceneManager.LoadScene(selectCharScene);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
