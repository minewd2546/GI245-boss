using System.Collections.Generic;
using UnityEngine;

public class PartyManager : MonoBehaviour
{
    [SerializeField] private List<Character> members = new List<Character>();
    public List<Character> Members => members;

    [SerializeField] private List<Character> selectChars = new List<Character>();
    public List<Character> SelectChars => selectChars;

    [SerializeField] private List<Quest> questList = new List<Quest>();
    public List<Quest> QuestList => questList;

    [SerializeField] private HeroData[] heroData;
    public HeroData[] HeroDataList => heroData;

    [SerializeField] private int money = 1000;
    public int Money
    {
        get => money;
        set
        {
            money = Mathf.Max(0, value);
            Settings.PartyMoney = money;
        }
    }

    [SerializeField] private bool seedDefaultLoadout = true;
    [SerializeField] private bool loadedFromHeroData;

    public static PartyManager instance;

    public int FindIndexFromClass(Character hero)
    {
        return members.IndexOf(hero);
    }

    public bool HasMember(Hero hero)
    {
        return hero != null && members.Contains(hero);
    }

    public bool AddMember(Hero hero)
    {
        if (hero == null || members.Contains(hero) || members.Count >= 6)
            return false;

        hero.CharInit(VFXManager.instance, UIManager.instance, InventoryManager.instance, this);
        members.Add(hero);
        SeedDefaultMagic();
        SeedDefaultItems();
        SelectSingleHero(members.Count - 1);
        UIManager.instance?.MapToggleAvatar();
        return true;
    }

    public void ShareExpToParty(int exp)
    {
        foreach (Character member in members)
        {
            Hero hero = member as Hero;
            if (hero != null)
                hero.ReceiveExp(exp);
        }
    }

    public void HeroSelectMagicSkill(int i)
    {
        if (selectChars.Count <= 0)
            return;

        Character selected = selectChars[0];
        if (i < 0 || i >= selected.MagicSkills.Count)
            return;

        selected.IsMagicMode = true;
        selected.CurMagicCast = selected.MagicSkills[i];
    }

    public void SelectSingleHero(int i)
    {
        if (i < 0 || i >= members.Count)
            return;

        foreach (Character c in members)
            c.ToggleRingSelection(false);

        selectChars.Clear();
        selectChars.Add(members[i]);
        selectChars[0].ToggleRingSelection(true);

        UIManager.instance?.MapToggleAvatar();
        UIManager.instance?.ShowMagicToggles();
        UIManager.instance?.RefreshSelectedHeroPanel();
    }

    public void SelectSingleHeroByToggle(int i)
    {
        if (i < 0 || i >= members.Count)
            return;

        Character member = members[i];
        if (!selectChars.Contains(member))
            selectChars.Add(member);

        member.ToggleRingSelection(true);
        UIManager.instance?.ShowMagicToggles();
        UIManager.instance?.RefreshSelectedHeroPanel();
    }

    public void UnSelectSingleHeroByToggle(int i)
    {
        if (i < 0 || i >= members.Count)
            return;

        Character member = members[i];

        if (selectChars.Count <= 1)
        {
            UIManager.instance?.ForceAvatarToggle(i, true);
            return;
        }

        member.ToggleRingSelection(false);
        selectChars.Remove(member);
        UIManager.instance?.ShowMagicToggles();
        UIManager.instance?.RefreshSelectedHeroPanel();
    }

    public bool RemoveMemberFromParty(int index)
    {
        if (index <= 0 || index >= members.Count)
            return false;

        Character target = members[index];
        if (target == null)
            return false;

        target.ToggleRingSelection(false);
        selectChars.Remove(target);
        members.RemoveAt(index);

        if (selectChars.Count == 0 && members.Count > 0)
            SelectSingleHero(0);

        UIManager.instance?.MapToggleAvatar();
        UIManager.instance?.RefreshSelectedHeroPanel();
        return true;
    }

    public void ClearParty(bool destroyMemberObjects)
    {
        if (destroyMemberObjects)
        {
            foreach (Character member in members)
            {
                if (member != null)
                    Destroy(member.gameObject);
            }
        }

        members.Clear();
        selectChars.Clear();
    }

