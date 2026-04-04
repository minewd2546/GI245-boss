using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

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

    [SerializeField]
    private GameObject downPanel;

    [SerializeField]
    private GameObject npcDialoguePanel;

    [SerializeField]
    private Image npcImage;

    [SerializeField]
    private TMP_Text npcNameText;

    [SerializeField]
    private TMP_Text dialogueText;

    [SerializeField]
    private int index; // dialogue step

    [SerializeField]
    private GameObject btnNext;

    [SerializeField]
    private TMP_Text btnNextText;

    [SerializeField]
    private GameObject btnAccept;

    [SerializeField]
    private TMP_Text btnAcceptText;

    [SerializeField]
    private GameObject btnReject;

    [SerializeField]
    private TMP_Text btnRejectText;

    [SerializeField]
    private GameObject btnFinish;

    [SerializeField]
    private TMP_Text btnFinishText;

    [SerializeField]
    private GameObject btnNotFinish;

    [SerializeField]
    private TMP_Text btnNotFinishText;

    [SerializeField]
    private GameObject rewardPanel;

    [SerializeField]
    private TMP_Text rewardText;

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

    private void ClearDialogueBox()
    {
        npcImage.sprite = null;

        npcNameText.text = "";
        dialogueText.text = "";

        btnNextText.text = "";
        btnNext.SetActive(false);

        btnAcceptText.text = "";
        btnAccept.SetActive(false);

        btnRejectText.text = "";
        btnReject.SetActive(false);

        btnFinishText.text = "";
        btnFinish.SetActive(false);

        btnNotFinishText.text = "";
        btnNotFinish.SetActive(false);
    }

    private void StartQuestDialogue(Quest quest)
    {
        dialogueText.text = quest.QuestDialogue[index];

        btnNext.SetActive(true);
        btnNextText.text = quest.AnswerNext[index];

        btnAccept.SetActive(false);
        btnReject.SetActive(false);
    }

    private void SetupDialoguePanel(Npc npc)
    {
        index = 0;

        npcImage.sprite = npc.AvatarPic;
        npcNameText.text = npc.CharName;

        Quest inProgressQuest = QuestManager.instance.CheckForQuest(npc, QuestStatus.InProgress);

        if (inProgressQuest != null) // There is an In-Progress Quest going on
        {
            Debug.Log("in-progress: " + inProgressQuest);
            dialogueText.text = inProgressQuest.QuestionInProgress;

            bool hasItem = QuestManager.instance.CheckIfFinishQuest();
            Debug.Log(hasItem);

            if (hasItem) // has item to finish quest
            {
                btnFinishText.text = inProgressQuest.AnswerFinish;
                btnFinish.SetActive(true);
            }
            else
            {
                btnNotFinishText.text = inProgressQuest.AnswerNotFinish;
                btnNotFinish.SetActive(true);
            }
        }
        else // Check for New Quest
        {
            Quest newQuest = QuestManager.instance.CheckForQuest(npc, QuestStatus.New);

            if (newQuest != null) // There is a new Quest
                StartQuestDialogue(newQuest);
            else
                ShowNoQuestDialogue();
        }
    }

    private void ToggleDialogueBox(bool flag)
    {
        downPanel.SetActive(!flag);
        npcDialoguePanel.SetActive(flag);
        togglePauseUnpause.isOn = flag;
    }

    public void PrepareDialogueBox(Npc npc)
    {
        ClearDialogueBox();
        SetupDialoguePanel(npc);
        ToggleDialogueBox(true);
    }

    public void AnswerNext() // map with ButtonNext
    {
        index++;
        dialogueText.text = QuestManager.instance.NextDialogue(index);

        if (QuestManager.instance.CheckLastDialogue(index)) // last dialogue
        {
            btnNext.SetActive(false);

            btnAcceptText.text = QuestManager.instance.CurQuest.AnswerAccept;
            btnAccept.SetActive(true);

            btnRejectText.text = QuestManager.instance.CurQuest.AnswerReject;
            btnReject.SetActive(true);
        }
        else
        {
            btnNext.SetActive(true);
            btnNextText.text = QuestManager.instance.CurQuest.AnswerNext[index];
        }
    }

    public void AnswerReject() // map with ButtonReject
    {
        if (QuestManager.instance != null && QuestManager.instance.CurQuest != null)
            QuestManager.instance.RejectQuest();

        ToggleDialogueBox(false);
    }

    public void AnswerAccept() // map with ButtonAccept
    {
        QuestManager.instance.AcceptQuest();
        ToggleDialogueBox(false);
    }

    public void AnswerFinish() // map with ButtonFinish
    {
        Debug.Log("Can Finish Quest");
        bool success = QuestManager.instance.DeliverItem();

        if (success)
        {
            if (QuestManager.instance.NpcGiveReward())
            {
                ShowQuestReward();
                Debug.Log("Quest Completed");
                ToggleDialogueBox(false);
            }
        }
    }

    public void AnswerNotFinish() // map with ButtonNotFinish
    {
        Debug.Log("Cannot Finish Quest");
        ToggleDialogueBox(false);
    }

    private void ShowNoQuestDialogue()
    {
        dialogueText.text = "No quest available.";
        btnNext.SetActive(false);

        btnRejectText.text = "Close";
        btnReject.SetActive(true);
    }

    private void ShowQuestReward()
    {
        if (rewardPanel == null || rewardText == null)
        {
            Debug.LogWarning("RewardPanel or RewardText is not assigned in UIManager.");
            return;
        }

        ItemData rewardItem = InventoryManager.instance.ItemData[QuestManager.instance.CurQuest.RewardItemId];
        rewardText.text = "Received: " + rewardItem.itemName;
        rewardPanel.SetActive(true);

        StopAllCoroutines();
        StartCoroutine(HideRewardUI());
    }

    private IEnumerator HideRewardUI()
    {
        yield return new WaitForSecondsRealtime(2f);

        if (rewardPanel != null)
            rewardPanel.SetActive(false);
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

        if (rewardPanel != null)
            rewardPanel.SetActive(false);
    }
}
