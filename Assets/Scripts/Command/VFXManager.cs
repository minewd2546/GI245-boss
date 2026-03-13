using UnityEngine;

public class VFXManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] magicVFX;
    public GameObject[] MagicVFX { get { return magicVFX; } }
    
    [SerializeField]
    private MagicData[] magicData;
    public MagicData[] MagicData { get { return magicData; } }
    
    [SerializeField]
    private GameObject doubleRingMarker;
    public GameObject DoubleRingMarker { get { return doubleRingMarker; } }

    public static VFXManager instance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;
    }
    
    public void LoadMagic(int id, Vector3 posA, float time)
    {
        //Load Magic
        if (magicVFX[id] == null)
            return;

        GameObject objLoad = Instantiate(magicVFX[id], posA, Quaternion.identity);
        Destroy(objLoad, time);
    }

	public void ShootMagic(int id, Vector3 posA, Vector3 posB, float time)
    {
        if (magicVFX[id] == null) return;

        // 1. สร้างเอฟเฟกต์ที่จุดยิง
        GameObject objShoot = Instantiate(magicVFX[id], posA, Quaternion.identity);
        objShoot.transform.LookAt(posB);

        // 2. เรียก Coroutine สั่งให้เอฟเฟกต์ลอยไปหาเป้าหมาย
        StartCoroutine(MoveVFX(objShoot, posA, posB, time));

        // 3. ทำลายทิ้ง: เวลาที่ลอยไป (time) + เวลาที่อยากให้ค้างอยู่ที่ตัวศัตรู (1.5 วินาที)
        Destroy(objShoot, time + 1.5f); 
    }

    // ✨ ฟังก์ชันใหม่: ตัวนี้จะคอยขยับเอฟเฟกต์ให้พุ่งไปหาเป้าหมาย
    private System.Collections.IEnumerator MoveVFX(GameObject vfx, Vector3 startPos, Vector3 targetPos, float duration)
    {
        float timer = 0f;

        // ลูปขยับตำแหน่งจนกว่าจะถึงเป้าหมาย
        while (timer < duration)
        {
            if (vfx == null) yield break;

            timer += Time.deltaTime;
            float percent = timer / duration;

            // ค่อยๆ เลื่อนตำแหน่งจากตัวเรา ไปหาศัตรู
            vfx.transform.position = Vector3.Lerp(startPos, targetPos, percent);

            yield return null; // รอขยับต่อในเฟรมหน้า
        }

        // พอพุ่งมาถึงตัวศัตรูแล้ว ลูปจะจบลง
        // เอฟเฟกต์จะหยุดนิ่งอยู่ที่ตำแหน่งเป้าหมาย (ค้างที่ตัวศัตรู)
        if (vfx != null)
        {
            vfx.transform.position = targetPos;
        }
    }

    //public void ShootMagic(int id, Vector3 posA, Vector3 posB, float time)
    //{
        //Shoot Magic
     //   if (magicVFX[id] == null)
     //       return;

    //    GameObject objShoot = Instantiate(magicVFX[id], posA, Quaternion.identity);
    //    objShoot.transform.position = Vector3.LerpUnclamped(posA, posB, time);
    //    Destroy(objShoot, time);
    //}
}