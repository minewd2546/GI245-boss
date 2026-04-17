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
    Die,
    WalkToNPC
}

public abstract class Character : MonoBehaviour
{
    protected NavMeshAgent navAgent;
    protected Animator anim;

    public Animator Anim => anim;

    [SerializeField] protected Sprite avatarPic;
    public Sprite AvatarPic => avatarPic;

    [SerializeField] protected string charName;
    public string CharName => charName;

    [SerializeField] protected List<Magic> magicSkills = new List<Magic>();
    public List<Magic> MagicSkills
    {
        get => magicSkills;
        set => magicSkills = value;
    }

    [SerializeField] protected Magic curMagicCast;
    public Magic CurMagicCast
    {
        get => curMagicCast;
        set => curMagicCast = value;
    }

    [SerializeField] protected bool isMagicMode;
    public bool IsMagicMode
    {
        get => isMagicMode;
        set => isMagicMode = value;
    }

    [Header("Inventory")]
    [SerializeField] protected Item[] inventoryItems;
    public Item[] InventoryItems
    {
        get => inventoryItems;
        set => inventoryItems = value;
    }

    [SerializeField] protected Item mainWeapon;
    public Item MainWeapon
    {
        get => mainWeapon;
        set => mainWeapon = value;
    }

    [SerializeField] protected Transform weaponHand;
    [SerializeField] protected GameObject weaponObj;

    [SerializeField] protected Item shield;
    public Item Shield
    {
        get => shield;
        set => shield = value;
    }

    [SerializeField] protected Transform shieldHand;
    [SerializeField] protected GameObject shieldObj;

    [SerializeField] protected int defensePower;
    public int DefensePower
    {
        get => defensePower;
        set => defensePower = Mathf.Max(0, value);
    }

    protected VFXManager vfxManager;
    protected UIManager uiManager;
    protected InventoryManager invManager;
    protected PartyManager partyManager;

    [SerializeField] protected CharState state;
    public CharState State => state;

    [SerializeField] protected GameObject ringSelection;
    public GameObject RingSelection => ringSelection;

    [SerializeField] protected int curHP = 10;
    public int CurHP
    {
        get => curHP;
        set => curHP = Mathf.Clamp(value, 0, maxHP);
    }

    [SerializeField] protected int maxHP = 100;
    public int MaxHP
    {
        get => maxHP;
        set => maxHP = Mathf.Max(1, value);
    }

    [SerializeField] protected Character curCharTarget;
    public Character CurCharTarget
    {
        get => curCharTarget;
        set => curCharTarget = value;
    }

    [SerializeField] protected int attackDamage = 3;
    public int AttackDamage
    {
        get => attackDamage;
        set => attackDamage = Mathf.Max(0, value);
    }

    [SerializeField] protected float attackRange = 2f;
    public float AttackRange => attackRange;

    [SerializeField] protected float attackCoolDown = 2f;
    [SerializeField] protected float attackTimer;
    [SerializeField] protected float findingRange = 20f;
    public float FindingRange => findingRange;

    public void ToAttackCharacter(Character target)
    {
        if (target == null || curHP <= 0 || state == CharState.Die)
            return;

        curCharTarget = target;
        navAgent.SetDestination(target.transform.position);
        navAgent.isStopped = false;

        if (isMagicMode)
            SetState(CharState.WalkToMagicCast);
        else
            SetState(CharState.WalkToEnemy);
    }

    public void ToTalkToNPC(Character npc)
    {
        if (npc == null || curHP <= 0 || state == CharState.Die)
            return;

        curCharTarget = npc;
        navAgent.SetDestination(npc.transform.position);
        navAgent.isStopped = false;
        SetState(CharState.WalkToNPC);
    }

    public bool IsMyEnemy(string targetTag)
    {
        string myTag = gameObject.tag;

        if ((myTag == "Hero" || myTag == "Player") && targetTag == "Enemy")
            return true;

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
        if (curHP <= 0 || state == CharState.Die)
            return;

        int damageAfter = damage - defensePower;
        if (damageAfter < 0)
            damageAfter = 0;

        curHP -= damageAfter;

        if (curHP <= 0)
        {
            curHP = 0;
            Die();
        }
    }

    public void Recover(int n)
    {
        curHP += n;
        if (curHP > maxHP)
            curHP = maxHP;
    }

    public void EquipShield(Item item)
    {
        if (item == null || invManager == null || invManager.ItemPrefabs == null)
            return;

        if (item.PrefabID < 0 || item.PrefabID >= invManager.ItemPrefabs.Length)
        {
            Debug.LogError("Shield prefabID is out of range: " + item.PrefabID);
            return;
        }

        if (shieldHand == null)
        {
            Debug.LogError("ShieldHand is not assigned on " + gameObject.name);
            return;
        }

        UnEquipShield();

        shieldObj = Instantiate(invManager.ItemPrefabs[item.PrefabID], shieldHand);
        shieldObj.transform.localPosition = new Vector3(-8.5f, -4f, 3f);
        shieldObj.transform.Rotate(-90f, 0f, 180f, Space.Self);

        defensePower += item.Power;
        shield = item;
    }

