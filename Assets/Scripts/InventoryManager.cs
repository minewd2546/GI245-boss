using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] itemPrefabs;
    public GameObject[] ItemPrefabs
    {
        get { return itemPrefabs; }
        set { itemPrefabs = value; }
    }

    [SerializeField]
    private ItemData[] itemData;
    public ItemData[] ItemData
    {
        get { return itemData; }
        set { itemData = value; }
    }

    public const int MAXSLOT = 18;
    public static InventoryManager instance;

    public bool AddItem(Character character, int id)
    {
        Item item = new Item(itemData[id]);
        return AddItem(character, item);
    }

    public bool AddItem(Character character, Item item)
    {
        if (character == null || item == null)
            return false;

        for (int i = 0; i < character.InventoryItems.Length; i++)
        {
            if (character.InventoryItems[i] == null)
            {
                character.InventoryItems[i] = item;
                return true;
            }
        }

        Debug.Log("Inventory Full");
        return false;
    }

    public void SaveItemInBag(int index, Item item)
    {
        if (PartyManager.instance.SelectChars.Count == 0)
            return;

        PartyManager.instance.SelectChars[0].InventoryItems[index] = item;

        switch (index)
        {
            case 16:
                PartyManager.instance.SelectChars[0].EquipWeapon(item);
                break;
            case 17:
                PartyManager.instance.SelectChars[0].EquipShield(item);
                break;
        }
    }

    public void RemoveItemInBag(int index)
    {
        if (PartyManager.instance.SelectChars.Count == 0)
            return;

        PartyManager.instance.SelectChars[0].InventoryItems[index] = null;

        switch (index)
        {
            case 16:
                PartyManager.instance.SelectChars[0].UnEquipWeapon();
                break;
            case 17:
                PartyManager.instance.SelectChars[0].UnEquipShield();
                break;
        }
    }

    public void DrinkConsumableItem(Item item, int slotId)
    {
        string s = string.Format("Drink: {0}", item.ItemName);
        Debug.Log(s);

        if (PartyManager.instance.SelectChars.Count > 0)
        {
            PartyManager.instance.SelectChars[0].Recover(item.Power);
            RemoveItemInBag(slotId);
        }
    }

    private void SpawnDropItem(Item item, Vector3 pos)
    {
        int id;

        switch (item.Type)
        {
            case ItemType.Consumable:
                id = 1;
                break;
            default:
                id = 0;
                break;
        }

        GameObject itemPrefab = ItemPrefabs[id];
        Vector3 dropPos = pos;

        if (itemPrefab != null && itemPrefab.name == "SM_Item_Bag_Large")
            dropPos += new Vector3(0f, 0.5f, 0f);

        GameObject itemObj = Instantiate(itemPrefab, dropPos, Quaternion.identity);
        ItemPick itemPick = itemObj.GetComponent<ItemPick>();
        if (itemPick == null)
            itemPick = itemObj.AddComponent<ItemPick>();

        SetLayerRecursively(itemObj, LayerMask.NameToLayer("Item"));
        if (itemObj.GetComponentInChildren<Collider>() == null)
            itemObj.AddComponent<SphereCollider>();

        itemPick.Init(item, instance, PartyManager.instance);
    }

    public void SpawnDropInventory(Item[] items, Vector3 pos)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] != null)
            {
                Vector2 randomOffset2D = Random.insideUnitCircle * 0.5f;
                Vector3 randomOffset = new Vector3(randomOffset2D.x, 0f, randomOffset2D.y);
                SpawnDropItem(items[i], pos + randomOffset);
            }
        }
    }

    private void SetLayerRecursively(GameObject obj, int layer)
    {
        if (layer < 0)
            return;

        obj.layer = layer;

        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }

    void Awake()
    {
        instance = this;
    }
}
