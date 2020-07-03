using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class N_Room : MonoBehaviour
{
    [Header("Constants")]
    [SerializeField] Image levelBGImage = null;
    [SerializeField] TextMeshProUGUI timerText = null;
    [Header("Variables")]
    [Tooltip("In Seconds")]
    [SerializeField] float levelChangeTimer = 30;

    VoteSlot[] delegators = null;
    float levelChangeCounter = 0;
    bool isTimerRunning = false;
    void Awake()
    {
        delegators = GetComponentsInChildren<VoteSlot>();
        foreach (var d in delegators)
        {
            d.E_OnPointerEntered += OnEntered;
        }
        levelChangeCounter = levelChangeTimer;
        isTimerRunning = true;
    }
    void Update()
    {
        if (!isTimerRunning)
            return;
        levelChangeCounter = levelChangeCounter - Time.deltaTime;
        int minutes = Mathf.FloorToInt(levelChangeCounter) / 60;
        int seconds = Mathf.FloorToInt(levelChangeCounter) % 60;
        if(minutes<10)
        {
            timerText.text = "0" + minutes + ":" + seconds;
        }
        else
        {
            timerText.text = minutes + ":" + seconds;
        }

        if (levelChangeCounter<=0)
        {
            isTimerRunning = false;
            SceneManager.LoadScene(GetComponent<N_VotesBox>().GetWinner());
        }
    }

    private void OnEntered(VoteSlot s)
    {
        levelBGImage.sprite = s.CoverSprite;
    }
}