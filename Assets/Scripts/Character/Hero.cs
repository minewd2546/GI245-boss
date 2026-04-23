using UnityEngine;

public class Hero : Character
{
    [SerializeField] private int prefabId;
    public int PrefabID
    {
        get => prefabId;
        set => prefabId = value;
    }

    [SerializeField] private int exp;
    public int Exp
    {
        get { return exp; }
        set { exp = value; }
    }

    [SerializeField] private int level;
    public int Level
    {
        get { return level; }
        set { level = value; }
    }

    [SerializeField] private int nextExp;
    public int NextExp
    {
        get { return nextExp; }
        set { nextExp = value; }
    }

    [SerializeField] private int strength;
    public int Strength
    {
        get { return strength; }
        set { strength = value; }
    }

    [SerializeField] private int dexterity;
    public int Dexterity
    {
        get { return dexterity; }
        set { dexterity = value; }
    }

    [SerializeField] private int constitution;
    public int Constitution
    {
        get { return constitution; }
        set { constitution = value; }
    }

    [SerializeField] private int intelligence;
    public int Intelligence
    {
        get { return intelligence; }
        set { intelligence = value; }
    }

    [SerializeField] private int wisdom;
    public int Wisdom
    {
        get { return wisdom; }
        set { wisdom = value; }
    }

    [SerializeField] private int charisma;
    public int Charisma
    {
        get { return charisma; }
        set { charisma = value; }
    }

    public bool AddItemToInventory(Item item)
    {
        if (item == null || inventoryItems == null)
            return false;

        for (int i = 0; i < 16 && i < inventoryItems.Length; i++)
        {
            if (inventoryItems[i] == null)
            {
                inventoryItems[i] = item;
                return true;
            }
        }

        return false;
    }

    public void SaveItemInInventory(Item item)
    {
        AddItemToInventory(item);
    }

    private void EnsureProgressionDefaults()
    {
        if (level <= 0)
            level = 1;

        if (nextExp <= 0)
            nextExp = 30;

        if (strength <= 0)
            strength = 1;

        if (dexterity <= 0)
            dexterity = 1;

        if (constitution <= 0)
            constitution = 1;

        if (intelligence <= 0)
            intelligence = 1;

        if (wisdom <= 0)
            wisdom = 1;

        if (charisma <= 0)
            charisma = 1;
    }

    public void ReceiveExp(int value)
    {
        if (value <= 0)
            return;

        exp += value;
        CheckLevel();
    }

    private void UpdateStat()
    {
        EnsureProgressionDefaults();

        attackDamage = 3 + Mathf.Max(0, strength - 1);
        defensePower = Mathf.Max(0, dexterity - 1);
        maxHP = 100 + Mathf.Max(0, constitution - 1) * 5;

        if (curHP > maxHP)
            curHP = maxHP;
    }

    private void LevelUpOnce()
    {
        level++;
        nextExp += 30;

        strength += 1;
        dexterity += 1;
        constitution += 1;
        intelligence += 1;
        wisdom += 1;
        charisma += 1;

        UpdateStat();
        curHP = maxHP;
        UnlockMagicByLevel();
    }

    private void CheckLevel()
    {
        while (exp >= nextExp)
            LevelUpOnce();

        if (uiManager != null)
        {
            uiManager.RefreshSelectedHeroPanel();
            uiManager.ShowMagicToggles();
        }
    }

    private void UnlockMagicByLevel()
    {
        if (vfxManager == null || vfxManager.MagicData == null)
            return;

        switch (level)
        {
            case 5:
                AddMagicIfMissing(0);
                break;
            case 10:
                AddMagicIfMissing(1);
                break;
        }
    }

    private void AddMagicIfMissing(int magicId)
    {
        if (magicId < 0 || magicId >= vfxManager.MagicData.Length || vfxManager.MagicData[magicId] == null)
            return;

        for (int i = 0; i < magicSkills.Count; i++)
        {
            if (magicSkills[i] != null && magicSkills[i].ID == vfxManager.MagicData[magicId].id)
                return;
        }

        magicSkills.Add(new Magic(vfxManager.MagicData[magicId]));
    }

