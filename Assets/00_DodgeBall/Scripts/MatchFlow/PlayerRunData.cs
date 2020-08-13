using System;
using UnityEngine;

[Serializable]
public class PlayerRunData
{
    [Header("By Inspector")]
    public string charaName = "";
    public CharaSkinDataPlain charaSkinData = null;
    [Header("Only Code")]
    public int actorID = -1;

    public PlayerRunData() { }
    public PlayerRunData(int actorID, CharaSkinDataPlain charaSkinData, string charaName)
    {
        this.charaSkinData = charaSkinData;
        this.actorID = actorID;
        this.charaName = charaName;
    }
}