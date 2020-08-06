using UnityEngine;

public class DieAfter : MonoBehaviour
{
    public void Begin(float time)
    {
        Destroy(gameObject, time);
    }
}
