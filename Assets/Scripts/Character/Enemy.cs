using UnityEngine;

public class Enemy : Character
{
    void Update()
    {
        switch (state)
        {
            case CharState.Walk:
                WalkUpdate();
                break;
        }
    }
}