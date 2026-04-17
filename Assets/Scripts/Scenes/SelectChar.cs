using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectChar : MonoBehaviour
{
    [SerializeField] private Hero[] heroPrefabs;
    [SerializeField] private Image charImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text[] statTexts;
    [SerializeField] private string villageScene = "VillageScene";

    [SerializeField] private int curIndex;

    void Start()
    {
        ShowCharacter();
    }

    private void ShowCharacter()
    {
        if (heroPrefabs == null || heroPrefabs.Length == 0)
            return;

        curIndex = Mathf.Clamp(curIndex, 0, heroPrefabs.Length - 1);
        Hero hero = heroPrefabs[curIndex];

        if (charImage != null)
            charImage.sprite = hero.AvatarPic;

        if (nameText != null)
            nameText.text = hero.CharName;

        if (statTexts == null || statTexts.Length < 6)
            return;

        statTexts[0].text = $"STR: {hero.Strength}";
        statTexts[1].text = $"DEX: {hero.Dexterity}";
        statTexts[2].text = $"CON: {hero.Constitution}";
        statTexts[3].text = $"INT: {hero.Intelligence}";
        statTexts[4].text = $"WIS: {hero.Wisdom}";
        statTexts[5].text = $"CHA: {hero.Charisma}";
    }

    public void SelectNextChar()
    {
        if (heroPrefabs == null || heroPrefabs.Length == 0)
            return;

        curIndex = (curIndex + 1) % heroPrefabs.Length;
        ShowCharacter();
    }

    public void SelectPreviousChar()
    {
        if (heroPrefabs == null || heroPrefabs.Length == 0)
            return;

        curIndex--;
        if (curIndex < 0)
            curIndex = heroPrefabs.Length - 1;

        ShowCharacter();
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void BeginGame()
    {
        Settings.SelectedHeroPrefabID = curIndex;
        Settings.IsNewGame = true;
        Settings.IsWarping = false;
        SceneManager.LoadScene(villageScene);
    }
}