    public void EquipWeapon(Item item)
    {
        if (item == null || invManager == null || invManager.ItemPrefabs == null)
            return;

        if (item.PrefabID < 0 || item.PrefabID >= invManager.ItemPrefabs.Length)
        {
            Debug.LogError("Weapon prefabID is out of range: " + item.PrefabID);
            return;
        }

        if (weaponHand == null)
        {
            Debug.LogError("WeaponHand is not assigned on " + gameObject.name);
            return;
        }

        UnEquipWeapon();

        weaponObj = Instantiate(invManager.ItemPrefabs[item.PrefabID], weaponHand);
        weaponObj.transform.localPosition = Vector3.zero;
        attackDamage += item.Power;
        mainWeapon = item;
    }

    public void UnEquipShield()
    {
        if (shield == null)
            return;

        defensePower -= shield.Power;
        shield = null;

        if (shieldObj != null)
            Destroy(shieldObj);
    }

    public void UnEquipWeapon()
    {
        if (mainWeapon == null)
            return;

        attackDamage -= mainWeapon.Power;
        mainWeapon = null;

        if (weaponObj != null)
            Destroy(weaponObj);
    }

    public virtual void CharInit(VFXManager vfxM, UIManager uiM, InventoryManager invM, PartyManager partyM)
    {
        vfxManager = vfxM;
        uiManager = uiM;
        invManager = invM;
        partyManager = partyM;

        if (inventoryItems == null || inventoryItems.Length != InventoryManager.MAXSLOT)
            inventoryItems = new Item[InventoryManager.MAXSLOT];
    }

    protected void AttackLogic()
    {
        Character target = curCharTarget;
        if (target != null)
            target.ReceiveDamage(attackDamage);
    }

    protected void MagicCastLogic(Magic magic)
    {
        Character target = curCharTarget;
        if (target != null)
            target.ReceiveDamage(magic.Power);
    }

    protected virtual void Die()
    {
        navAgent.isStopped = true;
        SetState(CharState.Die);
        anim.SetTrigger("Die");

        if (invManager != null)
            invManager.SpawnDropInventory(inventoryItems, transform.position);

        StartCoroutine(DestroyObject());
    }

    protected void WalkToEnemyUpdate()
    {
        if (curCharTarget == null)
        {
            SetState(CharState.Idle);
            return;
        }

        navAgent.SetDestination(curCharTarget.transform.position);
        float distance = Vector3.Distance(transform.position, curCharTarget.transform.position);

        if (distance <= attackRange)
        {
            SetState(CharState.Attack);
            Attack();
        }
    }

    private IEnumerator ShootMagicCast(Magic magic)
    {
        Vector3 chestOffset = new Vector3(0f, 0.5f, 0f);
        Vector3 startPos = transform.position + chestOffset;
        Vector3 targetPos = curCharTarget.transform.position + chestOffset;

        if (vfxManager != null)
            vfxManager.ShootMagic(magic.ShootID, startPos, targetPos, magic.ShootTime);

        yield return new WaitForSeconds(magic.ShootTime);

        MagicCastLogic(magic);
        isMagicMode = false;

        SetState(CharState.Idle);
        if (uiManager != null)
            uiManager.IsOnCurToggleMagic(false);
    }

    private IEnumerator LoadMagicCast(Magic magic)
    {
        Vector3 chestOffset = new Vector3(0f, 0.5f, 0f);
        Vector3 startPos = transform.position + chestOffset;

        if (vfxManager != null)
            vfxManager.LoadMagic(magic.LoadID, startPos, magic.LoadTime);

        yield return new WaitForSeconds(magic.LoadTime);
        StartCoroutine(ShootMagicCast(magic));
    }

    private void MagicCast(Magic magic)
    {
        transform.LookAt(curCharTarget.transform);
        anim.SetTrigger("MagicAttack");
        StartCoroutine(LoadMagicCast(magic));
    }

    protected void WalkToMagicCastUpdate()
    {
        if (curCharTarget == null || curMagicCast == null)
        {
            SetState(CharState.Idle);
            return;
        }

        navAgent.SetDestination(curCharTarget.transform.position);
        float distance = Vector3.Distance(transform.position, curCharTarget.transform.position);

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

        if (curCharTarget.CurHP <= 0)
        {
            SetState(CharState.Idle);
            return;
        }

        navAgent.isStopped = true;

        attackTimer += Time.deltaTime;
        if (attackTimer >= attackCoolDown)
        {
            attackTimer = 0f;
            Attack();
        }

        float distance = Vector3.Distance(transform.position, curCharTarget.transform.position);
        if (distance > attackRange)
        {
            SetState(CharState.WalkToEnemy);
            navAgent.SetDestination(curCharTarget.transform.position);
            navAgent.isStopped = false;
        }
    }

    public void ToggleRingSelection(bool flag)
    {
        if (ringSelection != null)
            ringSelection.SetActive(flag);
    }

    void Awake()
    {
        navAgent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }

    public void WalkToPosition(Vector3 position)
    {
        navAgent.isStopped = false;
        navAgent.SetDestination(position);
        SetState(CharState.Walk);
    }

    public void SetState(CharState s)
    {
        state = s;

        if (state == CharState.Idle)
        {
            navAgent.isStopped = true;
            navAgent.ResetPath();
        }
    }

    protected void Attack()
    {
        if (curCharTarget == null)
            return;

        transform.LookAt(curCharTarget.transform);
        anim.SetTrigger("Attack");
        AttackLogic();
    }

    protected void WalkUpdate()
    {
        float distance = Vector3.Distance(transform.position, navAgent.destination);
        if (distance <= navAgent.stoppingDistance)
            SetState(CharState.Idle);
    }
}
