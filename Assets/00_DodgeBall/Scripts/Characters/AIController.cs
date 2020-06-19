using UnityEngine;

public class AIController : MonoBehaviour
{
    [SerializeField] Transform moveTarget = null;
    [SerializeField] bool useWarp = false;
    [Header("Read Only")]
    [SerializeField] float lastDist = 0;
    DodgeballCharacter chara = null;
    Rigidbody rb3d = null;

    void Start()
    {
        chara = GetComponent<DodgeballCharacter>();
        rb3d = GetComponent<Rigidbody>();
    }
    void Update()
    {
        if (!moveTarget)
            return;

        Vector3 disp = moveTarget.position - rb3d.position;
        disp.y = 0;
        lastDist = disp.magnitude;
        if(useWarp)
        {
            //GetComponent<Mover>().Warp(moveTarget.position);
            transform.position = moveTarget.position;
        }
        else
        {
            chara.C_MoveInput(moveTarget.position);
        }
    }
}