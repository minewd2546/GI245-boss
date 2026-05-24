using UnityEngine;

public class WarpPoint : MonoBehaviour
{
    [SerializeField] private string targetSceneName = "VillageScene";
    [SerializeField] private int targetEnterPointID;

    private void OnTriggerEnter(Collider other)
    {
        if (Settings.IsWarping || Time.time < Settings.WarpDisabledUntil)
            return;

        Hero hero = other.GetComponent<Hero>();
        if (hero == null || GameManager.instance == null || PartyManager.instance == null)
            return;

        if (!PartyManager.instance.HasMember(hero))
            return;

        GameManager.instance.Warp(targetSceneName, targetEnterPointID);
    }
}

