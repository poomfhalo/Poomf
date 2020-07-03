using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;

public class N_VotesBox : MonoBehaviour
{
    [Header("Read Only")]
    [SerializeField] VoteSlot activeSlot = null;
    VoteSlot[] slots = null;
    void Start()
    {
        slots = GetComponentsInChildren<VoteSlot>();
        foreach (var s in slots)
        {
            s.E_OnSelected += OnSelected;
        }
        foreach (var s in slots)
        {
            s.DeSelect();
        }
        if (activeSlot != null)
            activeSlot.OnPointerClick(null);
    }

    private void OnSelected(VoteSlot s)
    {
        activeSlot = s;
        foreach (var slot in slots)
        {
            if (slot == s)
                continue;
            slot.DeSelect();
        }
    }

    public string GetWinner()
    {
        if(activeSlot)
            return activeSlot.SceneName;

        Log.Warning("Could Not, Local Winner, sending Stadium");
        return "SP Game";
    }
}