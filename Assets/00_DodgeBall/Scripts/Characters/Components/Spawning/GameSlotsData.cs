using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

[Serializable]
public class CharaSlotData
{
    public TeamTag team = TeamTag.A;
    public int id = 0;
}

[CreateAssetMenu(fileName = "GameSlotsData", menuName = "Dodgeball/GameSlotsData")]
public class GameSlotsData : ScriptableObject
{
    public List<CharaSlotData> slots = new List<CharaSlotData>();

    public CharaSlotData GetData(int id) => slots.Single(c => c.id == id);
}