using UnityEngine;
using System.Collections;

public class AttackAI : MonoBehaviour
{
    private Character myChar;

    [SerializeField]
    private Character curEnemy;

    void Start()
    {
        myChar = GetComponent<Character>();

        // สั่งให้ทำงานซ้ำๆ ทุก 1 วินาที (เพื่อไม่ให้กินเครื่องมากเกินไป)
        if (myChar != null)
        {
            InvokeRepeating("FindAndAttackEnemy", 0f, 1f);
        }
    }

    private void FindAndAttackEnemy()
    {
        if (myChar.CurCharTarget == null)
        {
            curEnemy = Formula.FindClosestEnemyChar(myChar);
            if (curEnemy == null)
                return;

            if (myChar.IsMyEnemy(curEnemy.gameObject.tag))
                myChar.ToAttackCharacter(curEnemy);
        }
    }
}