    public void SaveAllHeroData()
    {
        if (heroData == null || heroData.Length == 0)
            return;

        for (int i = 0; i < heroData.Length; i++)
        {
            if (heroData[i] != null)
                heroData[i].ResetData();
        }

        foreach (Character member in members)
        {
            Hero hero = member as Hero;
            if (hero == null)
                continue;

            int id = hero.PrefabID;
            if (id < 0 || id >= heroData.Length || heroData[id] == null)
                continue;

            hero.SaveToData(heroData[id]);
        }
    }

    public void LoadAllHeroData()
    {
        ClearParty(true);
        loadedFromHeroData = true;

        if (heroData == null || GameManager.instance == null)
            return;

        for (int i = 0; i < heroData.Length; i++)
        {
            HeroData data = heroData[i];
            if (data == null || !data.inParty)
                continue;

            Hero hero = GameManager.instance.SpawnHeroFromData(data);
            if (hero == null)
                continue;

            hero.CharInit(VFXManager.instance, UIManager.instance, InventoryManager.instance, this);
            hero.LoadFromData(data);
            members.Add(hero);
        }

        if (members.Count > 0)
            SelectSingleHero(0);
    }

    private void SeedDefaultMagic()
    {
        if (VFXManager.instance == null || VFXManager.instance.MagicData == null)
            return;

        if (members.Count > 0 && members[0].MagicSkills.Count == 0 && VFXManager.instance.MagicData.Length >= 3)
        {
            members[0].MagicSkills.Add(new Magic(VFXManager.instance.MagicData[0]));
            members[0].MagicSkills.Add(new Magic(VFXManager.instance.MagicData[2]));
        }

        if (members.Count > 1 && members[1].MagicSkills.Count == 0 && VFXManager.instance.MagicData.Length >= 4)
        {
            members[1].MagicSkills.Add(new Magic(VFXManager.instance.MagicData[1]));
            members[1].MagicSkills.Add(new Magic(VFXManager.instance.MagicData[3]));
        }
    }

    private void SeedDefaultItems()
    {
        if (!seedDefaultLoadout || InventoryManager.instance == null)
            return;

        if (members.Count > 0 && members[0].InventoryItems[0] == null)
        {
            InventoryManager.instance.AddItem(members[0], 0);
            InventoryManager.instance.AddItem(members[0], 1);
            InventoryManager.instance.AddItem(members[0], 11);
        }

        if (members.Count > 1 && members[1].InventoryItems[0] == null)
        {
            InventoryManager.instance.AddItem(members[1], 0);
            InventoryManager.instance.AddItem(members[1], 1);
            InventoryManager.instance.AddItem(members[1], 2);
            InventoryManager.instance.AddItem(members[1], 3);
            InventoryManager.instance.AddItem(members[1], 10);
            InventoryManager.instance.AddItem(members[1], 4);
            InventoryManager.instance.AddItem(members[1], 5);
            InventoryManager.instance.AddItem(members[1], 6);
            InventoryManager.instance.AddItem(members[1], 7);
            InventoryManager.instance.AddItem(members[1], 8);
            InventoryManager.instance.AddItem(members[1], 9);
        }
    }

    void Start()
    {
        foreach (Character c in members)
            c.CharInit(VFXManager.instance, UIManager.instance, InventoryManager.instance, this);

        if (members.Count > 0)
            SelectSingleHero(0);

        if (!loadedFromHeroData)
        {
            SeedDefaultMagic();
            SeedDefaultItems();
        }

        UIManager.instance?.MapToggleAvatar();
        UIManager.instance?.ShowMagicToggles();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M) && selectChars.Count > 0)
        {
            selectChars[0].IsMagicMode = true;

            if (selectChars[0].CurMagicCast == null && selectChars[0].MagicSkills.Count > 0)
                selectChars[0].CurMagicCast = selectChars[0].MagicSkills[0];
        }
    }

    void Awake()
    {
        instance = this;
        Money = Settings.PartyMoney;
    }
}
