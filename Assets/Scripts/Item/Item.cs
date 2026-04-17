using UnityEngine;

public enum ItemType
{
    Consumable,
    Equipment,
    Shield,
    Armor,
    Weapon,
    Ammo,
    Quest,
    Other
}

[System.Serializable]
public class Item
{
    [SerializeField] private int id;
    public int ID => id;

    [SerializeField] private string itemName;
    public string ItemName => itemName;

    [SerializeField] private ItemType type;
    public ItemType Type => type;

    [SerializeField] private Sprite icon;
    public Sprite Icon => icon;

    [SerializeField] private int power;
    public int Power => power;

    [SerializeField] private int prefabID;
    public int PrefabID => prefabID;

    [SerializeField] private int normalPrice;
    public int NormalPrice => normalPrice;

    public Item(ItemData data)
    {
        id = data.id;
        itemName = data.itemName;
        type = data.type;
        icon = data.icon;
        power = data.power;
        prefabID = data.prefabID;
        normalPrice = data.normalPrice;
    }
}
