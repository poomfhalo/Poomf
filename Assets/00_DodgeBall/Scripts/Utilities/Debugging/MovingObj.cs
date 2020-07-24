using UnityEngine;

public class MovingObj : MonoBehaviour
{
    [SerializeField] float moveSpeed = 40;

    void Update()
    {
        transform.position = transform.position + transform.forward * moveSpeed * Time.deltaTime;
    }
}