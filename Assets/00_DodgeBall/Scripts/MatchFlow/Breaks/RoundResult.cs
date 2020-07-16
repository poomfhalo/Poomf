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
    [SerializeField] TextMeshProUGUI winnerText = null;

    void Start()
    {
        winnerText.text = "";
        for (int i = 1;i<=MatchState.Instance.PreviousRoundNum;i++)
        {
            winnerText.text += "Round " + i + " Was Won By " + MatchState.Instance.GetRoundWinner(MatchState.Instance.PreviousRoundNum).ToString() + "\n";
        }
        this.InvokeDelayed(4, () =>{
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