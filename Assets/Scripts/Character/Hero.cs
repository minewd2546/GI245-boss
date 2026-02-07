using UnityEngine;

public class Hero : Character
{
    void Update()
    {
        switch (state)
        {
            case CharState.Walk:
                WalkUpdate();
                break;
            case CharState.WalkToEnemy:
                WalkToEnemyUpdate();
                break;
            case CharState.Attack:
                AttackUpdate(); // <--- ส่วนที่เพิ่มเข้ามาครับ
                break;
        }
    }
}