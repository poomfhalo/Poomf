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
        foreach (var slot in slots)
        {
            CharaController chara = GameExtentions.GetCharaOfSlot(slot);
            if (chara == null)
                continue;

            switch (command)
            {
                case Command.Lock:
                    chara.GetComponent<CharaController>().Lock();
                    break;
                case Command.Unlock:
                    chara.GetComponent<CharaController>().Unlock();
                    break;
            }
        }
        yield return 0;
    }
    public void RecieveCharacters(int[] charaSlots)=> this.slots = charaSlots;
}