using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public enum CharState
{
    Idle,
    Walk,
    WalkToEnemy,
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

    // --- ส่วนประกาศตัวแปร (ด้านบนของคลาส) ---
    [SerializeField] protected int curHP = 10;
    public int CurHP { get { return curHP; } }

    [SerializeField]
    protected Character curCharTarget; // ตัวแปรเดิมที่มีอยู่แล้ว

    // เพิ่มบรรทัดนี้เข้าไปต่อท้ายครับ
    public Character CurCharTarget { get { return curCharTarget; } set { curCharTarget = value; } }
    [SerializeField] protected int attackDamage = 3;

    [SerializeField] protected float attackRange = 2f;
    public float AttackRange { get { return attackRange; } }

    [SerializeField] protected float attackCoolDown = 2f;

    [SerializeField] protected float attackTimer = 0f;
    [SerializeField] protected float findingRange = 20f; // ระยะค้นหาศัตรู (เช่น 20 เมตร)
    public float FindingRange { get { return findingRange; } }

    // --- ฟังก์ชันสั่งให้เดินไปโจมตีเป้าหมาย ---
    public void ToAttackCharacter(Character target)
    {
        // ถ้าตายอยู่หรือเลือดหมด ไม่ต้องทำอะไร
        if (curHP <= 0 || state == CharState.Die)
            return;

        // ล็อคเป้าหมาย
        curCharTarget = target;

        // สั่งให้ NavMeshAgent เดินไปยังตำแหน่งของเป้าหมาย
        navAgent.SetDestination(target.transform.position);
        navAgent.isStopped = false;

        // เปลี่ยนสถานะเป็นเดินไปหาศัตรู
        SetState(CharState.WalkToEnemy);
    }

    public bool IsMyEnemy(string targetTag)
    {
        string myTag = gameObject.tag;

        // ถ้าเราเป็น Hero แล้วเจอป้ายชื่อ Enemy -> ใช่ศัตรู
        if ((myTag == "Hero" || myTag == "Player") && targetTag == "Enemy")
            return true;

        // ถ้าเราเป็น Enemy แล้วเจอป้ายชื่อ Hero -> ใช่ศัตรู
        if (myTag == "Enemy" && (targetTag == "Hero" || targetTag == "Player"))
            return true;

        return false;
    }

    protected virtual IEnumerator DestroyObject()
    {
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }

    public void ReceiveDamage(Character enemy)
    {
        // ถ้าตายอยู่แล้ว ไม่ต้องรับดาเมจเพิ่ม
        if (curHP <= 0 || state == CharState.Die)
            return;

        // ลดเลือดตามพลังโจมตีของศัตรู
        curHP -= enemy.attackDamage;

        // ถ้าเลือดหมด
        if (curHP <= 0)
        {
            curHP = 0;
            Die(); // เรียกฟังก์ชันตาย
        }
    }

    protected void AttackLogic()
    {
        // ดึงคอมโพเนนต์ Character ของเป้าหมายออกมา
        Character target = curCharTarget.GetComponent<Character>();

        // ถ้ามีเป้าหมายอยู่จริง ให้สั่งลดเลือด
        if (target != null)
        {
            target.ReceiveDamage(this);
        }
    }

    protected virtual void Die()
    {
        // หยุดการเคลื่อนที่
        navAgent.isStopped = true;
        SetState(CharState.Die);

        // เล่น Animation ท่าตาย (ต้องมี Trigger ชื่อ "Die" ใน Animator)
        anim.SetTrigger("Die");

        // เริ่มนับถอยหลังทำลาย Object (เรียกใช้ฟังก์ชันที่เราเพิ่งเขียนไปก่อนหน้า)
        StartCoroutine(DestroyObject());
    }


    protected void WalkToEnemyUpdate()
    {
        // ถ้าเป้าหมายหายไป ให้กลับไปสถานะ Idle
        if (curCharTarget == null)
        {
            SetState(CharState.Idle);
            return;
        }

        // อัปเดตตำแหน่งเป้าหมายเผื่อศัตรูขยับหนี
        navAgent.SetDestination(curCharTarget.transform.position);

        // คำนวณระยะห่างระหว่างตัวเรากับเป้าหมาย
        float distance = Vector3.Distance(transform.position, curCharTarget.transform.position);

        // ถ้าเข้าระยะโจมตีแล้ว ให้เปลี่ยนเป็นสถานะ Attack
        if (distance <= attackRange)
        {
            SetState(CharState.Attack);
            Attack();
            // //First Attack
        }
    }

    protected void AttackUpdate()
    {
        if (curCharTarget == null)
            return;

        // ถ้าศัตรูตายแล้ว (HP หมด) ให้เราหยุดและกลับไปยืนเฉยๆ
        if (curCharTarget.CurHP <= 0)
        {
            SetState(CharState.Idle);
            return;
        }

        // สั่งให้ NavMeshAgent หยุดเดิน (เพื่อยืนฟัน)
        navAgent.isStopped = true;

        // นับเวลาถอยหลังสำหรับการโจมตีครั้งถัดไป (Cooldown)
        attackTimer += Time.deltaTime;
        if (attackTimer >= attackCoolDown)
        {
            attackTimer = 0f;
            Attack();
        }

        // เช็คระยะห่าง: ถ้าศัตรูหนีออกไปไกลกว่าระยะโจมตี
        float distance = Vector3.Distance(transform.position, curCharTarget.transform.position);

        if (distance > attackRange)
        {
            // เปลี่ยนสถานะกลับไปเดินไล่ตาม
            SetState(CharState.WalkToEnemy);
            navAgent.SetDestination(curCharTarget.transform.position);
            navAgent.isStopped = false;
        }
    }


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

    protected void Attack()
    {
        // สั่งให้ตัวละครหันหน้าไปหาศัตรู
        transform.LookAt(curCharTarget.transform);

        // สั่ง Animator ให้เล่นท่าโจมตี (ต้องมี Parameter ชื่อ "Attack" แบบ Trigger ใน Animator)
        anim.SetTrigger("Attack");
        AttackLogic();

        //attack logic (ตรงนี้เดี๋ยวเราค่อยมาเติมโค้ดคำนวณดาเมจทีหลัง)
    }

    protected void WalkUpdate()
    {
        float distance = Vector3.Distance(transform.position, navAgent.destination);
        Debug.Log(distance);

        if (distance <= navAgent.stoppingDistance)
            SetState(CharState.Idle);
    }
}