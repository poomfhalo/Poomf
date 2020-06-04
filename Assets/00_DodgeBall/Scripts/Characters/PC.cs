using UnityEngine;

[RequireComponent(typeof(DodgeballCharacter))]
public class PC : MonoBehaviour
{
    DodgeballCharacter chara = null;
    void OnEnable()
    {
        chara = GetComponent<DodgeballCharacter>();
        ConnectInput();
    }
    void OnDisable()
    {
        DisconnectInput();
    }

    private void DisconnectInput()
    {
        MatchInputController.OnMoveInput -= chara.C_MoveInput;
        MatchInputController.OnCatch -= chara.C_Catch;
        MatchInputController.OnEnemy -= chara.C_Enemy;
        MatchInputController.OnFriendly -= chara.C_Friendly;
        MatchInputController.OnFire -= chara.C_Fire;
        MatchInputController.OnDodge -= chara.C_Dodge;
        MatchInputController.OnFakeFire -= chara.C_FakeFire;
        MatchInputController.OnJump -= chara.C_Jump;
    }
    private void ConnectInput()
    {
        MatchInputController.OnMoveInput += chara.C_MoveInput;
        MatchInputController.OnCatch += chara.C_Catch;
        MatchInputController.OnEnemy += chara.C_Enemy;
        MatchInputController.OnFriendly += chara.C_Friendly;
        MatchInputController.OnFire += chara.C_Fire;
        MatchInputController.OnDodge += chara.C_Dodge;
        MatchInputController.OnFakeFire += chara.C_FakeFire;
        MatchInputController.OnJump += chara.C_Jump;
    }
}