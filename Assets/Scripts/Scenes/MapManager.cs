using UnityEngine;

public class MapManager : MonoBehaviour
{
    [SerializeField] private Transform[] enterPoints;
    [SerializeField] private float formationSpacing = 1.5f;

    public Vector3 GetEnterPointPosition(int index)
    {
        if (enterPoints == null || enterPoints.Length == 0)
            return Vector3.zero;

        index = Mathf.Clamp(index, 0, enterPoints.Length - 1);
        return enterPoints[index] != null ? enterPoints[index].position : Vector3.zero;
    }

    public void MovePartyToEnterPoint(int index)
    {
        if (PartyManager.instance == null)
            return;

        Vector3 origin = GetEnterPointPosition(index);

        for (int i = 0; i < PartyManager.instance.Members.Count; i++)
        {
            Character member = PartyManager.instance.Members[i];
            if (member == null)
                continue;

            member.transform.position = origin + new Vector3(i * formationSpacing, 0f, 0f);
            member.SetState(CharState.Idle);
        }
    }
}
