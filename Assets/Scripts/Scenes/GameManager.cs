using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Hero[] heroPrefabs;
    public Hero[] HeroPrefabs => heroPrefabs;

    [SerializeField] private Vector3 playerStartPosition = new Vector3(27.6895f, 9.998167f, 31.66f);

    public static GameManager instance;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        TrySetupScene();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        TrySetupScene();
    }

    private void TrySetupScene()
    {
        if (PartyManager.instance == null)
            return;

        if (Settings.IsWarping)
        {
            PartyManager.instance.LoadAllHeroData();
            PartyManager.instance.Money = Settings.PartyMoney;

            MapManager mapManager = FindObjectOfType<MapManager>();
            if (mapManager != null)
                mapManager.MovePartyToEnterPoint(Settings.TargetEnterPointID);

            Settings.IsWarping = false;
            UIManager.instance?.MapToggleAvatar();
            UIManager.instance?.RefreshSelectedHeroPanel();
            UIManager.instance?.ShowMagicToggles();
            return;
        }

        if (Settings.IsNewGame)
        {
            PartyManager.instance.ClearParty(true);

            Hero hero = SpawnHeroFromPrefabID(Settings.SelectedHeroPrefabID, playerStartPosition);
            if (hero != null)
                PartyManager.instance.AddMember(hero);

            Settings.IsNewGame = false;
        }
    }

    private Hero SpawnHeroFromPrefabID(int prefabID)
    {
        return SpawnHeroFromPrefabID(prefabID, GetEnterPointSpawnPosition());
    }

    private Hero SpawnHeroFromPrefabID(int prefabID, Vector3 spawnPos)
    {
        if (heroPrefabs == null || prefabID < 0 || prefabID >= heroPrefabs.Length)
            return null;

        Hero prefab = heroPrefabs[prefabID];
        if (prefab == null)
            return null;

        Hero hero = Instantiate(prefab, spawnPos, Quaternion.identity);
        hero.PrefabID = prefabID;
        return hero;
    }

    private Vector3 GetEnterPointSpawnPosition()
    {
        MapManager mapManager = FindObjectOfType<MapManager>();
        return mapManager != null ? mapManager.GetEnterPointPosition(0) : Vector3.zero;
    }

    public Hero SpawnHeroFromData(HeroData data)
    {
        if (data == null)
            return null;

        Hero hero = SpawnHeroFromPrefabID(data.prefabId);
        return hero;
    }

    public void Warp(string targetScene, int targetEnterPointID)
    {
        if (PartyManager.instance != null)
        {
            Settings.PartyMoney = PartyManager.instance.Money;
            PartyManager.instance.SaveAllHeroData();
        }

        Settings.IsWarping = true;
        Settings.TargetEnterPointID = targetEnterPointID;
        SceneManager.LoadScene(targetScene);
    }
}
