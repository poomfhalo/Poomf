using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataWriter : MonoBehaviour
{
    [Header("TESTING")]
    [SerializeField] private int coinsCount = 0;
    [SerializeField] private int gemsCount = 0;

    [Header("Player Data")]
    [SerializeField] private PlayerData playerData = null;

    public delegate void DataDelegate();
    public DataDelegate OnDataUpdated = null;

    private void Start()
    {
        loadData();
    }

    private void loadData()
    {
        playerData.coins = coinsCount;
        playerData.gems = gemsCount;

        if (null != OnDataUpdated)
            OnDataUpdated();
    }
}