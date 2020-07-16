using System.Collections.Generic;
using System.Linq;
using GW_Lib;
using UnityEngine;
using UnityEngine.UI;

public class RoundsGroupUI : MonoBehaviour
{
    [SerializeField] Transform roundsHead = null;
    [SerializeField] RoundUI roundPrefab = null;

    public void Initialize(int totalRoundsCount, List<int> score)
    {
        Extentions.CleanChildren(roundsHead);
        for (int i = 0; i < totalRoundsCount; i++)
        {
            RoundUI clone = Instantiate(roundPrefab, roundsHead);
            if(i>=score.Count)
            {
                clone.WasNotPlayed();
            }
            else if(score[i] == 0)
            {
                clone.WasLost();
            }
            else if(score[i] == 1)
            {
                clone.WasWon();
            }
        }

        GetComponentsInChildren<RectTransform>().ToList().ForEach(LayoutRebuilder.MarkLayoutForRebuild);
    }
}