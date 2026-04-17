using UnityEngine;

public class Enemy : Character
{
    [SerializeField] private int expDrop = 10;
    public int ExpDrop => expDrop;

    protected override void Die()
    {
        if (partyManager != null)
            partyManager.ShareExpToParty(expDrop);

        base.Die();
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
                AttackUpdate();
                break;
        }
    }
}
