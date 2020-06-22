using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(fileName = "BallThrow", menuName = "Dodgeball/BallThrow")]
public class BallThrowData : ScriptableObject
{
    public byte id = 0;
    [Tooltip("Usable in GetSpeedOfDist")]
    [SerializeField] float speed = 10;
    public Ease ease = Ease.InOutSine;
    public float ofShootDist = 0.2f;

    public float GetTimeOfDist(float dist)
    {
        //t = x/v
        float time = 0;
        time = dist / speed;
        return time;
    }
}