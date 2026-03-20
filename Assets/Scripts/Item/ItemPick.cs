using UnityEngine;

public class ItemPick : MonoBehaviour
{
    [SerializeField]
    private Item item;
    public Item Item
    {
        get { return item; }
    }

    private InventoryManager inventoryManager;
    private PartyManager partyManager;

    void Start()
    {
        inventoryManager = InventoryManager.instance;
        partyManager = PartyManager.instance;
    }

    public void Init(Item item, InventoryManager invManager, PartyManager ptyManager)
    {
        this.item = item;
        inventoryManager = invManager;
        partyManager = ptyManager;
    }

    private void PickUpItem(Character hero)
    {
        if (inventoryManager == null)
            inventoryManager = InventoryManager.instance;

        if (inventoryManager != null && inventoryManager.AddItem(hero, item))
            Destroy(gameObject);
    }

    public void TryPickUpSelectedHero()
    {
        if (partyManager == null)
            partyManager = PartyManager.instance;

        if (partyManager != null && partyManager.SelectChars.Count > 0)
            PickUpItem(partyManager.SelectChars[0]);
    }

    private void OnMouseDown()
    {
        Debug.Log("Pick Up");
        TryPickUpSelectedHero();
    }
}
