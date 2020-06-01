using UnityEngine;

namespace GW_Lib.Utility
{
    [System.Serializable]
    public struct SqrFloat
    {
        public static SqrFloat One { get { return new SqrFloat(1); } }
        public static SqrFloat Zero { get { return new SqrFloat(0); } }
        public float SqrF => sqrF;
        public float F => f;

        [SerializeField] float f;
        [SerializeField] float sqrF;

        public SqrFloat(float f) : this()
        {
            SetValue(f);
        }

        public void SetValue(float f)
        {
            this.f = f;
            sqrF = f * f;
        }

        public static SqrFloat operator +(SqrFloat a, SqrFloat b)
        {
            return new SqrFloat(a.f + b.f);
        }
        public static SqrFloat operator -(SqrFloat a, SqrFloat b)
        {
            return new SqrFloat(a.f - b.f);
        }
        public static SqrFloat operator /(SqrFloat a, float b)
        {
            return new SqrFloat(a.f / b);
        }
        public static SqrFloat operator *(SqrFloat a, float b)
        {
            return new SqrFloat(a.f * b);
        }
        public static SqrFloat operator +(SqrFloat a, float b)
        {
            return new SqrFloat(a.f + b);
        }
        public static SqrFloat operator -(SqrFloat a, float b)
        {
            return new SqrFloat(a.f - b);
        }

        public static implicit operator float(SqrFloat a)
        {
            return a.f;
        }
        public static implicit operator SqrFloat(float a)
        {
            return new SqrFloat(a);
        }
    }
}
