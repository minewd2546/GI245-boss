using UnityEngine;

public class QuestManager : MonoBehaviour
{
    [SerializeField] private Npc[] npcPerson;
    public Npc[] NPCPerson
    {
        get => npcPerson;
        set => npcPerson = value;
    }

    [SerializeField] private QuestData[] questData;
    public QuestData[] QuestData
    {
        get => questData;
        set => questData = value;
    }

    [SerializeField] private Npc curNpc;
    public Npc CurNPC
    {
        get => curNpc;
        set => curNpc = value;
    }

    [SerializeField] private Quest curQuest;
    public Quest CurQuest
    {
        get => curQuest;
        set => curQuest = value;
    }

    public static QuestManager instance;

    void Awake()
    {
        instance = this;
    }

    private void AddQuestToNPC(Npc npc, QuestData data)
    {
        if (npc == null || data == null)
            return;

        Quest quest = new Quest(data);
        npc.QuestToGive.Add(quest);
    }

    public Quest CheckForQuest(Npc npc, QuestStatus status)
    {
        curNpc = npc;
        Quest quest = npc.CheckQuestList(status);
        curQuest = quest;
        return quest;
    }

    private bool CheckItemToDelivery()
    {
        return InventoryManager.instance.CheckPartyForItem(curQuest.QuestItemID);
    }

    public bool CheckIfFinishQuest()
    {
        if (curQuest == null)
            return false;

        switch (curQuest.Type)
        {
            case QuestType.Delivery:
                return CheckItemToDelivery();
            default:
                return false;
        }
    }

    public bool CheckLastDialogue(int i)
    {
        return curQuest != null && i == curQuest.QuestDialogue.Length - 1;
    }

    public string NextDialogue(int i)
    {
        if (curQuest != null && i < curQuest.QuestDialogue.Length)
            return curQuest.QuestDialogue[i];

        return "";
    }

    private void RemoveCurQuestFromNPC()
    {
        if (curNpc == null || curQuest == null)
            return;

        curNpc.QuestToGive.Remove(curQuest);
    }

    public void RejectQuest()
    {
        if (curQuest == null)
            return;

        curQuest.Status = QuestStatus.Reject;
        RemoveCurQuestFromNPC();
    }

    public void AcceptQuest()
    {
        if (curQuest == null)
            return;

        curQuest.Status = QuestStatus.InProgress;
        PartyManager.instance.QuestList.Add(curQuest);
    }

    public bool DeliverItem()
    {
        return curQuest != null && InventoryManager.instance.RemoveItemFromParty(curQuest.QuestItemID);
    }

    public bool NpcGiveReward()
    {
        if (PartyManager.instance.SelectChars.Count == 0 || curQuest == null)
            return false;

        Character hero = PartyManager.instance.SelectChars[0];
        Item item = new Item(InventoryManager.instance.ItemData[curQuest.RewardItemId]);

        for (int i = 0; i < 16; i++)
        {
            if (hero.InventoryItems[i] == null)
            {
                hero.InventoryItems[i] = item;
                PartyManager.instance.ShareExpToParty(curQuest.RewardExp);
                curQuest.Status = QuestStatus.Finish;
                PartyManager.instance.QuestList.Remove(curQuest);
                RemoveCurQuestFromNPC();
                return true;
            }
        }

        return false;
    }

    void Start()
    {
        foreach (Npc npc in npcPerson)
        {
            if (npc != null)
                npc.CharInit(VFXManager.instance, UIManager.instance, InventoryManager.instance, PartyManager.instance);
        }

        if (npcPerson.Length > 0 && questData.Length > 0)
            AddQuestToNPC(npcPerson[0], questData[0]);
    }
}
