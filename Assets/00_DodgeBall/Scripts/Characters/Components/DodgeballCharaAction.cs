using UnityEngine;

[RequireComponent(typeof(ActionsScheduler))]
public class DodgeballCharaAction : MonoBehaviour
{
    public Vector3 recievedInput = Vector3.zero;
    public virtual void RecieveInput(Vector3 i) => recievedInput = i;
}