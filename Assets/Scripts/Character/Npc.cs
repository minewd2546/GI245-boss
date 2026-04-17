using System.Collections.Generic;
using UnityEngine;

public class Npc : Character
{
    [SerializeField] private List<Quest> questToGive = new List<Quest>();
    public List<Quest> QuestToGive
    {
        get => questToGive;
        set => questToGive = value;
    }

    [SerializeField] private bool isShopKeeper;
    public bool IsShopKeeper => isShopKeeper;

    [SerializeField] private int money = 1000;
    public int Money
    {
        get => money;
        set => money = Mathf.Max(0, value);
    }

    [SerializeField] private Item[] shopItems = new Item[InventoryManager.MAXSLOT];
    public Item[] ShopItems => shopItems;

    private void EnsureShopSlots()
    {
        if (shopItems == null || shopItems.Length != InventoryManager.MAXSLOT)
            shopItems = new Item[InventoryManager.MAXSLOT];
    }

    public bool AddShopItem(Item item)
    {
        if (item == null)
            return false;

        EnsureShopSlots();

        for (int i = 0; i < shopItems.Length; i++)
        {
            if (shopItems[i] == null)
            {
                shopItems[i] = item;
                return true;
            }
        }

        return false;
    }

    public bool RemoveShopItem(Item item)
    {
        if (item == null)
            return false;

        EnsureShopSlots();

        for (int i = 0; i < shopItems.Length; i++)
        {
            if (shopItems[i] != null && shopItems[i].ID == item.ID)
            {
                shopItems[i] = null;
                return true;
            }
        }

        return false;
    }

    public Quest CheckQuestList(QuestStatus status)
    {
        foreach (Quest quest in questToGive)
        {
            if (quest.Status == status)
                return quest;
        }

        return null;
    }

    void Awake()
    {
        EnsureShopSlots();
    }
}
