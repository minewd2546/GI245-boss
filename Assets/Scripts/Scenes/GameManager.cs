using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Hero[] heroPrefabs;
    public Hero[] HeroPrefabs => heroPrefabs;

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

            MapManager mapManager = FindObjectOfType<MapManager>();
            if (mapManager != null)
                mapManager.MovePartyToEnterPoint(Settings.TargetEnterPointID);

            Settings.IsWarping = false;
            UIManager.instance?.MapToggleAvatar();
            UIManager.instance?.RefreshSelectedHeroPanel();
            UIManager.instance?.ShowMagicToggles();
            return;
        }

        if (Settings.IsNewGame && PartyManager.instance.Members.Count == 0)
        {
            Hero hero = SpawnHeroFromPrefabID(Settings.SelectedHeroPrefabID);
            if (hero != null)
            {
                PartyManager.instance.AddMember(hero);

                MapManager mapManager = FindObjectOfType<MapManager>();
                if (mapManager != null)
                    mapManager.MovePartyToEnterPoint(0);
            }

            Settings.IsNewGame = false;
        }
    }

    private Hero SpawnHeroFromPrefabID(int prefabID)
    {
        if (heroPrefabs == null || prefabID < 0 || prefabID >= heroPrefabs.Length)
            return null;

        Hero prefab = heroPrefabs[prefabID];
        if (prefab == null)
            return null;

        Vector3 spawnPos = Vector3.zero;
        MapManager mapManager = FindObjectOfType<MapManager>();
        if (mapManager != null)
            spawnPos = mapManager.GetEnterPointPosition(0);

        Hero hero = Instantiate(prefab, spawnPos, Quaternion.identity);
        hero.PrefabID = prefabID;
        return hero;
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
            PartyManager.instance.SaveAllHeroData();

        Settings.IsWarping = true;
        Settings.TargetEnterPointID = targetEnterPointID;
        SceneManager.LoadScene(targetScene);
    }
}
