using System;
using UnityEngine;

namespace GW_Lib.Utility
{
    [Serializable]
    public abstract class MultiSelectType
    {
        public abstract string[] possiblities { get; }
        public int Selections { get { return selections; } }
        [SerializeField] protected int selections;

        public bool DoesOverlap(MultiSelectType testType)
        {
            int targetMask = testType.selections;
            int contactMask = selections & targetMask;
            #region DebugHelper
            //Debug.Log("Current Mask__" + ToBits(selections) + "\n" +
            //              "Target Mask__" + ToBits(targetMask) + "\n" +
            //              "Contact Result__" + ToBits(contactMask));
            #endregion
            return contactMask != 0;
        }
        public bool DoesOverlap(int type)
        {
            int typeMask = (1 << type);
            int contactMask = selections & typeMask;
            return contactMask != 0;
        }
        private static string ToBits(int contact)
        {
            return Convert.ToString(contact, 2);
        }
        public override string ToString()
        {
            return ToBits(selections) + "__" + selections;
        }
        public MultiSelectType()
        {
            selections = 0;
        }
    }
}