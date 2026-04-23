using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private string selectCharScene = "SelectChar";

    public void StartNewGame()
    {
        Settings.SelectedHeroPrefabID = 0;
        Settings.PartyMoney = Settings.StartingMoney;
        Settings.IsNewGame = true;
        Settings.IsWarping = false;
        SceneManager.LoadScene(selectCharScene);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
