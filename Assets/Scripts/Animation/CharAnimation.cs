using UnityEngine;

public class CharAnimation : MonoBehaviour
{
    private Character character;

    void Start()
    {
        // 8.16: โหลด Component Character เข้ามาเก็บไว้ในตัวแปร
        character = GetComponent<Character>();
    }

    // 8.17: เมธอดสำหรับเลือกเล่น Animation ตามสถานะของตัวละคร
    private void ChooseAnimation(Character c)
    {
        // รีเซ็ตค่า Parameter ให้เป็น false ก่อน
        c.Anim.SetBool("IsIdle", false);
        c.Anim.SetBool("IsWalk", false);

        switch (c.State)
        {
            case CharState.Idle:
                c.Anim.SetBool("IsIdle", true);
                break;

            case CharState.Walk:
            case CharState.WalkToEnemy: // ใช้ท่าเดินเหมือนกัน
            case CharState.WalkToMagicCast:    
                c.Anim.SetBool("IsWalk", true);
                break;
        }
    }

    void Update()
    {
        // เรียกใช้เมธอด ChooseAnimation ใน Update เพื่อให้มันอัปเดตตลอดเวลา
        if (character != null)
        {
            ChooseAnimation(character);
        }
    }
}