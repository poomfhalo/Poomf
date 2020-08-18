using GW_Lib.Utility;
using UnityEngine;

public class DodgeballReflectionSurface : MonoBehaviour
{
    [Tooltip("-1 means the ball must be exactly in opposite direction of hit character\n" +
        "0 means perpindicular to hit character is the maximum direction of reflection\n" +
        "1 means the ball have a chance to be reflected at all 360 angles\n" +
        "everything else, is in between, depending on value")]
    [Range(-1, 1)]
    [SerializeField] float collisionDirThreshold = -0.75f;
    [Tooltip("How far the reflection point is")]
    [SerializeField] MinMaxRange reflectionDist = new MinMaxRange(0.5f, 6, 2, 3.5f);
    [Tooltip("the number that the speed of the ball gets divided by")]
    [SerializeField] MinMaxRange reflectionSpeedDivider = new MinMaxRange(1.1f, 6, 4.5f, 5.5f);

    public float GetCollisionDirThreshold() => collisionDirThreshold;
    public float GetReflectionDist() => reflectionDist.GetValue();
    public float GetReflectionSpeedDivider() => reflectionSpeedDivider.GetValue();
}
