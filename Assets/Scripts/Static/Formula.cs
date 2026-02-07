using UnityEngine;

public static class Formula
{
    public static Character FindClosestEnemyChar(Character me)
    {
        LayerMask charLayer = LayerMask.GetMask("Character");
        Character closestTarget = null;
        float closestDist = 0f;

        // ยิง SphereCast เพื่อหาวัตถุรอบตัว (ใช้ FindingRange เป็นรัศมี)
        RaycastHit[] hits = Physics.SphereCastAll(me.transform.position,
                                                  me.FindingRange,
                                                  Vector3.up,
                                                  charLayer);

        for (int i = 0; i < hits.Length; i++)
        {
            Character target = hits[i].collider.GetComponent<Character>();

            // กรองตัวที่ไม่ใช่ออก: เป็น Null, ตายแล้ว, หรือเป็นตัวเราเอง
            if (target == null || target.CurHP <= 0 || target == me)
                continue;

            // ถ้าไม่ใช่ศัตรู (เป็นพวกเดียวกัน) ก็ข้ามไป
            if (!me.IsMyEnemy(target.tag))
                continue;

            float distance = Vector3.Distance(me.transform.position,
                                              hits[i].transform.position);

            // เก็บค่าตัวที่ใกล้ที่สุด
            if (closestTarget == null || distance < closestDist)
            {
                closestTarget = target;
                closestDist = distance;
            }
        }

        return closestTarget;
    }
}
