using System.Collections.Generic;
using UnityEngine;

public class DodgeballThrowSetter : MonoBehaviour
{
    public int flippedCatchesCount = 0;
    [SerializeField] List<BallThrowData> ballThrows = new List<BallThrowData>();

    Dodgeball ball = null;
    DodgeballGoTo goTo = null;
    DodgeballGoLaunchTo launchTo = null;

    void Start()
    {
        ball = GetComponent<Dodgeball>();
        goTo = GetComponent<DodgeballGoTo>();
        launchTo = GetComponent<DodgeballGoLaunchTo>();

        goTo.onGoto += OnGoTo;
        launchTo.onLaunchedTo += OnLaunchedTo;
        ball.E_OnStateUpdated += OnStateUpdated;
    }
    void OnDestroy()
    {
        ball.E_OnStateUpdated -= OnStateUpdated;
        goTo.onGoto -= OnGoTo;
    }

    private void OnLaunchedTo()
    {

    }
    private void OnGoTo()
    {
        if (ball.ballState == Dodgeball.BallState.Flying)
        {

        }
    }
    private void OnStateUpdated(Dodgeball.BallState newState)
    {

    }
}