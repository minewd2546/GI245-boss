using UnityEngine;

public class QuestManager : MonoBehaviour
{
    [SerializeField]
    private Npc[] npcPerson;
    public Npc[] NPCPerson { get { return npcPerson; } set { npcPerson = value; } }

    [SerializeField]
    private QuestData[] questData;
    public QuestData[] QuestData { get { return questData; } set { questData = value; } }

    [SerializeField]
    private Npc curNpc;
    public Npc CurNPC { get { return curNpc; } set { curNpc = value; } }

    [SerializeField]
    private Quest curQuest;
    public Quest CurQuest { get { return curQuest; } set { curQuest = value; } }

    public static QuestManager instance;

    void Awake()
    {
        instance = this;
    }

    private void AddQuestToNPC(Npc npc, QuestData questData)
    {
        Quest quest = new Quest(questData);
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
        bool success = false;

        Debug.Log(curQuest.Type);

        switch (curQuest.Type)
        {
            case QuestType.Delivery:
                success = CheckItemToDelivery();
                break;
        }

        return success;
    }

    public bool CheckLastDialogue(int i)
    {
        if (i == curQuest.QuestDialogue.Length - 1)
            return true;
        else
            return false;
    }

    public string NextDialogue(int i) // map with ButtonNext
    {
        if (i < curQuest.QuestDialogue.Length)
            return curQuest.QuestDialogue[i];
        else
            return "";
    }

    private void RemoveCurQuestFromNPC()
    {
        if (curNpc == null || curQuest == null)
            return;

        curNpc.QuestToGive.Remove(curQuest);
    }

    public void RejectQuest() // map with ButtonReject
    {
        if (curQuest == null)
            return;

        curQuest.Status = QuestStatus.Reject;
        RemoveCurQuestFromNPC();
    }

    public void AcceptQuest() // map with ButtonAccept
    {
        curQuest.Status = QuestStatus.InProgress;
        PartyManager.instance.QuestList.Add(curQuest);
    }

    public bool DeliverItem()
    {
        return InventoryManager.instance.RemoveItemFromParty(curQuest.QuestItemID);
    }

    public bool NpcGiveReward()
    {
        if (PartyManager.instance.SelectChars.Count == 0)
            return false;

        Character hero = PartyManager.instance.SelectChars[0];

        Item item = new Item(InventoryManager.instance.ItemData[curQuest.RewardItemId]);

        for (int i = 0; i < 16; i++)
        {
            if (hero.InventoryItems[i] == null)
            {
                hero.InventoryItems[i] = item;
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
        AddQuestToNPC(npcPerson[0], questData[0]); // Give Golem - Give Potion Quest
    }
}
