using UnityEngine;
using System;

namespace Poomf.Data
{
    [CreateAssetMenu(fileName = "PlayerData", menuName = "ScriptableObjects/PlayerData", order = 1)]
    public class PlayerData : ScriptableObject
    {
        [SerializeField] private int maxCoins = 9999999;
        [SerializeField] private int maxGems = 9999;
        [NonSerialized] private int coins = 0;
        [NonSerialized] private int gems = 0;

        public void AddCoins(int i_value)
        {
            addToCurrency(i_value, ref coins, maxCoins);
        }

        public void AddGems(int i_value)
        {
            addToCurrency(i_value, ref gems, maxGems);
        }

        public int Coins { get { return coins; } }
        public int Gems { get { return gems; } }

        private void addToCurrency(int i_value, ref int i_currency, int i_maxValue)
        {
            i_currency = Mathf.Clamp(i_currency + i_value, 0, i_maxValue);
        }
    }
}