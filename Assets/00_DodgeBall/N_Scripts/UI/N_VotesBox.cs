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

    public string GetMostVotedScene()
    {
        int highestVotes = int.MinValue;
        foreach (var slot in slots)
        {
            if (slot.votesCount > highestVotes)
            {
                highestVotes = slot.votesCount;
            }
        }
        List<VoteSlot> highestSlots = slots.FindAll(s => s.votesCount == highestVotes);
        if (highestSlots.Count == 0)
        {
            Log.Warning("Could Not, Local Winner, sending to MP Game (Development Level)");
            return "MP Game";
        }
        if(highestSlots.Count == 1)
        {
            return highestSlots[0].SceneName;
        }
        int i = UnityEngine.Random.Range(0, highestSlots.Count);
        return highestSlots[i].SceneName;
    }
}