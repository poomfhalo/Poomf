using UnityEngine;

[RequireComponent(typeof(DodgeballCharacter))]
public class PC : MonoBehaviour
{
    MatchInputController input = null;
    DodgeballCharacter chara = null;
    void OnEnable()
    {
        chara = GetComponent<DodgeballCharacter>();
        input = GetComponent<MatchInputController>();
        ConnectInput();
    }
    void OnDisable()
    {
        DisconnectInput();
    }

    private void ConnectInput()
    {
        input.OnMoveInput += chara.C_MoveInput;
        input.OnEnemy += chara.C_Enemy;
        input.OnFriendly += chara.C_Friendly;
        input.OnBallAction += chara.C_OnBallAction;
        input.OnDodge += chara.C_Dodge;
        input.OnFakeFire += chara.C_FakeFire;
        input.OnJump += chara.C_Jump;
    }
    private void DisconnectInput()
    {
        input.OnMoveInput -= chara.C_MoveInput;
        input.OnEnemy -= chara.C_Enemy;
        input.OnFriendly -= chara.C_Friendly;
        input.OnBallAction -= chara.C_OnBallAction;
        input.OnDodge -= chara.C_Dodge;
        input.OnFakeFire -= chara.C_FakeFire;
        input.OnJump -= chara.C_Jump;
    }
}