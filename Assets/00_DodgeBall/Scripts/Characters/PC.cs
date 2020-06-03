using System;
using GW_Lib.Utility;
using UnityEngine;

public class PC : DodgeballCharacter
{
    void OnEnable()
    {
        MatchInputController.OnMoveInput += C_MoveInput;
        MatchInputController.OnCatch += C_Catch;
        MatchInputController.OnEnemy += C_Enemy;
        MatchInputController.OnFriendly += C_Friendly;
        MatchInputController.OnFire += C_Fire;
        MatchInputController.OnDodge+= C_Dodge;
        MatchInputController.OnFakeFire += C_FakeFire;
        MatchInputController.OnJump += C_Jump;
    }
    void OnDisable()
    {
        MatchInputController.OnMoveInput -= C_MoveInput;
        MatchInputController.OnCatch -= C_Catch;
        MatchInputController.OnEnemy -= C_Enemy;
        MatchInputController.OnFriendly -= C_Friendly;
        MatchInputController.OnFire -= C_Fire;
        MatchInputController.OnDodge -= C_Dodge;
        MatchInputController.OnFakeFire -= C_FakeFire;
        MatchInputController.OnJump -= C_Jump;
    }
}