using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public Vector3 position => transform.position;
    public Quaternion rotation => transform.rotation;
    public bool HasSpawned => hasSpawned;

    [Header("Read Only")]
    [SerializeField] bool hasSpawned = false;

    public void Fill()
    {
        hasSpawned = true;
    }
    public void Free()
    {
        hasSpawned = false;
    }
}