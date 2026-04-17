using UnityEngine;

[CreateAssetMenu(fileName = "HeroData", menuName = "Scriptable Objects/HeroData")]
public class HeroData : ScriptableObject
{
    public bool inParty;
    public int prefabId;
    public string charName;
    public int curHP;
    public int maxHP;
    public int attackDamage;
    public int defensePower;
    public int exp;
    public int level;
    public int nextExp;
    public int strength;
    public int dexterity;
    public int constitution;
    public int intelligence;
    public int wisdom;
    public int charisma;
    public Item[] inventoryItems = new Item[InventoryManager.MAXSLOT];
    public Item mainWeapon;
    public Item shield;

    public void ResetData()
    {
        inParty = false;
        prefabId = 0;
        charName = "";
        curHP = 0;
        maxHP = 0;
        attackDamage = 0;
        defensePower = 0;
        exp = 0;
        level = 0;
        nextExp = 0;
        strength = 0;
        dexterity = 0;
        constitution = 0;
        intelligence = 0;
        wisdom = 0;
        charisma = 0;
        inventoryItems = new Item[InventoryManager.MAXSLOT];
        mainWeapon = null;
        shield = null;
    }
}
