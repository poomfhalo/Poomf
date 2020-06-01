using System.Collections.Generic;
using UnityEngine;

namespace GW_Lib.Utility
{
    [System.Serializable]
    public class MinMaxRange
    {
        public static MinMaxRange SmallDefaultPositive { get { return new MinMaxRange(0, 1, 0.1f, 0.2f); } }
        public static MinMaxRange DefaultPositive { get { return new MinMaxRange(0, 1, 0.25f, 0.75f); } }
        public static MinMaxRange BigDefaultPositive { get { return new MinMaxRange(0, 2, 0.5f, 1.5f); } }
        public static MinMaxRange FarBigDefaultPositive { get { return new MinMaxRange(0, 2, 1.0f, 2); } }

        public float absMin=-1, absMax=1;
        public float rMin = -0.5f, rMax = 0.5f;

        public float GetValue()
        {
            return Random.Range(rMin, rMax);
        }
        public MinMaxRange(float absMin, float absMax, float rMin=-1, float rMax=-1):this()
        {
            this.absMax = absMax;
            this.absMin = absMin;
            if (rMin==-1)
            {
                this.rMin = absMin / 2.0f;
            }
            else
            {
                this.rMin = rMin;
            }
            if (rMax==-1)
            {
                this.rMax = absMax / 2.0f;
            }
            else
            {
                this.rMax = rMax;
            }
        }
        public MinMaxRange(){ }

        public MinMaxRange(MinMaxRange range)
        {
            absMin = range.absMin;
            absMax = range.absMax;
            rMin = range.rMin;
            rMax = range.rMax;
        }

        public void SetAbs(float absMin, float absMax, bool reSetMid=true)
        {
            this.absMin = absMin;
            this.absMax = absMax;
            if (reSetMid)
            {
                rMin = absMin / 2.0f;
                rMax = absMax / 2.0f;
            }
        }
        public bool IsWithinMidPoints(float value)
        {
            return value <= rMax && value >= rMin;
        }
        public bool IsWithinMaxPoints(float value)
        {
            return value <= absMax && value >= absMin;
        }
        /// <summary>
        /// returns 1 if to the right, returns -1 if to the left, returns zero if within
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int DirectionToRPoints(float value)
        {
            if (value>=rMax)
            {
                return 1;
            }
            else if (value<=rMin)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }
        /// <summary>
        /// returns 1 if to the right, returns -1 if to the left, returns zero if within
        /// (on maximum and minimum values of the range)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int DirectionToAbsPoints(float value)
        {
            if (value >= absMax)
            {
                return 1;
            }
            else if (value <= absMin)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }

        public bool Equals(MinMaxRange r)
        {
            return AreEqual(r.absMax, absMax) && AreEqual(r.absMin, absMin) && AreEqual(r.rMin, rMin) && AreEqual(r.rMax, rMax);
        }
        public bool AreEqual(float f1,float f2)
        {
            return Mathf.Abs(f1 - f2) < Mathf.Epsilon;
        }
    }
}