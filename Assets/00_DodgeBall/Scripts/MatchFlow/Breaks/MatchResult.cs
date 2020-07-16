using System;
using GW_Lib;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MatchResult : MonoBehaviour
{
    public event Action beforeChangingScene = null;
    [SerializeField] TextMeshProUGUI winnerText = null;

    void Start()
    {
        winnerText.text = MatchState.Instance.GetMatchWinner().ToString();
        this.InvokeDelayed(3, () => {
            beforeChangingScene?.Invoke();
            SceneFader.instance.FadeIn(1, () => {
                SceneManager.LoadScene("Menu");
            });
        });
    }

}