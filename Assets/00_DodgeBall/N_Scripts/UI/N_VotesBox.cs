using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;
using System.Linq;

public class N_VotesBox : MonoBehaviour
{
    [Header("Read Only")]
    [SerializeField] VoteSlot activeSlot = null;
    List<VoteSlot> slots = new List<VoteSlot>();

    PhotonView pv = null;

    void Start()
    {
        slots = GetComponentsInChildren<VoteSlot>().ToList();
        pv = GetComponent<PhotonView>();
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
        if (s == null || s == activeSlot)
            return;

        if (activeSlot != null)
        {
            if (PhotonNetwork.IsConnected)
                pv.RPC("RemoveVote", RpcTarget.AllViaServer, activeSlot.SceneName);
            else
                RemoveVote(activeSlot.SceneName);
        }
        activeSlot = s;

        foreach (var slot in slots)
        {
            if (slot == s)
                continue;
            slot.DeSelect();
        }
        if (PhotonNetwork.IsConnected)
            pv.RPC("AddVote", RpcTarget.AllViaServer, s.SceneName);
        else
            AddVote(s.SceneName);
    }

    [PunRPC]
    private void AddVote(string toScene)
    {
        VoteSlot slot = slots.Single(s => s.SceneName == toScene);
        slot.AddVote();
    }
    [PunRPC]
    private void RemoveVote(string fromScene)
    {
        VoteSlot slot = slots.Single(s => s.SceneName == fromScene);
        slot.RemoveVote();
    }
    [PunRPC]
    private void ClickRpc(string xxx)
    {
        Debug.Log("WTF :: " + xxx);
    }

    public string GetWinner()
    {
        if(activeSlot)
            return activeSlot.SceneName;

        Log.Warning("Could Not, Local Winner, sending Stadium");
        return "SP Game";
    }
}