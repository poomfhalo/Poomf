using UnityEngine;
using UnityEngine.InputSystem;

namespace Poomf.Data
{
    public class DataDebugger : MonoBehaviour
    {
#if UNITY_EDITOR

        #region UNITY
        // Start is called before the first frame update
        void Start()
        {
            initialize();
        }

        // Update is called once per frame
        void Update()
        {
            manageInput();
        }
        #endregion

        #region PRIVATE

        void initialize()
        {
            debugAllCurrencies();
        }

        void manageInput()
        {
            if (Keyboard.current.cKey.wasPressedThisFrame)
            {
                addCoins(5);
            }
            else if (Keyboard.current.gKey.wasPressedThisFrame)
            {
                addGems(5);
            }
        }

        void addCoins(int i_coinsToAdd)
        {
            CurrencyDataManager.AddCoins(i_coinsToAdd);
            debugCoins();
        }

        void addGems(int i_gemsToAdd)
        {
            CurrencyDataManager.AddGems(i_gemsToAdd);
            debugGems();
        }

        void debugAllCurrencies()
        {
            debugCoins();
            debugGems();
        }

        void debugCoins()
        {
            Debug.LogWarning("Coins value: " + CurrencyDataManager.Coins);
        }

        void debugGems()
        {
            Debug.LogWarning("Coins value: " + CurrencyDataManager.Gems);
        }

        #endregion

#endif
    }
}