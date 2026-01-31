using UnityEngine;
using UnityEngine.AI;

public enum CharState
{
    Idle,
    Walk,
    Attack,
    Hit,
    Die
}

public abstract class Character : MonoBehaviour
{
    protected NavMeshAgent navAgent;

    protected Animator anim;
    public Animator Anim { get { return anim; } }

    [SerializeField]
    protected CharState state;
    public CharState State { get { return state; } }

    [SerializeField]
    protected GameObject ringSelection;
    public GameObject RingSelection { get { return ringSelection; } }

    public void ToggleRingSelection(bool flag)
    {
        // Add this 'if' statement to check if ringSelection exists before using it
        if (ringSelection != null)
        {
            ringSelection.SetActive(flag);
        }
        else
        {
            Debug.LogWarning("Ring Selection is not assigned on " + gameObject.name);
        }
    }

    void Awake()
    {
        navAgent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }

    // เพิ่มส่วนนี้: เพื่อให้สถานะ Walk ทำงานในทุกเฟรม

    // เพิ่มส่วนนี้: เพื่อให้ RightClick.cs สามารถสั่งให้ตัวละครเดินได้ห
    public void WalkToPosition(Vector3 position)
    {
        navAgent.isStopped = false; // สั่งให้ Agent เริ่มทำงานใหม่
        navAgent.SetDestination(position);
        SetState(CharState.Walk);
    }

    public void SetState(CharState s)
    {
        state = s;

        // เพิ่มส่วนนี้ตามหัวข้อ 12.9
        if (state == CharState.Idle)
        {
            navAgent.isStopped = true; // หยุดการเคลื่อนที่
            navAgent.ResetPath();      // ล้างเส้นทางเดิม
        }
    }

    protected void WalkUpdate()
    {
        float distance = Vector3.Distance(transform.position, navAgent.destination);
        Debug.Log(distance);

        if (distance <= navAgent.stoppingDistance)
            SetState(CharState.Idle);
    }
}