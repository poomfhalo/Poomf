using System.Collections;
using UnityEngine;

public class R_SetCharasLock : Reaction,I_R_CharaSupplier
{
    [SerializeField] int[] slots = new int[0];
    [SerializeField] Command command = Command.Lock;

    public enum Command { Lock,Unlock }

    protected override void Initialize()
    {

    }
    protected override bool IsDone()
    {
        return true;
    }
    protected override IEnumerator ReactionBehavior()
    {
        bool lockState = false;
        if (command == Command.Lock)
            lockState = true;
        else if (command == Command.Unlock)
            lockState = false;

        GameExtentions.SetCharasLock(lockState, slots);
        yield return 0;
    }
    public void RecieveCharacters(int[] charaSlots)=> this.slots = charaSlots;
}