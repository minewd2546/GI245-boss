using UnityEngine;
using UnityEngine.AI;

public class MapManager : MonoBehaviour
{
    [SerializeField] private Transform[] enterPoints;
    [SerializeField] private float formationSpacing = 1.5f;

    public Vector3 GetEnterPointPosition(int index)
    {
        if (TryGetEnterPointPosition(index, out Vector3 position))
            return position;

        return GetFallbackEnterPointPosition();
    }

    private bool TryGetEnterPointPosition(int index, out Vector3 position)
    {
        position = Vector3.zero;

        if (enterPoints == null || enterPoints.Length == 0)
            return false;

        index = Mathf.Clamp(index, 0, enterPoints.Length - 1);
        if (enterPoints[index] != null)
        {
            position = enterPoints[index].position;
            return true;
        }

        for (int i = 0; i < enterPoints.Length; i++)
        {
            if (enterPoints[i] == null)
                continue;

            position = enterPoints[i].position;
            Debug.LogWarning($"Enter point {index} is missing. Using enter point {i} instead.");
            return true;
        }

        return false;
    }

    private Vector3 GetFallbackEnterPointPosition()
    {
        WarpPoint warpPoint = FindObjectOfType<WarpPoint>();
        if (warpPoint != null)
        {
            Debug.LogWarning("MapManager has no assigned enter points. Using a position near a WarpPoint as the fallback spawn position.");
            return warpPoint.transform.position + warpPoint.transform.forward * 2f;
        }

        Debug.LogWarning("MapManager has no assigned enter points or WarpPoint fallback. Using MapManager position.");
        return transform.position;
    }

    private Vector3 GetWalkablePosition(Vector3 position)
    {
        if (NavMesh.SamplePosition(position, out NavMeshHit hit, 3f, NavMesh.AllAreas))
            return hit.position;

        return position;
    }

    public void MovePartyToEnterPoint(int index)
    {
        if (PartyManager.instance == null)
            return;

        // Slightly offset the party spawn position to avoid overlapping teleport trigger colliders
        Vector3 origin = GetEnterPointPosition(index) + new Vector3(0f, 0f, 0.5f);

        for (int i = 0; i < PartyManager.instance.Members.Count; i++)
        {
            Character member = PartyManager.instance.Members[i];
            if (member == null)
                continue;

            Vector3 memberPosition = GetWalkablePosition(origin + new Vector3(i * formationSpacing, 0f, 0f));
            NavMeshAgent agent = member.GetComponent<NavMeshAgent>();
            if (agent != null && agent.enabled && agent.isOnNavMesh)
                agent.Warp(memberPosition);
            else
                member.transform.position = memberPosition;

            member.SetState(CharState.Idle);
        }
    }
}
