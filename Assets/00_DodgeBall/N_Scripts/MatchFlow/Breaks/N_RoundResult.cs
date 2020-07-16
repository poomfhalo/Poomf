using System.Collections;
using UnityEngine;
using Photon.Pun;

public class N_RoundResult : MonoBehaviour
{
    RoundResult roundResult = null;
    void Start()
    {
        roundResult = GetComponent<RoundResult>();
        roundResult.LoadStageFunc = LoadStage;
    }

    private IEnumerator LoadStage()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            SceneFader.instance.FadeIn(1.1f, null);
            yield return new WaitForSeconds(1.1f);
            PhotonNetwork.LoadLevel(MatchState.Instance.GetMatchSceneName);
        }
        else
        {
            SceneFader.instance.FadeIn(1, null);
            yield return new WaitForSeconds(1);
        }
    }
}