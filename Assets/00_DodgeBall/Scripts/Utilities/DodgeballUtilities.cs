using UnityEngine;

public class DodgeballUtilities : MonoBehaviour
{
    [SerializeField] float posCheckDist = 0.5f;
    public void SetPosition(Transform to)
    {
        float testDist = posCheckDist + GetComponent<SphereCollider>().radius;
        if (Vector3.Distance(to.position, transform.position) >= testDist)
        {
            transform.position = to.position;
            transform.rotation = to.rotation;
        }
    }
}
