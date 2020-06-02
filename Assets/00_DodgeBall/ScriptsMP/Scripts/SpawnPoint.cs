using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public Vector3 position => transform.position;
    public Quaternion rotation => transform.rotation;
    public bool HasSpawned => hasSpawned;

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