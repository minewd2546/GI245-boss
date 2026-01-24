using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Camera cam;

    [Header("Move")]
    [SerializeField] private float moveSpeed;

    [SerializeField] private Transform corner1;
    [SerializeField] private Transform corner2;

    [SerializeField] private float xInput;
    [SerializeField] private float zInput;

    public static CameraController instance;

    [Header("Zoom")]
    [SerializeField] private float zoomModifier;

    void Awake()
    {
        instance = this;
        cam = Camera.main;
    }

    void Start()
    {
        moveSpeed = 50;
    }

    private void Zoom()
    {
        zoomModifier = Input.GetAxis("Mouse ScrollWheel");
        if (Input.GetKey(KeyCode.Z))
            zoomModifier = -0.1f;
        if (Input.GetKey(KeyCode.X))
            zoomModifier = 0.1f;

        cam.orthographicSize += zoomModifier;
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, 4, 10);
    }

    private Vector3 Clamp(Vector3 lowerLeft, Vector3 topRight)
    {
        Vector3 pos = new Vector3(Mathf.Clamp(transform.position.x, lowerLeft.x, topRight.x),
        transform.position.y, Mathf.Clamp(transform.position.z, lowerLeft.z, topRight.z));

        return pos;
    }

    private void MoveByKB()
    {
        xInput = Input.GetAxis("Horizontal");
        zInput = Input.GetAxis("Vertical");

        Vector3 dir = (transform.forward * zInput) + (transform.right * xInput);

        transform.position += dir * moveSpeed * Time.deltaTime;
        
        // แก้คำผิดจาก posittion เป็น position
        transform.position = Clamp(corner1.position, corner2.position); 
    }

    // ส่วนที่ทำตาม Challenge ข้อ 6.29 (หน้า 28)
    private void MoveByMouse()
    {
        // 1. ขวา (ตามตัวอย่างข้อ 6.26)
        if (Input.mousePosition.x >= Screen.width)
            transform.Translate(Vector3.right * moveSpeed * Time.deltaTime, Space.World);

        // 2. ซ้าย (เพิ่มเองตามโจทย์: x น้อยกว่าหรือเท่ากับ 0)
        if (Input.mousePosition.x <= 0)
            transform.Translate(Vector3.left * moveSpeed * Time.deltaTime, Space.World);

        // 3. บน (เพิ่มเองตามโจทย์: y มากกว่าหรือเท่ากับความสูงจอ)
        if (Input.mousePosition.y >= Screen.height)
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime, Space.World);

        // 4. ล่าง (เพิ่มเองตามโจทย์: y น้อยกว่าหรือเท่ากับ 0)
        if (Input.mousePosition.y <= 0)
            transform.Translate(Vector3.back * moveSpeed * Time.deltaTime, Space.World);
    }

    void Update()
    {
        MoveByKB();
        Zoom();
        MoveByMouse();
    }
}