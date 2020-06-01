using System;
using System.Linq;

namespace GW_Lib.Utility
{
    [Serializable]
    public class UnitType : MultiSelectType
    {
        public enum UnitsTypes {Infantry,Tanks,Air,Naval}

        public override string[] possiblities
        {
            get
            {
                int max = Enum.GetValues(typeof(UnitsTypes)).Cast<int>().Max() + 1;
                string[] names = new string[max];
                for (int i = 0; i < max; i++)
                {
                    names[i] = ((UnitsTypes)i).ToString();
                }
                return names;
            }
        }

        public bool DoesOverlap(UnitsTypes type)
        {
            return DoesOverlap((int)type);
        }
    }
}