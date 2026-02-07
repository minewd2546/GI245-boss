using UnityEngine;

public class VFXManager : MonoBehaviour
{
    [SerializeField]
    private GameObject doubleRingMarker;
    public GameObject DoubleRingMarker { get { return doubleRingMarker; } }

    public static VFXManager instance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = this;
    }
}