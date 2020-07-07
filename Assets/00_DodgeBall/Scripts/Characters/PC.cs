using UnityEngine;

[RequireComponent(typeof(DodgeballCharacter))]
public class PC : CharaController
{
    MatchInputController input = null;
    [Header("Read Only")]
    [SerializeField] bool isLocked = false;
    public override bool IsLocked { get => isLocked; protected set => isLocked = value; }

    void OnEnable()
    {
        input = GetComponent<MatchInputController>();
        Unlock();
    }
    void OnDisable()
    {
        Lock();
    }

    public override void Lock()
    {
        input.OnMoveInput -= chara.C_MoveInput;
        input.OnEnemy -= chara.C_Enemy;
        input.OnFriendly -= chara.C_Friendly;
        input.OnBallAction -= chara.C_OnBallAction;
        input.OnDodge -= chara.C_Dodge;
        input.OnFakeFire -= chara.C_FakeFire;
        input.OnJump -= chara.C_Jump;
        IsLocked = true;
    }
    public override void Unlock()
    {
        IsLocked = false;
        input.OnMoveInput += chara.C_MoveInput;
        input.OnEnemy += chara.C_Enemy;
        input.OnFriendly += chara.C_Friendly;
        input.OnBallAction += chara.C_OnBallAction;
        input.OnDodge += chara.C_Dodge;
        input.OnFakeFire += chara.C_FakeFire;
        input.OnJump += chara.C_Jump;
    }
}