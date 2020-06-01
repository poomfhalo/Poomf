using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace GW_Lib.Utility
{
    public class SortFilesByIntThenAlpha : IComparer<FileInfo>
    {
        public int Compare(FileInfo x, FileInfo y)
        {
            char start = x.Name[0];
            char end = y.Name[0];
            int startInt, endInt;
            bool startIsNum = int.TryParse(start.ToString(), out startInt);
            bool endIsNum = int.TryParse(end.ToString(), out endInt);
            if (startIsNum && endIsNum)
            {
                return startInt.CompareTo(endInt);
            }
            return x.Name.CompareTo(y.Name);
        }
    }
    public class SortRayCastHitsByDist : IComparer<RaycastHit>
    {
        public int Compare(RaycastHit x, RaycastHit y)
        {
            return x.distance.CompareTo(y.distance);
        }
    }
}