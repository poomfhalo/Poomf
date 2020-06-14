using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurrencyBar : MonoBehaviour, IPlayerDataReader
{
    [SerializeField] private Text i_coinsText = null;
    [SerializeField] private Text i_gemsText = null;

    public void Initialize(PlayerData i_playerData)
    {
        if (null == i_playerData)
        {
            Debug.LogError("CurrencyBar::Initialize -> Player data is null.");
            return;
        }

        if (null != i_coinsText)
            i_coinsText.text = i_playerData.coins.ToString();

        if (null != i_gemsText)
            i_gemsText.text = i_playerData.gems.ToString();
    }
}
