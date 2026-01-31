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
        }
    }
}