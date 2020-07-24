using UnityEngine;
using UnityEngine.UI;
using Poomf.Data;

namespace Poomf.UI
{
    public class CurrencyBar : CurrencyDataLoaderAbstract
    {
        [SerializeField] private Text i_coinsText = null;
        [SerializeField] private Text i_gemsText = null;

        protected override void updateData()
        {
            if (null != i_coinsText)
                i_coinsText.text = CurrencyDataManager.Coins.ToString();

            if (null != i_gemsText)
                i_gemsText.text = CurrencyDataManager.Gems.ToString();
        }
    }
}