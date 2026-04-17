using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private List<Enemy> monsters;
    public List<Enemy> Monsters => monsters;

    public static EnemyManager instance;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        foreach (Character m in monsters)
            m.CharInit(VFXManager.instance, UIManager.instance, InventoryManager.instance, PartyManager.instance);

        if (monsters.Count > 0)
        {
            InventoryManager.instance.AddItem(monsters[0], 0);
            InventoryManager.instance.AddItem(monsters[0], 1);
            InventoryManager.instance.AddItem(monsters[0], 2);
        }
    }
}
