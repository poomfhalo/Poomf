using UnityEngine;
/// <summary>
/// Class, created to hold deprecated movement smoothing systems, as reference
/// </summary>
public class N_PC_D : MonoBehaviour
{
    //[SerializeField] float inputWeigth = 1;
    //[Tooltip("Lag Weigth, increases leaning toweards the last taken input direction")]
    //[SerializeField] float lagWeigth = 1.5f;
    //[Tooltip("Curve that Governs how close we get to posWeigth (try to catch up) to the networked position as we move," +
    //    " starting from 0 to snapXZDist\n")]
    //[SerializeField] AnimationCurve distToInputCurve = AnimationCurve.Linear(0, 0, 1, 1);
    //[SerializeField] float posWeigth = 2;

    //private void DeprecatedInputSync()
    //{
    //    Vector3 currPos = rb3d.position;
    //    currPos.y = 0;
    //    dist = Vector3.Distance(currPos, networkedPos);

    //    Vector3 lagPart = networkedInput * lastLag * lagWeigth;
    //    Vector3 inputElement = networkedInput * inputWeigth;
    //    Vector3 weigthedInput = inputElement;

    //    weigthedInput.y = 0;
    //    weigthedInput.Normalize();
    //    chara.syncedInput = weigthedInput;

    //    if (dist >= autoMoveThreshold)
    //    {
    //        float normDist = dist / snapXZDist;
    //        float catchUpVal = distToInputCurve.Evaluate(normDist) * posWeigth;
    //        if (networkedInput == Vector3.zero)
    //        {

    //            Vector3 lerpedNetPos = Vector3.Lerp(rb3d.position, networkedPos, catchUpVal * Time.fixedDeltaTime);
    //            rb3d.MovePosition(lerpedNetPos);
    //        }
    //        else
    //        {

    //            Vector3 dirToPos = (rb3d.position - networkedPos).normalized;
    //            dirToPos.y = 0;
    //            dirToPos.Normalize();
    //            float amountInSameDir = Vector3.Dot(transform.forward, dirToPos.normalized);
    //            if (amountInSameDir < 0)
    //            {
    //                amountInSameDir = 0;
    //            }

    //            Vector3 p1 = transform.forward * lastLag * chara.GetComponent<Mover>().maxSpeed;
    //            Vector3 p2 = dirToPos * amountInSameDir * catchUpVal * lastLag * lagWeigth;
    //            Vector3 p = Time.fixedDeltaTime * (p1 + p2) / 2.0f;

    //            rb3d.MovePosition(rb3d.position + p);
    //        }
    //    }
    //}
}