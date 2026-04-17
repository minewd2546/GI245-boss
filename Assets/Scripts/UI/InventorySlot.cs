using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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

    private ItemDrag FindItemDragChild()
    {
        ItemDrag[] drags = GetComponentsInChildren<ItemDrag>(true);
        for (int i = 0; i < drags.Length; i++)
        {
            if (drags[i].transform.parent == transform)
                return drags[i];
        }

        return null;
    }

    void Start()
    {
        inventoryManager = InventoryManager.instance;

        // Decorative UI inside a slot should not block drag/drop events.
        Graphic[] childGraphics = GetComponentsInChildren<Graphic>(true);
        for (int i = 0; i < childGraphics.Length; i++)
        {
            if (childGraphics[i].gameObject != gameObject && childGraphics[i].GetComponentInParent<ItemDrag>() == null)
                childGraphics[i].raycastTarget = false;
        }
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

        ItemDrag itemDragB = FindItemDragChild();

        if (itemDragB != null && itemDragB.gameObject != objA)
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
