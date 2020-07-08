using UnityEngine;

[RequireComponent(typeof(DodgeballCharacter))]
public class PC : CharaController
{
    public bool connectToInputOnStart = true;
    [SerializeField] MatchInputController inputPrefab = null;
    MatchInputController input = null;
    [Header("Read Only")]
    [SerializeField] bool isLocked = false;
    public override bool IsLocked { get => isLocked; protected set => isLocked = value; }

    void OnEnable()
    {
        Unlock();
    }
    void OnDisable()
    {
        Lock();
    }
    void Start()
    {
        if (connectToInputOnStart)
        {
            ConnectToInput();
            Unlock();
        }
    }
    public override void Lock()
    {
        IsLocked = true;
        GetComponent<Mover>().movementType = Mover.MovementType.ToPoint;
        if (!input)
            return;

        input.E_OnMoveInput -= chara.C_MoveInput;
        input.E_OnEnemy -= chara.C_Enemy;
        input.E_OnFriendly -= chara.C_Friendly;
        input.E_OnBallAction -= chara.C_OnBallAction;
        input.E_OnDodge -= chara.C_Dodge;
        input.E_OnFakeFire -= chara.C_FakeFire;
        input.E_OnJump -= chara.C_Jump;
    }
    public override void Unlock()
    {
        IsLocked = false;
        GetComponent<Mover>().movementType = Mover.MovementType.ByInput;
        if (!input)
            return;

        input.E_OnMoveInput += chara.C_MoveInput;
        input.E_OnEnemy += chara.C_Enemy;
        input.E_OnFriendly += chara.C_Friendly;
        input.E_OnBallAction += chara.C_OnBallAction;
        input.E_OnDodge += chara.C_Dodge;
        input.E_OnFakeFire += chara.C_FakeFire;
        input.E_OnJump += chara.C_Jump;
    }

    public void ConnectToInput()
    {
        input = Instantiate(inputPrefab, transform);
        input.transform.localPosition = Vector3.zero;
        input.transform.localRotation = Quaternion.identity;
    }
}