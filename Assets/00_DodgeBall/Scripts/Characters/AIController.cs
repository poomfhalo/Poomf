using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    [SerializeField] Transform moveTarget = null;
    [SerializeField] float stoppingDist = 0.5f;

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

        Vector3 disp = rb3d.position - moveTarget.position;
        if (disp.magnitude < stoppingDist)
        {
            chara.C_MoveInput(Vector3.zero);
            return;
        }
        disp.y = 0;
        disp.Normalize();
        chara.C_MoveInput(disp);
    }
}