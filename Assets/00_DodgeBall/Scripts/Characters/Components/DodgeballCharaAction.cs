using UnityEngine;

[RequireComponent(typeof(ActionsScheduler))]
public class DodgeballCharaAction : MonoBehaviour
{
    [SerializeField] protected Vector3 recievedInput = Vector3.zero;
    [SerializeField] protected bool isButtonClicked = false;

    public virtual void RecieveInput(Vector3 i)
    {
        recievedInput = i;
    }
    public void RecieveButtonInput(bool i)
    {
        isButtonClicked = i;
    }
}