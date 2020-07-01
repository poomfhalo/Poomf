using UnityEngine;
using Photon.Pun;
using Smooth;

//We Will Testout 2 Methods:
//1.
//Keep the Syncer, run appropriately, at the correct times, be disabled/enabled.
//Have The Authority, being Transfered, to the correct character.
//Actions only apply on the Appropriate "Owner"

//Note: if Client, Calls Throw, we keep waiting, on the client machine, 
//untill the same player, but on the master, reaches the throw point, then we allow them to throw
//at the same time
//If Master, Throws, we Do not, allow throw locally, but we send the throw, across the network
//at the same time for everyone.

//2.
//Do Commands Syncing, ball is local unless on ground, and does snap.
//Only core Commands get called
//we have a list of commands, that are synced, whatever gets called on local machine, gets removed from synced list.
//every period of time, we deal with the ball appropriately, depending on the remaining, highest order most recent command

public class N_DodgeballV2 : MonoBehaviour, IPunObservable
{
    //[SerializeField] int commandsSnapCount = 3;
    //[SerializeField] List<DodgeballCommand> sentCommands = new List<DodgeballCommand>();

    PhotonView pv = null;
    Dodgeball ball = null;
    SmoothSyncPUN2 syncer = null;

    void Start()
    {
        ball = GetComponent<Dodgeball>();
        pv = GetComponent<PhotonView>();
        syncer = GetComponent<SmoothSyncPUN2>();

        ball.E_OnCommandActivated += OnCommandActivated;
        ball.E_OnStateUpdated += OnStateUpdated;
        //ball.reflection.extReflectionTest = pv.IsMine;
    }
    void Update()
    {
        if (ball.reflection.IsRunning && ball.reflection.extReflectionTest)
        {
            //Debug.LogWarning("Reflection Is running and I Am The Master :: " + pv.IsMine);
        }
    }

    private void OnCommandActivated(DodgeballCommand cmd)
    {
        if(pv.IsMine)
        {
            return;
        }

        switch (cmd)
        {
            case DodgeballCommand.GoToChara:
                syncer.enabled = false;
                break;
            case DodgeballCommand.LaunchTo:
                syncer.enabled = true;
                break;
            case DodgeballCommand.LaunchUp:
                syncer.enabled = false;
                break;
        }
    }
    private void OnStateUpdated(Dodgeball.BallState newState)
    {
        if (pv.IsMine)
        {
            return;
        }

        switch (newState)
        {
            case Dodgeball.BallState.OnGround:
                syncer.enabled = true;
                break;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {

        }
        else if (stream.IsReading)
        {

        }
    }
}