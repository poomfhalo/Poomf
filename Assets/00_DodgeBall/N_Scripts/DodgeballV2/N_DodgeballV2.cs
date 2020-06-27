using UnityEngine;
using Photon.Pun;

//We Will Testout 2 Methods:
//1.
//Keep the Syncer, run appropriately, at the correct times, be disabled/enabled.
//Have The Authority, being Transfered, to the correct character.
//Actions only apply on the Appropriate "Owner"
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

    void Start()
    {
        ball = GetComponent<Dodgeball>();
        pv = GetComponent<PhotonView>();

        ball.E_OnCommandActivated += OnCommandActivated;
        ball.E_OnStateUpdated += OnStateUpdated;
        ball.reflection.extReflectionTest = pv.IsMine;
    }
    void Update()
    {
        if (ball.reflection.IsRunning)
        {
            Debug.LogWarning("Reflection Is running and I Am The Master :: " + pv.IsMine);
        }
    }
    private void OnCommandActivated(DodgeballCommand cmd)
    {

    }
    private void OnStateUpdated(Dodgeball.BallState newState)
    {

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