using UnityEngine;
using System.Collections.Generic;

public class Npc : Character
{
    [SerializeField]
    private List<Quest> questToGive = new List<Quest>();
    public List<Quest> QuestToGive { get { return questToGive; } set { questToGive = value; } }

    public Quest CheckQuestList(QuestStatus status)
    {
        foreach (Quest quest in questToGive)
        {
            if (quest.Status == status)
                return quest;
        }

        return null;
    }
}
