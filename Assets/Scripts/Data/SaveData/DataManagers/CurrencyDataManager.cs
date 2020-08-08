namespace Poomf.Data
{
    public class CurrencyDataManager : DataManager
    {
        public static DataManagerDelegate OnCurrencyDataUpdated = null;

        private const string coinsKey = "currency_coins";
        private const string gemsKey = "currency_gems";
        private const int defaultCoinsValue = 1000;
        private const int defaultGemsValue = 50;

        #region PUBLIC API

        public static void AddCoins(int i_coinsToAdd)
        {
            addCoins(i_coinsToAdd);
        }

        public static void AddGems(int i_gemsToAdd)
        {
            addGems(i_gemsToAdd);
        }

        public static int Coins { get { return SaveManager.GetData(coinsKey, defaultCoinsValue); } }
        public static int Gems { get { return SaveManager.GetData(gemsKey, defaultGemsValue); } }

        #endregion

        #region PRIVATE

        static void addCoins(int i_coinsToAdd)
        {
            addCurrency(coinsKey, defaultCoinsValue, i_coinsToAdd);
        }

        static void addGems(int i_gemsToAdd)
        {
            addCurrency(gemsKey, defaultGemsValue, i_gemsToAdd);
        }

        static void addCurrency(string i_currencyKey, int i_defaultValue, int i_coinsToAdd)
        {
            int currentCurrencyValue = SaveManager.GetData(i_currencyKey, i_defaultValue) + i_coinsToAdd;
            SaveManager.SaveData(i_currencyKey, currentCurrencyValue);

            if (null != OnCurrencyDataUpdated)
                OnCurrencyDataUpdated();
        }

        #endregion
    }
}