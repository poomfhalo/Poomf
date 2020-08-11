using System;
using GW_Lib;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MatchResult : MonoBehaviour
{
    public bool allowRematch = false;
    public bool allowScoreBoard = false;

    [SerializeField] Button home = null, rematch = null, scoreBoard = null;
    [SerializeField] GameObject voteTexts = null; 

    public event Action beforeChangingScene = null;
    [SerializeField] TextMeshProUGUI winnerText = null;
    [SerializeField] bool autoMove = false;

    void Start()
    {
        if (!allowRematch)
        {
            rematch.gameObject.SetActive(false);
            voteTexts.SetActive(false);
        }
        if (!allowScoreBoard)
            scoreBoard.gameObject.SetActive(false);

        scoreBoard.onClick.AddListener(OnScoreboardClicked);
        rematch.onClick.AddListener(OnRematchClicked);
        home.onClick.AddListener(OnHomeClicked);
        TeamTag winner = MatchState.Instance.GetMatchWinner();
        if (winner == TeamTag.None)
            winnerText.text = "A Tie";
        else
            winnerText.text = winner.ToString() + " Wins";

        if (!autoMove)
            return;

        home.onClick.Invoke();
    }

    private void OnHomeClicked()
    {
        home.interactable = false;
        this.InvokeDelayed(1, () => {
            beforeChangingScene?.Invoke();
            SceneFader.instance.FadeIn(1, () => {
                SceneManager.LoadScene("Menu");
            });
        });
    }
    private void OnRematchClicked()
    {

    }
    private void OnScoreboardClicked()
    {

    }
}