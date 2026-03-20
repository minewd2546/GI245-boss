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

        // ลบ Item A ออกจากช่องเดิม
        inventoryManager.RemoveItemInBag(slotA.ID);

        // ถ้าช่องปลายทางมีของอยู่แล้ว ให้สลับกลับไปช่องเดิม
        if (transform.childCount > 0)
        {
            GameObject objB = transform.GetChild(0).gameObject;
            ItemDrag itemDragB = objB.GetComponent<ItemDrag>();

            if (itemDragB != null)
            {
                itemDragB.transform.SetParent(itemDragA.IconParent);
                itemDragB.transform.localPosition = Vector3.zero;
                itemDragB.IconParent = itemDragA.IconParent;
                inventoryManager.SaveItemInBag(slotA.ID, itemDragB.Item);
            }
        }

        // ย้าย Item A มาช่องนี้
        itemDragA.IconParent = transform;
        itemDragA.transform.SetParent(transform);
        itemDragA.transform.localPosition = Vector3.zero;
        inventoryManager.SaveItemInBag(id, itemDragA.Item);
    }
}