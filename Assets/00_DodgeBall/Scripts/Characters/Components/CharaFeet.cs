using UnityEngine;

public class CharaFeet : MonoBehaviour
{
    [SerializeField] float force = 5;
    [SerializeField] float timeBetweenPushes = 0.06f;

    Vector3 lastPos = new Vector3();
    Rigidbody rb3d = null;
    Vector3 travelDir = Vector3.zero;
    float counter = 0;
    void Start()
    {
        rb3d = GetComponent<Rigidbody>();
        lastPos = transform.position;
    }
    void FixedUpdate()
    {
        travelDir = transform.position - lastPos;
        travelDir.Normalize();
        lastPos = transform.position;
        counter += Time.fixedDeltaTime/timeBetweenPushes;
        if (counter > 1)
            counter = 1;
    }
    private void OnTriggerStay(Collider col)
    {
        Rigidbody ballRB = col.GetComponent<Rigidbody>();
        if (ballRB)
        {
            if (counter < 1)
                return;

            ballRB.AddForce(travelDir * force, ForceMode.Impulse);
            counter = 0;
        }
    }
}