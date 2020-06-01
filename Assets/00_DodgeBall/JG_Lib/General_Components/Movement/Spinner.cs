using System.Collections;
using UnityEngine;

namespace GW_Lib.Utility
{
    public class Spinner : MonoBehaviour
    {
        [SerializeField] bool useLocal = false;
        [SerializeField] MinMaxRange xRotPerMin = new MinMaxRange(0, 30, 1, 1.5f);
        [SerializeField] MinMaxRange yRotPerMin = new MinMaxRange(0, 30, 1, 1.5f);
        [SerializeField] MinMaxRange zRotPerMin = new MinMaxRange(0, 30, 1, 1.5f);

        [Header("Read Only")]
        [SerializeField] float xRot;
        [SerializeField] float yRot, zRot;
        [SerializeField] float xPerFrame, yPerFrame, zPerFrame;
        [SerializeField] float deltaTime;

        IEnumerator Start()
        {
            while (true)
            {
                UpdateData();

                yield return new WaitForSeconds(60);
            }
        }

        private void UpdateData()
        {
            xRot = xRotPerMin.GetValue();
            yRot = yRotPerMin.GetValue();
            zRot = zRotPerMin.GetValue();

            deltaTime = Time.deltaTime;
            xPerFrame = MathExtensions.RotPerMinuteToRotPerFrame(xRot, deltaTime);
            yPerFrame = MathExtensions.RotPerMinuteToRotPerFrame(yRot, deltaTime);
            zPerFrame = MathExtensions.RotPerMinuteToRotPerFrame(zRot, deltaTime);
        }

        void LateUpdate() => Spin();
        

        private void Spin()
        {
            // Vector3 deltaEU = new Vector3(xPerFrame, yPerFrame, zPerFrame);

            // if (useLocal)
            // {
            //     transform.localEulerAngles = transform.localEulerAngles + deltaEU;
            // }
            // else
            // {
            //     transform.eulerAngles = transform.eulerAngles + deltaEU;
            // }

            // transform.RotateAround(transform.position,right, xPerFrame);
            // transform.RotateAround(transform.position,up, yPerFrame);
            // transform.RotateAround(transform.position,fwd, zPerFrame);

            transform.Rotate(right, xPerFrame);
            transform.Rotate(up, yPerFrame);
            transform.Rotate(fwd, zPerFrame);
        }

        Vector3 up
        {
            get
            {
                if (useLocal)
                {
                    return transform.up;
                }
                return Vector3.up;
            }
        }
        Vector3 right
        {
            get
            {
                if (useLocal)
                {
                    return transform.right;
                }
                return Vector3.right;
            }
        }
        Vector3 fwd
        {
            get
            {
                if (useLocal)
                {
                    return transform.forward;
                }
                return Vector3.forward;
            }
        }

    }
}