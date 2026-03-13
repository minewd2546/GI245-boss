using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum CharState
{
    Idle,
    Walk,
    WalkToEnemy,
    Attack,
    WalkToMagicCast,
    MagicCast,
    Hit,
    Die
}


public abstract class Character : MonoBehaviour
{
    protected NavMeshAgent navAgent;

    protected Animator anim;
    public Animator Anim { get { return anim; } }

    [SerializeField]
    protected List<Magic> magicSkills = new List<Magic>();
    public List<Magic> MagicSkills
    { get { return magicSkills; } set { magicSkills = value; } }

    [SerializeField]
    protected Magic curMagicCast = null;
    public Magic CurMagicCast
    { get { return curMagicCast; } set { curMagicCast = value; } }

    [SerializeField]
    protected bool isMagicMode = false;
    public bool IsMagicMode
    { get { return isMagicMode; } set { isMagicMode = value; } }
	
	[Header("Inventory")]

	[SerializeField]
	protected Item[] inventoryItems;
	public Item[] InventoryItems
	{
    get { return inventoryItems; }
    set { inventoryItems = value; }
	}

	[SerializeField]
	protected Item mainWeapon;
	public Item MainWeapon
	{
    get { return mainWeapon; }
    set { mainWeapon = value; }
	}

	[SerializeField]
	protected Item shield;
	public Item Shield
	{
    get { return shield; }
    set { shield = value; }
	}

    protected VFXManager vfxManager;
    protected UIManager uiManager;
    
    [SerializeField]
    protected CharState state;
    public CharState State { get { return state; } }

    [SerializeField]
    protected GameObject ringSelection;
    public GameObject RingSelection { get { return ringSelection; } }

    
    [SerializeField] protected int curHP = 10;
    public int CurHP { get { return curHP; } }

    [SerializeField]
    protected Character curCharTarget; 

    
    public Character CurCharTarget { get { return curCharTarget; } set { curCharTarget = value; } }
    [SerializeField] protected int attackDamage = 3;

    [SerializeField] protected float attackRange = 2f;
    public float AttackRange { get { return attackRange; } }

    [SerializeField] protected float attackCoolDown = 2f;

    [SerializeField] protected float attackTimer = 0f;
    [SerializeField] protected float findingRange = 20f; // ���Ф����ѵ�� (�� 20 ����)
    public float FindingRange { get { return findingRange; } }

    
	
    public void ToAttackCharacter(Character target)
    {
        
        if (curHP <= 0 || state == CharState.Die)
            return;

        // ��ͤ�������
        curCharTarget = target;

        // ������ NavMeshAgent �Թ��ѧ���˹觢ͧ�������
        navAgent.SetDestination(target.transform.position);
        navAgent.isStopped = false;
        
        if (isMagicMode)
            SetState(CharState.WalkToMagicCast);
        else
            SetState(CharState.WalkToEnemy);
    }

    public bool IsMyEnemy(string targetTag)
    {
        string myTag = gameObject.tag;

        // �������� Hero �����ͻ��ª��� Enemy -> ���ѵ��
        if ((myTag == "Hero" || myTag == "Player") && targetTag == "Enemy")
            return true;

        // �������� Enemy �����ͻ��ª��� Hero -> ���ѵ��
        if (myTag == "Enemy" && (targetTag == "Hero" || targetTag == "Player"))
            return true;

        return false;
    }

    protected virtual IEnumerator DestroyObject()
    {
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }

    public void ReceiveDamage(int damage)
    {
        // ��ҵ���������� ����ͧ�Ѻ���������
        if (curHP <= 0 || state == CharState.Die)
            return;

        // Ŵ���ʹ�����ѧ���բͧ�ѵ��
        curHP -= damage;

        // ������ʹ���
        if (curHP <= 0)
        {
            curHP = 0;
            Die(); // ���¡�ѧ��ѹ���
        }
    }
    
    public void charInit(VFXManager vfxM , UIManager uiM)
    {
        vfxManager = vfxM;
        uiManager = uiM;
		
		inventoryItems = new Item[16];
    }
    
    protected void AttackLogic()
    {
        // �֧����๹�� Character �ͧ��������͡��
        Character target = curCharTarget.GetComponent<Character>();

        // �����������������ԧ ������Ŵ���ʹ
        if (target != null)
        {
            target.ReceiveDamage(attackDamage);
        }
    }
    
    protected void MagicCastLogic(Magic magic)
    {
        Character target = curCharTarget.GetComponent<Character>();

        if (target != null)
            target.ReceiveDamage(magic.Power);
    }
    
    protected virtual void Die()
    {
        // ��ش�������͹���
        navAgent.isStopped = true;
        SetState(CharState.Die);

        // ��� Animation ��ҵ�� (��ͧ�� Trigger ���� "Die" � Animator)
        anim.SetTrigger("Die");

        // ������Ѻ�����ѧ����� Object (���¡��ѧ��ѹ�����������¹仡�͹˹��)
        StartCoroutine(DestroyObject());
    }


