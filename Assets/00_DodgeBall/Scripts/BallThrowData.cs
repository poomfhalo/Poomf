using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(fileName = "BallThrow", menuName = "Dodgeball/BallThrow")]
public class BallThrowData : ScriptableObject
{
    public byte id = 0;
    [Tooltip("Usable in GetSpeedOfDist")]
    [SerializeField] float speed = 10;
    public Ease ease = Ease.InOutSine;
    [Tooltip("Extra Distance, the ball will travel, after reaching the point")]
    public float ofShootDist = 0.2f;
    [Header("Visual Changes")]
    public ParticleSystem throwEffect = null;
    public bool canBeCaught = true;

    public float GetTimeOfDist(float dist)
    {
        //t = x/v
        float time = 0;
        time = dist / speed;
        return time;
    }
}