using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Poomf.Data
{
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
            playerData.AddCoins(coinsCount);
            playerData.AddGems(gemsCount);

            if (null != OnDataUpdated)
                OnDataUpdated();
        }
    }
}