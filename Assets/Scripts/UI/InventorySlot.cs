using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    [SerializeField]
    private int id;
    public int ID
    {
        get { return id; }
        set { id = value; }
    }

    [SerializeField]
    private ItemType itemType;
    public ItemType ItemType
    {
        get { return itemType; }
        set { itemType = value; }
    }

    [SerializeField]
    private InventoryManager inventoryManager;

    void Start()
    {
        inventoryManager = InventoryManager.instance;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null)
            return;

        GameObject objA = eventData.pointerDrag;
        ItemDrag itemDragA = objA.GetComponent<ItemDrag>();

        if (itemDragA == null || itemDragA.IconParent == null)
            return;

        InventorySlot slotA = itemDragA.IconParent.GetComponent<InventorySlot>();

        if (slotA == null)
            return;

        if (itemType == ItemType.Shield || itemType == ItemType.Weapon)
        {
            if (itemDragA.Item.Type != itemType)
                return;
        }

        if (transform.childCount > 0)
        {
            GameObject objB = transform.GetChild(0).gameObject;
            ItemDrag itemDragB = objB.GetComponent<ItemDrag>();

            if (itemDragB != null)
            {
                if (slotA.ItemType == ItemType.Shield || slotA.ItemType == ItemType.Weapon)
                {
                    if (itemDragB.Item.Type != slotA.ItemType)
                        return;
                }

                inventoryManager.RemoveItemInBag(slotA.ID);

                itemDragB.transform.SetParent(itemDragA.IconParent);
                itemDragB.transform.localPosition = Vector3.zero;
                itemDragB.IconParent = itemDragA.IconParent;
                inventoryManager.SaveItemInBag(slotA.ID, itemDragB.Item);

                inventoryManager.RemoveItemInBag(id);
            }
        }
        else
        {
            inventoryManager.RemoveItemInBag(slotA.ID);
        }

        itemDragA.IconParent = transform;
        itemDragA.transform.SetParent(transform);
        itemDragA.transform.localPosition = Vector3.zero;
        inventoryManager.SaveItemInBag(id, itemDragA.Item);
    }
}
