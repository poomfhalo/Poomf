using UnityEngine;

public class FaceCam : MonoBehaviour
{
    Camera cam = null;

    void Awake()
    {
        cam = Camera.main;
    }
    void LateUpdate()
    {
        transform.LookAt(cam.transform);
    }
}