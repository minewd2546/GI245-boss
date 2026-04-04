using UnityEngine;

public class Hero : Character
{
    protected void WalkToNPCUpdate()
    {
        float distance = Vector3.Distance(transform.position,
            curCharTarget.transform.position);

        if (distance <= 2f)
        {
            navAgent.isStopped = true;
            SetState(CharState.Idle);

            Npc npc = curCharTarget.GetComponent<Npc>();

            uiManager.PrepareDialogueBox(npc);
        }
    }

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
            case CharState.WalkToNPC:
                WalkToNPCUpdate();
                break;
        }
    }
}
