using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RoundTimerUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text = null;
    RoundTimer roundTimer = null;

    void Start()
    {
        roundTimer = FindObjectOfType<RoundTimer>();
        roundTimer.E_OnTimeUpdated += OnTimeUpdated;
    }

    private void OnTimeUpdated(float newTime)
    {
        text.text = GameExtentions.GetTimeAsMinSecPair(newTime, 1, 1);
    }
}