using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MatchType { Practice, OneVsOne, TwoVsTwo, ThreeVsThree, FourVsFour }
public class MatchTypeButton : MonoBehaviour
{
    public MatchType matchType = MatchType.Practice;
}