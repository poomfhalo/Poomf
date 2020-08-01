using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelsRoom : MonoBehaviour
{
    public Func<IEnumerator> TryLoadLevelFunc = null;
    public bool extLoadOnClick = true;

    [Header("Constants")]
    [SerializeField] Image levelBGImage = null;
    [SerializeField] TextMeshProUGUI timerText = null;
    [SerializeField] MatchState matchState = null;
    [Header("Variables")]
    [Tooltip("In Seconds")]
    [SerializeField] float levelChangeTimer = 30;

    Coroutine loadLevelCoro = null;
    float levelChangeCounter = 0;
    bool isTimerRunning = false;
    VoteSlot[] delegators = null;
    string selectedSceneName = "";

    void Awake()
    {
        levelChangeCounter = levelChangeTimer;
        isTimerRunning = true;
        delegators = GetComponentsInChildren<VoteSlot>();

        foreach (var d in delegators)
        {
            d.E_OnPointerEntered += OnEntered;
            d.E_OnSelected += OnSelected;
        }

        TryLoadLevelFunc = TryLoadLevel;
    }

    void Update()
    {
        if (!isTimerRunning)
            return;

        levelChangeCounter = levelChangeCounter - Time.deltaTime;
        int minutes = Mathf.FloorToInt(levelChangeCounter) / 60;
        int seconds = Mathf.FloorToInt(levelChangeCounter) % 60;
        seconds = Mathf.FloorToInt(Mathf.Clamp(seconds, 0, levelChangeTimer));

        string m = "";
        string s = "";

        if (minutes < 10)
        {
            m = "0" + minutes;
        }
        else
        {
            m = minutes.ToString();
        }
        if (seconds < 10)
        {
            s = "0" + seconds;
        }
        else
        {
            s = seconds.ToString();
        }
        timerText.text = m + ":" + s;

        if (levelChangeCounter <= 0)
        {
            isTimerRunning = false;
            if (loadLevelCoro == null)
                loadLevelCoro = StartCoroutine(TryLoadLevelFunc());
        }
    }

    private IEnumerator TryLoadLevel()
    {
        yield return 0;
        SceneFader.instance.FadeIn(1, () =>{
            if (string.IsNullOrEmpty(selectedSceneName))
            {
                int i = UnityEngine.Random.Range(0, delegators.Length);
                selectedSceneName = delegators[i].SceneName;
            }
            matchState.StartNewGame(selectedSceneName);
            SceneManager.LoadScene(selectedSceneName);
        });
    }

    private void OnEntered(VoteSlot s)
    {
        levelBGImage.sprite = s.CoverSprite;
    }
    private void OnSelected(VoteSlot slot)
    {
        if (loadLevelCoro != null)
            return;
        if (!extLoadOnClick)
            return;

        selectedSceneName = slot.SceneName;
        isTimerRunning = false;
        loadLevelCoro = StartCoroutine(TryLoadLevelFunc());
    }
}