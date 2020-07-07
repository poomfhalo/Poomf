using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class R_SetCharasLock : Reaction,I_R_CharaSupplier
{
    [SerializeField] DodgeballCharacter[] characters = new DodgeballCharacter[0];
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
        foreach (var chara in characters)
        {
            PlayerInput pi = chara.GetComponent<PlayerInput>();
            Mover mover = chara.GetComponent<Mover>();
            switch (command)
            {
                case Command.Lock:
                    pi.enabled = false;
                    mover.movementMode = Mover.MovementType.ToPoint;
                    break;
                case Command.Unlock:
                    pi.enabled = true;
                    mover.movementMode = Mover.MovementType.ByInput;
                    break;
            }
        }
        yield return 0;
    }
    public void RecieveCharacters(DodgeballCharacter[] charas)
    {
        this.characters = charas;
    }
}
