using UnityEngine;

public class DodgeballTrail : MonoBehaviour
{
    [SerializeField] Dodgeball ball = null;
    TrailRenderer tr = null;

    void Start()
    {
        tr = GetComponent<TrailRenderer>();
        ball.E_OnStateUpdated += OnStateUpdated;
    }
    private void OnStateUpdated(Dodgeball.BallState newState)
    {
        switch (newState)
        {
            case Dodgeball.BallState.Flying:
                tr.enabled = true;
                break;
            case Dodgeball.BallState.Held:
                tr.enabled = false;
                break;
            case Dodgeball.BallState.OnGround:
                tr.enabled = false;
                break;
        }   
    }
}