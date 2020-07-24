namespace Poomf.Data
{
    public class CurrencyDataLoaderAbstract : DataLoaderAbstract
    {
        protected override void subscribeToEvents()
        {
            CurrencyDataManager.OnCurrencyDataUpdated += updateData;
        }

        protected override void unsubscribeFromEvents()
        {
            CurrencyDataManager.OnCurrencyDataUpdated -= updateData;
        }
    }
}