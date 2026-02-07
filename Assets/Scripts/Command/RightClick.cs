using UnityEngine;

public class RightClick : MonoBehaviour
{
    public static RightClick instance;

    private Camera cam;
    public LayerMask layerMask;
    private LeftClick leftClick;

    void Awake()
    {
        leftClick = GetComponent<LeftClick>();
    }

    // Start is called once before the first execution of Update
    void Start()
    {
        instance = this;
        cam = Camera.main;
        layerMask = LayerMask.GetMask("Ground", "Character", "Building");
    }

    // Update is called once per frame
    void Update()
    {
        // mouse up
        if (Input.GetMouseButtonUp(1))
        {
            TryCommand(Input.mousePosition);
        }
    }

    private void TryCommand(Vector2 screenPos)
    {
        Ray ray = cam.ScreenPointToRay(screenPos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000, layerMask))
        {
            switch (hit.collider.tag)
            {
                case "Ground":
                    CommandToWalk(hit, leftClick.CurChar);
                    break;
                case "Enemy":
                    CommandToAttack(hit, leftClick.CurChar);
                    break;
            }
        }
    }

    private void CommandToAttack(RaycastHit hit, Character c)
    {
        if (c == null)
            return;

        // ลองดึง Component 'Character' ออกมาจากสิ่งที่เมาส์ชี้โดน
        Character target = hit.collider.GetComponent<Character>();
        Debug.Log("Attack: " + target);

        // ถ้าสิ่งที่คลิกเป็น Character (ไม่ใช่พื้นดิน หรือสิ่งของประกอบฉาก)
        if (target != null)
            c.ToAttackCharacter(target); // สั่งให้ตัวละครของเราเริ่มกระบวนการโจมตี
    }

    private void CreateVFX(Vector3 pos, GameObject vfxPrefab)
    {
        if (vfxPrefab == null)
            return;

        Instantiate(vfxPrefab,
            pos + new Vector3(0f, 0.1f, 0f), Quaternion.identity);
    }


    private void CommandToWalk(RaycastHit hit, Character c)
    {
        if (c != null)
        {
            c.WalkToPosition(hit.point);

            CreateVFX(hit.point, VFXManager.instance.DoubleRingMarker);
        }
    }
}