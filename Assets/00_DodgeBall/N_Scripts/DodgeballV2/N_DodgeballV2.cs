using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class N_DodgeballV2 : MonoBehaviour, IPunObservable
{
    [SerializeField] int commandsSnapCount = 3;
    [SerializeField] List<DodgeballCommand> sentCommands = new List<DodgeballCommand>();

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