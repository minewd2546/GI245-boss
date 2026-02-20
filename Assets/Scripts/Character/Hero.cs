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
                AttackUpdate(); // <--- ��ǹ�����������Ҥ�Ѻ
                break;
            case CharState.WalkToMagicCast:
                WalkToMagicCastUpdate();
                break;
        }
    }
}