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

    [SerializeField] float inputWeigth = 1;
    [Tooltip("Lag Weigth, increases leaning toweards the last taken input direction")]
    [SerializeField] float lagWeigth = 1.5f;
    [Tooltip("Curve that Governs how close we get to posWeigth (try to catch up) to the networked position as we move," +
    	" starting from 0 to snapXZDist\n")]
    [SerializeField] AnimationCurve distToInputCurve = AnimationCurve.Linear(0, 0, 1, 1);
    [SerializeField] float posWeigth = 2;
    [Tooltip("Distance (Networked Pos/Current Position) of which above we will keep trying to actively move")]
    [SerializeField] float autoMoveThreshold = 0.3f;

    protected PC pc = null;
    DodgeballCharacter chara = null;
    Rigidbody rb3d = null;
    PhotonView pv = null;

    [Header("Read Only")]
    [SerializeField] float lastLag = 0;
    [SerializeField] float netDist = 0;
    [SerializeField] Vector3 netPos = new Vector3();
    [SerializeField] Vector3 netDisp = new Vector3();
    [SerializeField] Vector3 netDir = new Vector3();

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
        //netPos = s.position;

        gameObject.SetActive(true);

        GetComponent<DodgeballCharacter>().SetTeam(N_TeamsManager.GetTeam(ActorID));
    }

    public virtual void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(rb3d.position.x);
            stream.SendNext(rb3d.position.z);
        }
        else if (stream.IsReading)
        {
            netPos.x = (float)stream.ReceiveNext();
            netPos.z = (float)stream.ReceiveNext();
            UpdateNetData();

            TrySnapToNetPos();
            UpdateSyncedInput();
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
        float currX = rb3d.position.x;
        float currZ = rb3d.position.z;
        pv.RPC("RecieveCommand", RpcTarget.Others, (int)command,currX,currZ);
    }

    [PunRPC]
    private void RecieveCommand(int c,float currX,float currZ)
    {
        netPos.x = currX;
        netPos.z = currZ;
        UpdateNetData();

        DodgeballCharaCommand command = (DodgeballCharaCommand)c;
        switch (command)
        {
            case DodgeballCharaCommand.Catch:
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
                UpdateSyncedInput();
                chara.C_MoveInput();
                break;
        }
    }   

    //Helper Functions
    private void UpdateNetData()
    {
        netDisp = (rb3d.position - netPos);
        netDisp.y = 0;
        netDist = netDisp.magnitude;
        netDir = netDisp.normalized;
    }
    private void TrySnapToNetPos()
    {
        Vector3 currPos = rb3d.position;
        currPos.y = 0;
        netDist = Vector3.Distance(rb3d.position, netPos);

        if (netDist > snapXZDist)
        {
            rb3d.MovePosition(netPos);
        }
    }
    private void UpdateSyncedInput()
    {
        if (pv.IsMine)
            return;

        if (netDist < autoMoveThreshold)
        {
            chara.syncedInput = Vector3.zero;
            chara.C_MoveInput(chara.syncedInput);
            return;
        }

        chara.syncedInput = netDir;
    }

    private void DeprecatedInputSync()
    {
        //Vector3 currPos = rb3d.position;
        //currPos.y = 0;
        //dist = Vector3.Distance(currPos, networkedPos);

        //Vector3 lagPart = networkedInput * lastLag * lagWeigth;
        //Vector3 inputElement = networkedInput * inputWeigth;
        //Vector3 weigthedInput = inputElement;

        //weigthedInput.y = 0;
        //weigthedInput.Normalize();
        //chara.syncedInput = weigthedInput;

        //if (dist >= autoMoveThreshold)
        //{
        //    float normDist = dist / snapXZDist;
        //    float catchUpVal = distToInputCurve.Evaluate(normDist) * posWeigth;
        //    if (networkedInput == Vector3.zero)
        //    {

        //        Vector3 lerpedNetPos = Vector3.Lerp(rb3d.position, networkedPos, catchUpVal * Time.fixedDeltaTime);
        //        rb3d.MovePosition(lerpedNetPos);
        //    }
        //    else
        //    {

        //        Vector3 dirToPos = (rb3d.position - networkedPos).normalized;
        //        dirToPos.y = 0;
        //        dirToPos.Normalize();
        //        float amountInSameDir = Vector3.Dot(transform.forward, dirToPos.normalized);
        //        if (amountInSameDir < 0)
        //        {
        //            amountInSameDir = 0;
        //        }

        //        Vector3 p1 = transform.forward * lastLag * chara.GetComponent<Mover>().maxSpeed;
        //        Vector3 p2 = dirToPos * amountInSameDir * catchUpVal * lastLag * lagWeigth;
        //        Vector3 p = Time.fixedDeltaTime * (p1 + p2) / 2.0f;

        //        rb3d.MovePosition(rb3d.position + p);
        //    }
        //}
    }
}