using UnityEngine;

public abstract class CharaController : MonoBehaviour
{
    public DodgeballCharacter chara => GetComponent<DodgeballCharacter>();
    public abstract bool IsLocked { protected set; get; }
    public abstract void Lock();
    public abstract void Unlock();

    public void SetLockTo(bool newState)
    {
        if (newState)
            Lock();
        else
            Unlock();
    }
}