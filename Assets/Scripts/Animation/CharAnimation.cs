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
        // รีเซ็ตค่า Boolean เดิมก่อน
        c.Anim.SetBool("IsIdle", false);
        c.Anim.SetBool("IsWalk", false);

        // เช็คสถานะปัจจุบันของตัวละคร (c.State)
        switch (c.State)
        {
            case CharState.Idle:
                c.Anim.SetBool("IsIdle", true);
                break;
            case CharState.Walk:
                c.Anim.SetBool("IsWalk", true);
                break;
        }
    }

    void Update()
    {
        // อย่าลืมเรียกใช้เมธอด ChooseAnimation ใน Update เพื่อให้มันอัปเดตตลอดเวลา
        if (character != null)
        {
            ChooseAnimation(character);
        }
    }
}