    protected void WalkToEnemyUpdate()
    {
        // �������������� ����Ѻ�ʶҹ� Idle
        if (curCharTarget == null)
        {
            SetState(CharState.Idle);
            return;
        }

        // �ѻവ���˹�������������ѵ�٢�Ѻ˹�
        navAgent.SetDestination(curCharTarget.transform.position);

        // �ӹǳ������ҧ�����ҧ�����ҡѺ�������
        float distance = Vector3.Distance(transform.position, curCharTarget.transform.position);

        // ������������������ �������¹��ʶҹ� Attack
        if (distance <= attackRange)
        {
            SetState(CharState.Attack);
            Attack();
            // //First Attack
        }
    }
    
	private IEnumerator ShootMagicCast(Magic curMagicCast)
    {
        // เพิ่มความสูงให้จุดยิง 0.5f 
        Vector3 chestOffset = new Vector3(0, 0.5f, 0);
        Vector3 startPos = transform.position + chestOffset;
        Vector3 targetPos = curCharTarget.transform.position + chestOffset;

        if (vfxManager != null)
            vfxManager.ShootMagic(curMagicCast.ShootID,
                startPos, 
                targetPos, 
                curMagicCast.ShootTime);

        yield return new WaitForSeconds(curMagicCast.ShootTime);

        //cast logic
        MagicCastLogic(curMagicCast);
        isMagicMode = false;

        SetState(CharState.Idle);
        if (uiManager != null)
            uiManager.IsOnCurToggleMagic(false);
    }

	private IEnumerator LoadMagicCast(Magic curMagicCast)
    {
        // เพิ่มความสูงให้จุดโหลดเวทย์ 0.5f
        Vector3 chestOffset = new Vector3(0, 0.5f, 0);
        Vector3 startPos = transform.position + chestOffset;

        if (vfxManager != null)
            vfxManager.LoadMagic(curMagicCast.LoadID,
                startPos, 
                curMagicCast.LoadTime);

        yield return new WaitForSeconds(curMagicCast.LoadTime);

        StartCoroutine(ShootMagicCast(curMagicCast));
    }

    private void MagicCast(Magic curMagicCast)
    {
        transform.LookAt(curCharTarget.transform);
        anim.SetTrigger("MagicAttack");

        StartCoroutine(LoadMagicCast(curMagicCast));
    }
    
    protected void WalkToMagicCastUpdate()
    {
        if (curCharTarget == null || curMagicCast == null)
        {
            SetState(CharState.Idle);
            return;
        }

        navAgent.SetDestination(curCharTarget.transform.position);

        float distance = Vector3.Distance(transform.position,
            curCharTarget.transform.position);

        if (distance <= curMagicCast.Range)
        {
            navAgent.isStopped = true;
            SetState(CharState.MagicCast);

            MagicCast(curMagicCast);
        }
    }
    protected void AttackUpdate()
    {
        if (curCharTarget == null)
            return;

        // ����ѵ�ٵ������ (HP ���) ��������ش��С�Ѻ��׹���
        if (curCharTarget.CurHP <= 0)
        {
            SetState(CharState.Idle);
            return;
        }

        // ������ NavMeshAgent ��ش�Թ (�����׹�ѹ)
        navAgent.isStopped = true;

        // �Ѻ���Ҷ����ѧ����Ѻ������դ��駶Ѵ� (Cooldown)
        attackTimer += Time.deltaTime;
        if (attackTimer >= attackCoolDown)
        {
            attackTimer = 0f;
            Attack();
        }

        // ��������ҧ: ����ѵ��˹��͡��š�����������
        float distance = Vector3.Distance(transform.position, curCharTarget.transform.position);

        if (distance > attackRange)
        {
            // ����¹ʶҹС�Ѻ��Թ�����
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

    // ������ǹ���: �������ʶҹ� Walk �ӧҹ㹷ء���

    // ������ǹ���: ������� RightClick.cs ����ö���������Ф��Թ���
    public void WalkToPosition(Vector3 position)
    {
        navAgent.isStopped = false; // ������ Agent ������ӧҹ����
        navAgent.SetDestination(position);
        SetState(CharState.Walk);
    }

    public void SetState(CharState s)
    {
        state = s;

        // ������ǹ�������Ǣ�� 12.9
        if (state == CharState.Idle)
        {
            navAgent.isStopped = true; // ��ش�������͹���
            navAgent.ResetPath();      // ��ҧ��鹷ҧ���
        }
    }

    protected void Attack()
    {
        // ���������Ф��ѹ˹������ѵ��
        transform.LookAt(curCharTarget.transform);

        // ��� Animator �����蹷������ (��ͧ�� Parameter ���� "Attack" Ẻ Trigger � Animator)
        anim.SetTrigger("Attack");
        AttackLogic();

        //attack logic (�ç����������Ҥ���������鴤ӹǳ���������ѧ)
    }

    protected void WalkUpdate()
    {
        float distance = Vector3.Distance(transform.position, navAgent.destination);
        Debug.Log(distance);

        if (distance <= navAgent.stoppingDistance)
            SetState(CharState.Idle);
    }
}