using System.Collections.Generic;
using UnityEngine;

public class PartyManager : MonoBehaviour
{
    [SerializeField]
    private List<Character> members = new List<Character>();
    public List<Character> Members { get { return members; } }

    [SerializeField]
    private List<Character> selectChars = new List<Character>();
    public List<Character> SelectChars { get { return selectChars; } }

    public static PartyManager instance;

    void Start()
    {
        foreach (Character c in members)
        {
            c.charInit(VFXManager.instance, UIManager.instance, InventoryManager.instance);
        }

        SelectSingleHero(0);
		// new Magic(รหัสสกิล, "ชื่อสกิล", ระยะยิง, พลังโจมตี, เวลาร่าย(วินาที), เวลาพุ่งชน(วินาที), IDเอฟเฟกต์ตอนร่าย, IDเอฟเฟกต์ตอนยิง);
        members[0].MagicSkills.Add(new Magic(VFXManager.instance.MagicData[0]));
		members[0].MagicSkills.Add(new Magic(VFXManager.instance.MagicData[2]));

        members[1].MagicSkills.Add(new Magic(VFXManager.instance.MagicData[1]));
		members[1].MagicSkills.Add(new Magic(VFXManager.instance.MagicData[3]));
        
        // Member 1
		InventoryManager.instance.AddItem(members[0], 0); // Health Potion
		InventoryManager.instance.AddItem(members[0], 1); // Sword
        
        // Member 2
        InventoryManager.instance.AddItem(members[1], 0); // Healing Potion
        InventoryManager.instance.AddItem(members[1], 1); // Sword
        InventoryManager.instance.AddItem(members[1], 2); // Shield
        InventoryManager.instance.AddItem(members[1], 3); // Mana Potion
        InventoryManager.instance.AddItem(members[1], 4); // Crystal
        InventoryManager.instance.AddItem(members[1], 5); // Raw Turkey
        InventoryManager.instance.AddItem(members[1], 6); // Necklace
        InventoryManager.instance.AddItem(members[1], 7); // Key
        InventoryManager.instance.AddItem(members[1], 8); // Gem
        InventoryManager.instance.AddItem(members[1], 9); // Ring

        UIManager.instance.ShowMagicToggles();
    }

	void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (selectChars.Count > 0)
            {
                selectChars[0].IsMagicMode = true;
                
                // เพิ่มการตรวจสอบ: ถ้ายังไม่มีการเลือกสกิลไว้ ค่อยให้ค่าเริ่มต้นเป็นสกิลที่ 0
                if (selectChars[0].CurMagicCast == null)
                {
                    selectChars[0].CurMagicCast = selectChars[0].MagicSkills[0];
                }
            }
        }
    }
    
    public void HeroSelectMagicSkill(int i)
    {
        if (selectChars.Count <= 0)
            return;

        selectChars[0].IsMagicMode = true;
        selectChars[0].CurMagicCast = selectChars[0].MagicSkills[i];
    }
    
    public void SelectSingleHero(int i)
    {
        foreach (Character c in selectChars)
            c.ToggleRingSelection(false);

        selectChars.Clear();

        selectChars.Add(members[i]);
        selectChars[0].ToggleRingSelection(true);
        UIManager.instance.ShowMagicToggles();
    }
    
    void Awake()
    {
        instance = this;
    }
}
