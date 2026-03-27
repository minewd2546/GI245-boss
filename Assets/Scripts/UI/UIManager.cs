using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private RectTransform selectionBox;
    public RectTransform SelectionBox { get { return selectionBox; } }

    public static UIManager instance;

    [SerializeField] private Toggle togglePauseUnpause;
    [SerializeField] private GameObject blackImage;
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private GameObject grayImage;
    [SerializeField] private GameObject itemDialog;
    [SerializeField] private GameObject itemUIPrefab;
    [SerializeField] private GameObject[] slots;
    [SerializeField] private ItemDrag curItemDrag;
    [SerializeField] private int curSlotId;

    [SerializeField] private Toggle[] toggleMagic;
    public Toggle[] ToggleMagic { get { return toggleMagic; } }

    [SerializeField] private int curToggleMagicID = -1;

    public void ToggleAI(bool isOn)
    {
        foreach (Character member in PartyManager.instance.Members)
        {
            AttackAI ai = member.gameObject.GetComponent<AttackAI>();

            if (ai != null)
                ai.enabled = isOn;
        }
    }

public void ToggleInventoryPanel()
{
    if (!inventoryPanel.activeInHierarchy)
    {
        inventoryPanel.SetActive(true);
        blackImage.SetActive(true);

        ClearInventory();
        ShowInventory();
    }
    else
    {
        inventoryPanel.SetActive(false);
        blackImage.SetActive(false);
    }
}

    public void ShowMagicToggles()
    {
        if (PartyManager.instance.SelectChars.Count <= 0)
            return;

        Character hero = PartyManager.instance.SelectChars[0];

        for (int i = 0; i < toggleMagic.Length; i++)
        {
            toggleMagic[i].interactable = false;
            toggleMagic[i].isOn = false;
            toggleMagic[i].GetComponentInChildren<Text>().text = "";
        }

        for (int i = 0; i < hero.MagicSkills.Count && i < toggleMagic.Length; i++)
        {
            toggleMagic[i].interactable = true;
            toggleMagic[i].isOn = false;
            toggleMagic[i].GetComponentInChildren<Text>().text = hero.MagicSkills[i].Name;
            toggleMagic[i].targetGraphic.GetComponent<Image>().sprite = hero.MagicSkills[i].Icon;
        }
    }

    public void SelectMagicSkill(int i)
    {
        curToggleMagicID = i;
        PartyManager.instance.HeroSelectMagicSkill(i);
    }

    public void IsOnCurToggleMagic(bool flag)
    {
        if (curToggleMagicID < 0 || curToggleMagicID >= toggleMagic.Length)
            return;

        toggleMagic[curToggleMagicID].isOn = flag;
    }

    public void SelectAll()
    {
        PartyManager.instance.SelectChars.Clear();

        foreach (Character member in PartyManager.instance.Members)
        {
            if (member.CurHP > 0)
            {
                member.ToggleRingSelection(true);
                PartyManager.instance.SelectChars.Add(member);
            }
        }
    }

public void ClearInventory()
{
    for (int i = 0; i < slots.Length; i++)
    {
        for (int j = slots[i].transform.childCount - 1; j >= 0; j--)
        {
            Destroy(slots[i].transform.GetChild(j).gameObject);
        }
    }
}

    public void ShowInventory()
    {
        if (PartyManager.instance.SelectChars.Count <= 0)
            return;

    Character hero = PartyManager.instance.SelectChars[0];

    for (int i = 0; i < hero.InventoryItems.Length && i < slots.Length; i++)
    {
        if (hero.InventoryItems[i] != null)
        {
            GameObject itemObj = Instantiate(itemUIPrefab, slots[i].transform);
            ItemDrag itemDrag = itemObj.GetComponent<ItemDrag>();

            if (itemDrag == null)
            {
                Debug.LogError("ItemUI prefab is missing ItemDrag component.");
                continue;
            }

            itemDrag.UIManager = this;
            itemDrag.Item = hero.InventoryItems[i];
            itemDrag.IconParent = slots[i].transform;

            if (itemDrag.Image != null)
            {
                itemDrag.Image.sprite = hero.InventoryItems[i].Icon;
            }
            else
            {
                Image img = itemObj.GetComponent<Image>();
                if (img == null)
                    img = itemObj.GetComponentInChildren<Image>();

                if (img != null)
                {
                    img.sprite = hero.InventoryItems[i].Icon;
                    itemDrag.Image = img;
                }
            }
            }
        }
    }

    public void SetCurItemInUse(ItemDrag itemDrag, int index)
    {
        curItemDrag = itemDrag;
        curSlotId = index;
    }

    public void ToggleItemDialog(bool flag)
    {
        grayImage.SetActive(flag);
        itemDialog.SetActive(flag);
    }

    public void DeleteItemIcon()
    {
        Destroy(curItemDrag.gameObject);
    }

    public void ClickDrinkConsumable()
    {
        InventoryManager.instance.DrinkConsumableItem(curItemDrag.Item, curSlotId);
        DeleteItemIcon();
        ToggleItemDialog(false);
    }

    private void InitSlots()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            InventorySlot slot = slots[i].GetComponent<InventorySlot>();

            if (slot != null)
                slot.ID = i;
        }
    }

    public void PauseUnpause(bool isOn)
    {
        Time.timeScale = isOn ? 0 : 1;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            togglePauseUnpause.isOn = !togglePauseUnpause.isOn;
    }

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        InitSlots();
    }
}
