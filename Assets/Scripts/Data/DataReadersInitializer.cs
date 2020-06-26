using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Poomf.Data
{
    public class DataReadersInitializer : MonoBehaviour
    {
        [SerializeField] private MonoBehaviour[] dataReaderComponents = null;
        [SerializeField] private PlayerData playerData = null;
        [SerializeField] private DataWriter dataWriter = null;

        private void Awake()
        {
            subscribeToEvents();
        }

        private void OnDestroy()
        {
            unsubscribeFromEvents();
        }

        private void subscribeToEvents()
        {
            if (null != dataWriter)
                dataWriter.OnDataUpdated += onPlayerDataUpdatedCallback;
        }

        private void unsubscribeFromEvents()
        {
            if (null != dataWriter)
                dataWriter.OnDataUpdated -= onPlayerDataUpdatedCallback;
        }

        private void onPlayerDataUpdatedCallback()
        {
            initializeDataReaders();
        }

        private void initializeDataReaders()
        {
            IEnumerable dataReaders = dataReaderComponents.OfType<IPlayerDataReader>();

            foreach (IPlayerDataReader dataReader in dataReaders)
            {
                dataReader.Initialize(playerData);
            }
        }
    }
}
