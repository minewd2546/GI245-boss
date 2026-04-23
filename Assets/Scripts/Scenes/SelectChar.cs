using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectChar : MonoBehaviour
{
    [SerializeField] private Image charImage;
    [SerializeField] private TMP_Text charNameText;

    [Header("Stat")]
    [SerializeField] private TMP_Text strengthText;
    [SerializeField] private TMP_Text dexterityText;
    [SerializeField] private TMP_Text constitutionText;
    [SerializeField] private TMP_Text intelligenceText;
    [SerializeField] private TMP_Text wisdomText;
    [SerializeField] private TMP_Text charismaText;

    [SerializeField] private GameObject[] heroPrefabs;
    [SerializeField] private string villageScene = "VillageScene";

    [SerializeField] private int curId;

    private const string MainMenuScene = "MainMenu";

    void Start()
    {
        ShowCharacter();
    }

    void OnValidate()
    {
        ShowCharacter();
    }

    private void ShowCharacter()
    {
        if (heroPrefabs == null || heroPrefabs.Length == 0)
            return;

        curId = Mathf.Clamp(curId, 0, heroPrefabs.Length - 1);
        Hero hero = GetHero(curId);
        if (hero == null)
            return;

        if (charImage != null)
        {
            charImage.sprite = hero.AvatarPic;

            if (hero.AvatarPic == null)
                Debug.LogWarning($"{hero.gameObject.name} has no Avatar Pic assigned in the Hero component.");
        }

        if (charNameText != null)
        {
            string displayName = string.IsNullOrEmpty(hero.CharName) ? hero.gameObject.name : hero.CharName;
            charNameText.text = displayName;

            if (string.IsNullOrEmpty(hero.CharName))
                Debug.LogWarning($"{hero.gameObject.name} has no Char Name assigned in the Hero component. Using prefab name instead.");
        }

        if (strengthText != null)
            strengthText.text = hero.Strength.ToString();
        if (dexterityText != null)
            dexterityText.text = hero.Dexterity.ToString();
        if (constitutionText != null)
            constitutionText.text = hero.Constitution.ToString();
        if (intelligenceText != null)
            intelligenceText.text = hero.Intelligence.ToString();
        if (wisdomText != null)
            wisdomText.text = hero.Wisdom.ToString();
        if (charismaText != null)
            charismaText.text = hero.Charisma.ToString();
    }

    private Hero GetHero(int index)
    {
        if (heroPrefabs == null || index < 0 || index >= heroPrefabs.Length || heroPrefabs[index] == null)
            return null;

        return heroPrefabs[index].GetComponent<Hero>();
    }

    private bool CanLoadScene(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("Scene name is empty on SelectChar.");
            return false;
        }

        if (Application.CanStreamedLevelBeLoaded(sceneName))
            return true;

        Debug.LogError($"Scene '{sceneName}' is not in Build Settings or the name is wrong.");
        return false;
    }

    public void SelectNextChar()
    {
        if (heroPrefabs == null || heroPrefabs.Length == 0)
            return;

        curId = (curId + 1) % heroPrefabs.Length;
        ShowCharacter();
    }

    public void SelectPreviousChar()
    {
        if (heroPrefabs == null || heroPrefabs.Length == 0)
            return;

        curId--;
        if (curId < 0)
            curId = heroPrefabs.Length - 1;

        ShowCharacter();
    }

    public void BackToMainMenu()
    {
        if (CanLoadScene(MainMenuScene))
            SceneManager.LoadScene(MainMenuScene);
    }

    public void BeginGame()
    {
        if (GetHero(curId) == null)
        {
            Debug.LogError($"Hero Prefabs Element {curId} is missing a Hero component.");
            return;
        }

        if (!CanLoadScene(villageScene))
            return;

        Settings.SelectedHeroPrefabID = curId;
        Settings.PartyMoney = Settings.StartingMoney;
        Settings.IsNewGame = true;
        Settings.IsWarping = false;
        SceneManager.LoadScene(villageScene);
    }
}
