using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private RectTransform selectionBox;
    public RectTransform SelectionBox => selectionBox;

    public static UIManager instance;

    [Header("Common UI")]
    [SerializeField] private Toggle togglePauseUnpause;
    [SerializeField] private GameObject blackImage;
    [SerializeField] private GameObject grayImage;
    [SerializeField] private GameObject downPanel;

    [Header("Inventory")]
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private GameObject itemDialog;
    [SerializeField] private GameObject itemUIPrefab;
    [SerializeField] private GameObject[] slots;
    [SerializeField] private ItemDrag curItemDrag;
    [SerializeField] private int curSlotId;

    [Header("Magic")]
    [SerializeField] private Toggle[] toggleMagic;
    public Toggle[] ToggleMagic => toggleMagic;
    [SerializeField] private int curToggleMagicID = -1;

    [Header("Party Avatar")]
    [SerializeField] private Toggle[] toggleAvatar;
    [SerializeField] private GameObject charPanel;
    [SerializeField] private TMP_Text charNameText;
    [SerializeField] private TMP_Text statText;
    [SerializeField] private TMP_Text abilityText;
    [SerializeField] private Image heroImage;
    [SerializeField] private GameObject partyPanel;
    [SerializeField] private Toggle[] toggleRemove;
    [SerializeField] private GameObject removeButton;
    [SerializeField] private GameObject confirmPanel;
    [SerializeField] private int removeMemberIndex = -1;

    [Header("Dialogue")]
    [SerializeField] private GameObject npcDialoguePanel;
    [SerializeField] private Image npcImage;
    [SerializeField] private TMP_Text npcNameText;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private int index;
    [SerializeField] private GameObject btnNext;
    [SerializeField] private TMP_Text btnNextText;
    [SerializeField] private GameObject btnAccept;
    [SerializeField] private TMP_Text btnAcceptText;
    [SerializeField] private GameObject btnReject;
    [SerializeField] private TMP_Text btnRejectText;
    [SerializeField] private GameObject btnFinish;
    [SerializeField] private TMP_Text btnFinishText;
    [SerializeField] private GameObject btnNotFinish;
    [SerializeField] private TMP_Text btnNotFinishText;
    [SerializeField] private GameObject btnJoinParty;
    [SerializeField] private TMP_Text btnJoinPartyText;
    [SerializeField] private GameObject btnNotJoinParty;
    [SerializeField] private TMP_Text btnNotJoinPartyText;
    [SerializeField] private Hero pendingJoinHero;

    [Header("Quest Reward")]
    [SerializeField] private GameObject rewardPanel;
    [SerializeField] private TMP_Text rewardText;

    [Header("Shop")]
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private TMP_Text npcShopNameText;
    [SerializeField] private Transform shopListParent;
    [SerializeField] private Transform partyListParent;
    [SerializeField] private TMP_Text shopMoneyText;
    [SerializeField] private TMP_Text heroMoneyText;
    [SerializeField] private GameObject itemInShopPrefab;
    [SerializeField] private List<ItemInShop> shopItemList = new List<ItemInShop>();
    [SerializeField] private List<ItemInShop> partyItemList = new List<ItemInShop>();
    [SerializeField] private int totalCost;
    [SerializeField] private int totalPrice;
    [SerializeField] private Npc curShopNpc;
    [SerializeField] private Hero curShopHero;
    [SerializeField] private TMP_Text heroNameText;
    [SerializeField] private Item selectedShopItem;
    [SerializeField] private Item selectedPartyItem;

    public void ToggleAI(bool isOn)
    {
        foreach (Character member in PartyManager.instance.Members)
        {
            AttackAI ai = member.GetComponent<AttackAI>();
            if (ai != null)
                ai.enabled = isOn;
        }
    }

    public void ToggleInventoryPanel()
    {
        bool flag = !inventoryPanel.activeInHierarchy;

        inventoryPanel.SetActive(flag);
        if (blackImage != null)
            blackImage.SetActive(flag);

        if (flag)
        {
            ClearInventory();
            ShowInventory();
        }
    }

    public void ShowMagicToggles()
    {
        Character hero = PartyManager.instance.SelectChars.Count > 0 ? PartyManager.instance.SelectChars[0] : null;

        for (int i = 0; i < toggleMagic.Length; i++)
        {
            toggleMagic[i].interactable = false;
            toggleMagic[i].SetIsOnWithoutNotify(false);
            toggleMagic[i].GetComponentInChildren<Text>().text = "";
        }

        if (hero == null)
            return;

        for (int i = 0; i < hero.MagicSkills.Count && i < toggleMagic.Length; i++)
        {
            toggleMagic[i].interactable = true;
            toggleMagic[i].SetIsOnWithoutNotify(false);
            toggleMagic[i].GetComponentInChildren<Text>().text = hero.MagicSkills[i].Name;

            Image image = toggleMagic[i].targetGraphic.GetComponent<Image>();
            if (image != null)
                image.sprite = hero.MagicSkills[i].Icon;
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

        toggleMagic[curToggleMagicID].SetIsOnWithoutNotify(flag);
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

        MapToggleAvatar();
        RefreshSelectedHeroPanel();
        ShowMagicToggles();
    }

    public void MapToggleAvatar()
    {
        if (toggleAvatar == null)
            return;

        for (int i = 0; i < toggleAvatar.Length; i++)
        {
            bool hasMember = PartyManager.instance != null && i < PartyManager.instance.Members.Count;

            toggleAvatar[i].gameObject.SetActive(hasMember);
            toggleAvatar[i].interactable = hasMember;
            toggleAvatar[i].SetIsOnWithoutNotify(hasMember && PartyManager.instance.SelectChars.Contains(PartyManager.instance.Members[i]));

            if (hasMember)
            {
                Image image = toggleAvatar[i].targetGraphic as Image;
                Sprite avatar = PartyManager.instance.Members[i].AvatarPic;
                if (image != null && avatar != null)
                    image.sprite = avatar;
            }
        }
    }

    public void ForceAvatarToggle(int i, bool isOn)
    {
        if (toggleAvatar == null || i < 0 || i >= toggleAvatar.Length || toggleAvatar[i] == null)
            return;

        toggleAvatar[i].SetIsOnWithoutNotify(isOn);
    }

    public void SelectHeroByAvatar(int i)
    {
        if (toggleAvatar == null || i < 0 || i >= toggleAvatar.Length || toggleAvatar[i] == null)
            return;

        if (toggleAvatar[i].isOn)
            PartyManager.instance.SelectSingleHeroByToggle(i);
        else
            PartyManager.instance.UnSelectSingleHeroByToggle(i);

        RefreshSelectedHeroPanel();
    }

    public void RefreshSelectedHeroPanel()
    {
        if (charPanel != null && charPanel.activeInHierarchy)
            ShowCharPanelData();
    }

    public void ToggleCharPanel(bool flag)
    {
        if (charPanel == null)
            return;

        charPanel.SetActive(flag);

        if (flag)
        {
            if (partyPanel != null)
                partyPanel.SetActive(false);
            if (confirmPanel != null)
                confirmPanel.SetActive(false);

            ShowCharPanelData();
        }
    }

    public void ToggleCharPanel()
    {
        if (charPanel == null)
            return;

        ToggleCharPanel(!charPanel.activeInHierarchy);
    }

    public void ClearCharPanel()
    {
        if (charNameText != null)
            charNameText.text = "";

        if (statText != null)
            statText.text = "";

        if (abilityText != null)
            abilityText.text = "";

        if (heroImage != null)
            heroImage.sprite = null;
    }

    private void ShowCharPanelData()
    {
        if (PartyManager.instance.SelectChars.Count == 0)
            return;

        Hero hero = PartyManager.instance.SelectChars[0] as Hero;
        if (hero == null)
            return;

        if (heroImage != null)
            heroImage.sprite = hero.AvatarPic;

        if (charNameText != null)
            charNameText.text = hero.CharName;

        if (statText != null)
        {
            statText.text =
                $"Level: {hero.Level}\n" +
                $"Exp: {hero.Exp}/{hero.NextExp}\n" +
                $"HP: {hero.CurHP}/{hero.MaxHP}\n" +
                $"Attack: {hero.AttackDamage}\n" +
                $"Defense: {hero.DefensePower}";
        }

        if (abilityText != null)
        {
            abilityText.text =
                $"STR: {hero.Strength}\n" +
                $"DEX: {hero.Dexterity}\n" +
                $"CON: {hero.Constitution}\n" +
                $"INT: {hero.Intelligence}\n" +
                $"WIS: {hero.Wisdom}\n" +
                $"CHA: {hero.Charisma}";
        }
    }

    public void TogglePartyPanel(bool flag)
    {
        if (partyPanel == null)
            return;

        partyPanel.SetActive(flag);

        if (charPanel != null)
            charPanel.SetActive(!flag);

        if (confirmPanel != null)
            confirmPanel.SetActive(false);

        if (flag)
        {
            MapToggleRemove();
            CheckRemoveButton();
        }
    }

    public void MapToggleRemove()
    {
        if (toggleRemove == null)
            return;

        for (int i = 0; i < toggleRemove.Length; i++)
        {
            int memberIndex = i + 1;
            bool hasMember = PartyManager.instance != null && memberIndex < PartyManager.instance.Members.Count;

            toggleRemove[i].gameObject.SetActive(hasMember);
            toggleRemove[i].SetIsOnWithoutNotify(false);

            if (hasMember)
            {
                Image image = toggleRemove[i].targetGraphic as Image;
                Sprite avatar = PartyManager.instance.Members[memberIndex].AvatarPic;
                if (image != null && avatar != null)
                    image.sprite = avatar;
            }
        }

        removeMemberIndex = -1;
    }

    public void SelectToRemove(int i)
    {
        int toggleIndex = i - 1;
        if (toggleRemove == null || toggleIndex < 0 || toggleIndex >= toggleRemove.Length)
            return;

        removeMemberIndex = toggleRemove[toggleIndex].isOn ? i : -1;
        CheckRemoveButton();
    }

    public void CheckRemoveButton()
    {
        if (removeButton != null)
            removeButton.SetActive(removeMemberIndex > 0);
    }

    public void ToggleConfirmPanel(bool flag)
    {
        if (confirmPanel == null || partyPanel == null)
            return;

        confirmPanel.SetActive(flag);
        partyPanel.SetActive(!flag);
    }

    public void RemoveMemberFromParty()
    {
        if (PartyManager.instance.RemoveMemberFromParty(removeMemberIndex))
        {
            removeMemberIndex = -1;
            ToggleConfirmPanel(false);
            MapToggleRemove();
            CheckRemoveButton();
        }
    }

    public void ClearInventory()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            for (int j = slots[i].transform.childCount - 1; j >= 0; j--)
                Destroy(slots[i].transform.GetChild(j).gameObject);
        }
    }

    public void ShowInventory()
    {
        if (PartyManager.instance.SelectChars.Count <= 0)
            return;

        Character hero = PartyManager.instance.SelectChars[0];

        for (int i = 0; i < hero.InventoryItems.Length && i < slots.Length; i++)
        {
            if (hero.InventoryItems[i] == null)
                continue;

            GameObject itemObj = Instantiate(itemUIPrefab, slots[i].transform);
            ItemDrag itemDrag = itemObj.GetComponent<ItemDrag>();

            if (itemDrag == null)
                continue;

            itemDrag.UIManager = this;
            itemDrag.Item = hero.InventoryItems[i];
            itemDrag.IconParent = slots[i].transform;

            if (itemDrag.Image != null)
                itemDrag.Image.sprite = hero.InventoryItems[i].Icon;
            else
            {
                Image image = itemObj.GetComponent<Image>();
                if (image == null)
                    image = itemObj.GetComponentInChildren<Image>();

                if (image != null)
                {
                    image.sprite = hero.InventoryItems[i].Icon;
                    itemDrag.Image = image;
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
        if (grayImage != null)
            grayImage.SetActive(flag);
        if (itemDialog != null)
            itemDialog.SetActive(flag);
    }

    public void DeleteItemIcon()
    {
        if (curItemDrag != null)
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
        int bagIndex = 0;

        for (int i = 0; i < slots.Length; i++)
        {
            InventorySlot slot = slots[i].GetComponent<InventorySlot>();
            if (slot != null)
            {
                switch (slot.ItemType)
                {
                    case ItemType.Weapon:
                        slot.ID = 16;
                        break;
                    case ItemType.Shield:
                        slot.ID = 17;
                        break;
                    default:
                        slot.ID = bagIndex;
                        bagIndex++;
                        break;
                }
            }
        }
    }

    public void PauseUnpause(bool isOn)
    {
        Time.timeScale = isOn ? 0 : 1;
    }

    private void ClearDialogueBox()
    {
        if (npcImage != null)
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

        if (btnJoinPartyText != null)
            btnJoinPartyText.text = "";
        if (btnNotJoinPartyText != null)
            btnNotJoinPartyText.text = "";
        if (btnJoinParty != null)
            btnJoinParty.SetActive(false);
        if (btnNotJoinParty != null)
            btnNotJoinParty.SetActive(false);
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

        if (inProgressQuest != null)
        {
            dialogueText.text = inProgressQuest.QuestionInProgress;

            bool hasItem = QuestManager.instance.CheckIfFinishQuest();
            if (hasItem)
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
        else
        {
            Quest newQuest = QuestManager.instance.CheckForQuest(npc, QuestStatus.New);

            if (newQuest != null)
                StartQuestDialogue(newQuest);
            else
                ShowNoQuestDialogue();
        }
    }

    private void ToggleDialogueBox(bool flag)
    {
        if (downPanel != null)
            downPanel.SetActive(!flag);
        if (npcDialoguePanel != null)
            npcDialoguePanel.SetActive(flag);
        if (togglePauseUnpause != null)
            togglePauseUnpause.isOn = flag;
    }

    public void PrepareDialogueBox(Npc npc)
    {
        ClearDialogueBox();
        SetupDialoguePanel(npc);
        ToggleDialogueBox(true);
    }

    public void PrepareHeroJoinBox(Hero hero)
    {
        if (hero == null || PartyManager.instance.HasMember(hero))
            return;

        pendingJoinHero = hero;

        ClearDialogueBox();
        npcImage.sprite = hero.AvatarPic;
        npcNameText.text = hero.CharName;
        dialogueText.text = $"Would you like {hero.CharName} to join the party?";

        if (btnJoinParty != null)
        {
            btnJoinParty.SetActive(true);
            if (btnJoinPartyText != null)
                btnJoinPartyText.text = "Welcome.";
        }

        if (btnNotJoinParty != null)
        {
            btnNotJoinParty.SetActive(true);
            if (btnNotJoinPartyText != null)
                btnNotJoinPartyText.text = "No. Not now.";
        }

        ToggleDialogueBox(true);
    }

    public void AnswerJoinParty()
    {
        if (pendingJoinHero != null)
            PartyManager.instance.AddMember(pendingJoinHero);

        pendingJoinHero = null;
        ToggleDialogueBox(false);
    }

    public void AnswerNotJoinParty()
    {
        pendingJoinHero = null;
        ToggleDialogueBox(false);
    }

    public void AnswerNext()
    {
        index++;
        dialogueText.text = QuestManager.instance.NextDialogue(index);

        if (QuestManager.instance.CheckLastDialogue(index))
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

    public void AnswerReject()
    {
        if (QuestManager.instance != null && QuestManager.instance.CurQuest != null)
            QuestManager.instance.RejectQuest();

        ToggleDialogueBox(false);
    }

    public void AnswerAccept()
    {
        QuestManager.instance.AcceptQuest();
        ToggleDialogueBox(false);
    }

    public void AnswerFinish()
    {
        bool success = QuestManager.instance.DeliverItem();

        if (success && QuestManager.instance.NpcGiveReward())
        {
            ShowQuestReward();
            ToggleDialogueBox(false);
        }
    }

    public void AnswerNotFinish()
    {
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
            return;

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

    public void ClearShopPanel()
    {
        selectedShopItem = null;
        selectedPartyItem = null;

        if (shopListParent != null)
        {
            for (int i = shopListParent.childCount - 1; i >= 0; i--)
                Destroy(shopListParent.GetChild(i).gameObject);
        }

        if (partyListParent != null)
        {
            for (int i = partyListParent.childCount - 1; i >= 0; i--)
                Destroy(partyListParent.GetChild(i).gameObject);
        }

        shopItemList.Clear();
        partyItemList.Clear();
        totalCost = 0;
        totalPrice = 0;
    }

    private void SetupShopItems()
    {
        if (curShopNpc == null || shopListParent == null || itemInShopPrefab == null)
            return;

        for (int i = 0; i < curShopNpc.ShopItems.Length; i++)
        {
            Item item = curShopNpc.ShopItems[i];
            if (!IsUsableShopItem(item))
                continue;

            GameObject obj = Instantiate(itemInShopPrefab, shopListParent);
            ItemInShop card = obj.GetComponent<ItemInShop>();
            if (card != null)
            {
                card.Init(item, true, this);
                shopItemList.Add(card);
            }
        }
    }

    private void SetupPartyItems()
    {
        if (PartyManager.instance.SelectChars.Count == 0 || partyListParent == null || itemInShopPrefab == null)
            return;

        Character hero = PartyManager.instance.SelectChars[0];
        curShopHero = hero as Hero;

        for (int i = 0; i < hero.InventoryItems.Length; i++)
        {
            Item item = hero.InventoryItems[i];
            if (!IsUsableShopItem(item))
                continue;

            GameObject obj = Instantiate(itemInShopPrefab, partyListParent);
            ItemInShop card = obj.GetComponent<ItemInShop>();
            if (card != null)
            {
                card.Init(item, false, this);
                partyItemList.Add(card);
            }
        }
    }

    public void SelectShopItem(Item item)
    {
        ClearCardSelection(partyItemList);
        selectedShopItem = item;
        selectedPartyItem = null;
    }

    public void SelectPartyItem(Item item)
    {
        ClearCardSelection(shopItemList);
        selectedShopItem = null;
        selectedPartyItem = item;
    }

    private bool IsUsableShopItem(Item item)
    {
        if (item == null)
            return false;

        return !string.IsNullOrEmpty(item.ItemName) || item.Icon != null || item.NormalPrice > 0;
    }

    private void ClearCardSelection(List<ItemInShop> cards)
    {
        if (cards == null)
            return;

        for (int i = 0; i < cards.Count; i++)
        {
            if (cards[i] != null)
                cards[i].SetSelectedWithoutNotify(false);
        }
    }

    private int FindItemIndex(Character hero, Item item)
    {
        if (hero == null || item == null || hero.InventoryItems == null)
            return -1;

        for (int i = 0; i < hero.InventoryItems.Length; i++)
        {
            if (ReferenceEquals(hero.InventoryItems[i], item))
                return i;
        }

        for (int i = 0; i < hero.InventoryItems.Length; i++)
        {
            if (hero.InventoryItems[i] != null && hero.InventoryItems[i].ID == item.ID)
                return i;
        }

        return -1;
    }

    private TMP_Text FindTMPByName(Transform root, string objectName)
    {
        if (root == null)
            return null;

        TMP_Text[] texts = root.GetComponentsInChildren<TMP_Text>(true);
        for (int i = 0; i < texts.Length; i++)
        {
            if (texts[i].gameObject.name == objectName)
                return texts[i];
        }

        return null;
    }

    private void ResolveShopUIRefs()
    {
        if (shopPanel == null)
            return;

        Transform root = shopPanel.transform;

        if (npcShopNameText == null)
            npcShopNameText = FindTMPByName(root, "NPCShopNameText");

        if (heroNameText == null)
            heroNameText = FindTMPByName(root, "OwnerText");

        if (shopMoneyText == null)
            shopMoneyText = FindTMPByName(root, "MoneyText");

        if (heroMoneyText == null)
        {
            TMP_Text[] texts = root.GetComponentsInChildren<TMP_Text>(true);
            for (int i = 0; i < texts.Length; i++)
            {
                if (texts[i].gameObject.name == "MoneyText" && texts[i] != shopMoneyText)
                {
                    heroMoneyText = texts[i];
                    break;
                }
            }
        }
    }

    public void ToggleShopPanel(bool flag)
    {
        if (shopPanel == null)
            return;

        shopPanel.SetActive(flag);

        if (blackImage != null)
            blackImage.SetActive(flag);

        if (!flag)
            ClearShopPanel();
    }

    public void PrepareShopPanel(Npc npc)
    {
        curShopNpc = npc;
        ClearShopPanel();
        ResolveShopUIRefs();

        if (npcShopNameText != null)
            npcShopNameText.text = npc.CharName;
        if (shopMoneyText != null)
            shopMoneyText.text = $"Shop Gold: {npc.Money}";
        if (heroMoneyText != null)
            heroMoneyText.text = $"Party Gold: {PartyManager.instance.Money}";
        if (PartyManager.instance.SelectChars.Count > 0)
        {
            curShopHero = PartyManager.instance.SelectChars[0] as Hero;

            if (heroNameText != null && curShopHero != null)
                heroNameText.text = curShopHero.CharName;
        }

        SetupShopItems();
        SetupPartyItems();
        ToggleShopPanel(true);
    }

    public void SellItemToShop()
    {
        if (curShopNpc == null || selectedPartyItem == null || PartyManager.instance.SelectChars.Count == 0)
            return;

        Hero hero = PartyManager.instance.SelectChars[0] as Hero;
        if (hero == null)
            return;

        int inventoryIndex = FindItemIndex(hero, selectedPartyItem);
        if (inventoryIndex < 0)
            return;

        int price = Mathf.Max(1, selectedPartyItem.NormalPrice);
        if (curShopNpc.Money < price)
            return;

        if (!curShopNpc.AddShopItem(selectedPartyItem))
            return;

        if (inventoryIndex == 16 || inventoryIndex == 17)
            InventoryManager.instance.RemoveItemInBag(inventoryIndex);
        else
            hero.InventoryItems[inventoryIndex] = null;

        PartyManager.instance.Money += price;
        curShopNpc.Money -= price;
        PrepareShopPanel(curShopNpc);
    }

    public void BuyItemFromShop()
    {
        if (curShopNpc == null || selectedShopItem == null || PartyManager.instance.SelectChars.Count == 0)
            return;

        Hero hero = PartyManager.instance.SelectChars[0] as Hero;
        if (hero == null)
            return;

        int price = Mathf.Max(1, selectedShopItem.NormalPrice);
        if (PartyManager.instance.Money < price)
            return;

        if (!curShopNpc.RemoveShopItem(selectedShopItem))
            return;

        if (!hero.AddItemToInventory(selectedShopItem))
        {
            curShopNpc.AddShopItem(selectedShopItem);
            return;
        }

        PartyManager.instance.Money -= price;
        curShopNpc.Money += price;
        PrepareShopPanel(curShopNpc);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && togglePauseUnpause != null)
            togglePauseUnpause.isOn = !togglePauseUnpause.isOn;
    }

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        InitSlots();
        MapToggleAvatar();

        if (rewardPanel != null)
            rewardPanel.SetActive(false);
        if (charPanel != null)
            charPanel.SetActive(false);
        if (partyPanel != null)
            partyPanel.SetActive(false);
        if (confirmPanel != null)
            confirmPanel.SetActive(false);
        if (shopPanel != null)
            shopPanel.SetActive(false);
    }
}
