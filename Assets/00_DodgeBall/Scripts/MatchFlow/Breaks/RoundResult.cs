using System;
using System.Collections;
using GW_Lib;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoundResult : MonoBehaviour
{
    public bool allowLoading = false;
    public Func<IEnumerator> LoadStageFunc
    {
        get
        {
            if (loadStagfunc == null)
                loadStagfunc = LoadStage;
            return loadStagfunc;
        }
        set
        {
            loadStagfunc = value;
        }
    }
    Func<IEnumerator> loadStagfunc = null;

    [SerializeField] float loadWaitTime = 10;
    [SerializeField] TextMeshProUGUI winnerText = null;

    void Start()
    {
        winnerText.text = "";
        for (int i = 1;i<=MatchState.Instance.PreviousRoundNum;i++)
        {
            TeamTag t = MatchState.Instance.GetRoundWinner(i);
            if (t ==  TeamTag.None)
                winnerText.text = "Round " + i + " Was A Tie\n";
            else
                winnerText.text += "Round " + i + " Was Won By " + t.ToString() + "\n";
        }
        this.InvokeDelayed(loadWaitTime, () =>{
            StartCoroutine(LoadStageFunc());
        });
    }
    private IEnumerator LoadStage()
    {
        SceneFader.instance.FadeIn(1, null);
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(MatchState.Instance.GetMatchSceneName);
    }
}