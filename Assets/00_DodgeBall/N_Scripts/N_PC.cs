using UnityEngine;
using Photon.Pun;
using System.Linq;

public class N_PC : MonoBehaviour,IPunObservable
{
    public int CreatorViewID => creatorViewID;
    public int ActorID => pv.Controller.ActorNumber;
    [SerializeField] int creatorViewID = 0;

    [Header("Move Smoothing Settings")]
    [Tooltip("if this distance between current position and networked position is higher than this, we snap to correct XZ place")]
    [SerializeField] float snapXZDist = 2;

    [Tooltip("Curve that Governs how close we get to posWeigth (try to catch up) to the networked position as we move," +
    	" starting from 0 to snapXZDist\n")]
    [SerializeField] AnimationCurve distToInputCurve = AnimationCurve.Linear(0, 0, 1, 1);
    [SerializeField] float posWeigth = 2;
    [SerializeField] float inputWeigth = 1;
    [Tooltip("Lag Weigth, increases leaning toweards the last taken input direction")]
    [SerializeField] float lagWeigth = 1.5f;
    [Tooltip("Distance (Networked Pos/Current Position) of which above we will keep trying to actively move")]
    [SerializeField] float autoMoveThreshold = 0.3f;
    [SerializeField] float autoMoveSatisfaction = 0.1f;

    protected PC pc = null;
    DodgeballCharacter chara = null;
    Rigidbody rb3d = null;
    PhotonView pv = null;

    [Header("Read Only")]
    [SerializeField] Vector3 networkedInput = new Vector3();
    [SerializeField] Vector3 networkedPos = new Vector3();
    [SerializeField] float lastLag = 0;

    protected virtual void Start()
    {
        pv = GetComponent<PhotonView>();
        pc = GetComponent<PC>();
        if (!pv.IsMine)
        {
            pc.enabled = false;
        }
    }
    void OnEnable()
    {
        pv = GetComponent<PhotonView>();
        rb3d = GetComponent<Rigidbody>();
        chara = GetComponent<DodgeballCharacter>();
        if(GetComponent<PhotonView>().IsMine)
            chara.OnCommandActivated += SendCommand;
    }
    void OnDisable()
    {
        if (pv.IsMine)
            chara.OnCommandActivated -= SendCommand;
    }

    [PunRPC]//Called In N_PlayerManager
    private void OnCreated(int creatorViewID)
    {
        this.creatorViewID = creatorViewID;
        TeamsManager.AddCharacter(GetComponent<DodgeballCharacter>());
        gameObject.SetActive(false);
        name = pv.Controller.NickName;
    }
    [PunRPC]//Called In N_GameManager
    private void PrepareForGame()
    {
        SpawnPoint s = FindObjectsOfType<SpawnPoint>().ToList().Find(p => p.CheckPlayer(pv.Controller.ActorNumber));
        transform.position = s.position;
        transform.rotation = s.rotation;
        gameObject.SetActive(true);

        GetComponent<DodgeballCharacter>().SetTeam(N_TeamsManager.GetTeam(ActorID));
    }

    public virtual void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(chara.syncedInput.x);
            stream.SendNext(chara.syncedInput.z);

            stream.SendNext(rb3d.position.x);
            stream.SendNext(rb3d.position.z);

            //stream.SendNext(chara.syncedYAngle);
        }
        else if (stream.IsReading)
        {
            networkedInput.x = (float)stream.ReceiveNext();
            networkedInput.z = (float)stream.ReceiveNext();

            networkedPos.x = (float)stream.ReceiveNext();
            networkedPos.z = (float)stream.ReceiveNext();

            TrySnapToNetPos();
            UpdateSyncedInput();

            //chara.syncedYAngle = (float)stream.ReceiveNext();

            chara.C_MoveInput();
        }
        lastLag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
    }
    void FixedUpdate()
    {
        UpdateSyncedInput();
    }

    private void SendCommand(DodgeballCharaCommand command)
    {
        pv.RPC("RecieveCommand", RpcTarget.Others, (int)command);
    }

    [PunRPC]
    private void RecieveCommand(int c)
    {
        DodgeballCharaCommand command = (DodgeballCharaCommand)c;
        switch (command)
        {
            case DodgeballCharaCommand.Catch:
                Debug.Log("What Happened?");
                chara.C_Catch();
                break;
            case DodgeballCharaCommand.Dodge:
                chara.C_Dodge();
                break;
            case DodgeballCharaCommand.Enemy:
                chara.C_Enemy();
                break;
            case DodgeballCharaCommand.FakeFire:
                chara.C_FakeFire();
                break;
            case DodgeballCharaCommand.Fire:
                chara.C_Fire();
                break;
            case DodgeballCharaCommand.Friendly:
                chara.C_Friendly();
                break;
            case DodgeballCharaCommand.Jump:
                chara.C_Jump();
                break;
            case DodgeballCharaCommand.MoveInput:
                chara.C_MoveInput();
                break;
        }
    }   

    //Helper Functions
    private void TrySnapToNetPos()
    {
        float dist = Vector3.Distance(rb3d.position, networkedPos);

        if (dist > snapXZDist)
        {
            rb3d.MovePosition(networkedPos);
        }
    }
    private void UpdateSyncedInput()
    {
        if (pv.IsMine)
            return;

        Vector3 currPos = rb3d.position;
        currPos.y = 0;
        float dist = Vector3.Distance(currPos, networkedPos);
        float normDist = dist / snapXZDist;
        float catchUpVal = distToInputCurve.Evaluate(normDist) * posWeigth;
        Vector3 dirToNetPos = -(networkedPos - currPos).normalized;

        Vector3 dirElement = dirToNetPos * catchUpVal;
        Vector3 lagPart = networkedInput * lastLag * lagWeigth;
        Vector3 inputElement = networkedInput * inputWeigth;

        Vector3 weithedInput = dirElement + inputElement + lagPart;
        //Vector3 weithedInput = Vector3.Lerp(inputElement, dirElement, Time.fixedDeltaTime);
        weithedInput.y = 0;
        weithedInput.Normalize();
        chara.syncedInput = weithedInput;

        //Debug.Log(dist);
        //if(dist<autoMoveSatisfaction)
        //{
        //    chara.syncedInput = Vector3.zero;
        //    chara.C_MoveInput();
        //    Debug.LogWarning("Should have stopped ?!");
        //    return;
        //}
        //if (dist>=autoMoveThreshold)
        //{
        //    chara.C_MoveInput();
        //}
    }
}