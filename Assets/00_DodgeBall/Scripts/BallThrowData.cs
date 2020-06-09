using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(fileName = "BallThrow", menuName = "Dodgeball/BallThrow")]
public class BallThrowData : ScriptableObject
{
    public byte id = 0;
    [Tooltip("Usable in GetTimeOfDist")]
    [SerializeField] float duration = 0.3f;
    [Tooltip("Usable in GetSpeedOfDist")]
    [SerializeField] float speed = 10;
    public Ease ease = Ease.InOutSine;

    public float GetTimeOfDist(float dist)
    {
        //t = x/v
        float time = 0;
        time = dist / speed;
        return time;
    }
    public float GetSpeedOfDist(float dist)
    {
        //x/t = v
        float s = 0;
        s = dist / duration;
        return s;
    }
}