    public void SaveToData(HeroData data)
    {
        if (data == null)
            return;

        data.prefabId = prefabId;
        data.inParty = true;
        data.charName = charName;
        data.curHP = curHP;
        data.maxHP = maxHP;
        data.attackDamage = attackDamage;
        data.defensePower = defensePower;
        data.exp = exp;
        data.level = level;
        data.nextExp = nextExp;
        data.strength = strength;
        data.dexterity = dexterity;
        data.constitution = constitution;
        data.intelligence = intelligence;
        data.wisdom = wisdom;
        data.charisma = charisma;
        data.mainWeapon = mainWeapon;
        data.shield = shield;

        if (inventoryItems == null)
            data.inventoryItems = new Item[InventoryManager.MAXSLOT];
        else
            data.inventoryItems = (Item[])inventoryItems.Clone();
    }

    public void LoadFromData(HeroData data)
    {
        if (data == null)
            return;

        prefabId = data.prefabId;
        charName = string.IsNullOrEmpty(data.charName) ? charName : data.charName;
        maxHP = Mathf.Max(1, data.maxHP);
        curHP = Mathf.Clamp(data.curHP, 0, maxHP);
        attackDamage = data.attackDamage;
        defensePower = data.defensePower;
        exp = data.exp;
        level = Mathf.Max(1, data.level);
        nextExp = Mathf.Max(30, data.nextExp);
        strength = data.strength;
        dexterity = data.dexterity;
        constitution = data.constitution;
        intelligence = data.intelligence;
        wisdom = data.wisdom;
        charisma = data.charisma;
        inventoryItems = data.inventoryItems != null ? (Item[])data.inventoryItems.Clone() : new Item[InventoryManager.MAXSLOT];
        mainWeapon = data.mainWeapon;
        shield = data.shield;

        UpdateStat();
        RestoreEquipmentFromInventory();
        curHP = Mathf.Clamp(curHP, 0, maxHP);
    }

    private void RestoreEquipmentFromInventory()
    {
        Item weaponToEquip = inventoryItems != null && inventoryItems.Length > 16 ? inventoryItems[16] ?? mainWeapon : mainWeapon;
        Item shieldToEquip = inventoryItems != null && inventoryItems.Length > 17 ? inventoryItems[17] ?? shield : shield;

        mainWeapon = null;
        shield = null;

        if (weaponObj != null)
            Destroy(weaponObj);

        if (shieldObj != null)
            Destroy(shieldObj);

        if (weaponToEquip != null)
        {
            if (inventoryItems != null && inventoryItems.Length > 16)
                inventoryItems[16] = weaponToEquip;

            EquipWeapon(weaponToEquip);
        }

        if (shieldToEquip != null)
        {
            if (inventoryItems != null && inventoryItems.Length > 17)
                inventoryItems[17] = shieldToEquip;

            EquipShield(shieldToEquip);
        }
    }

    protected void WalkToNPCUpdate()
    {
        if (curCharTarget == null)
        {
            SetState(CharState.Idle);
            return;
        }

        float distance = Vector3.Distance(transform.position, curCharTarget.transform.position);

        if (distance <= 2f)
        {
            navAgent.isStopped = true;
            SetState(CharState.Idle);

            Npc npc = curCharTarget.GetComponent<Npc>();
            Hero hero = curCharTarget.GetComponent<Hero>();

            if (npc != null)
            {
                if (npc.IsShopKeeper)
                    uiManager.PrepareShopPanel(npc);
                else
                    uiManager.PrepareDialogueBox(npc);
            }
            else if (hero != null && uiManager != null)
            {
                uiManager.PrepareHeroJoinBox(hero);
            }
        }
    }

    public override void CharInit(VFXManager vfxM, UIManager uiM, InventoryManager invM, PartyManager partyM)
    {
        base.CharInit(vfxM, uiM, invM, partyM);
        EnsureProgressionDefaults();
        UpdateStat();
    }

    void Update()
    {
        switch (state)
        {
            case CharState.Walk:
                WalkUpdate();
                break;
            case CharState.WalkToEnemy:
                WalkToEnemyUpdate();
                break;
            case CharState.Attack:
                AttackUpdate();
                break;
            case CharState.WalkToMagicCast:
                WalkToMagicCastUpdate();
                break;
            case CharState.WalkToNPC:
                WalkToNPCUpdate();
                break;
        }
    }
}
