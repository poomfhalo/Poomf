using UnityEngine;
using Photon.Pun;
using GW_Lib;

[RequireComponent(typeof(Dodgeball))]
public class N_Dodgeball : N_Singleton<N_Dodgeball>, IPunObservable
{
    [SerializeField] bool disableOnStart = true;
    [SerializeField] float timeBeforeBallLaunch = 1f;
    [Header("Read Only")]
    [SerializeField] Vector3 netPos = new Vector3();

    Rigidbody rb3d = null;
    PhotonView pv = null;
    Dodgeball ball = null;

    public override void OnEnable()
    {
        base.OnEnable();
        pv = GetComponent<PhotonView>();
        ball = GetComponent<Dodgeball>();
        rb3d = GetComponent<Rigidbody>();

        ball.OnCommandActivated += SendCommand;
    }
    public override void OnDisable()
    {
        base.OnDisable();
        ball.OnCommandActivated -= SendCommand;
    }
    void Start()
    {
        if (disableOnStart)
            gameObject.SetActive(false);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(rb3d.position.x);
            stream.SendNext(rb3d.position.y);
            stream.SendNext(rb3d.position.z);
            stream.SendNext((int)ball.ballState);
        }
        else if(stream.IsReading)
        {
            netPos.x = (float)stream.ReceiveNext();
            netPos.y = (float)stream.ReceiveNext();
            netPos.z = (float)stream.ReceiveNext();
            ball.ballState = (Dodgeball.BallState)(int)stream.ReceiveNext();
        }
    }
    private void SendCommand(DodgeballCommand command)
    {
        int holder = ball.GetHolder().GetComponent<N_PC>().ActorID;
        pv.RPC("RecieveCommand", RpcTarget.Others, (int)command,holder);
    }

    [PunRPC]
    private void RecieveCommand(int c,int holder)
    {
        DodgeballCommand command = (DodgeballCommand)c;
        DodgeballCharacter n_holder = N_TeamsManager.GetPlayer(holder).GetComponent<DodgeballCharacter>();
        switch (command)
        {
            case DodgeballCommand.GoToChara:
                Debug.Log("Ball().GoingToChara Command RPC");
                //Dodgeball.GoTo(n_holder,);
                break;
        }
    }
    [PunRPC]
    private void PrepareForGame()
    {
        gameObject.SetActive(true);
        this.InvokeDelayed(timeBeforeBallLaunch, () => ball.LaunchUp(-1));
    }
}