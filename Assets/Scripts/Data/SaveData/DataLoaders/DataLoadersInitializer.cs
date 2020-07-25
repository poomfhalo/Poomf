using UnityEngine;

namespace Poomf.Data
{
    public class DataLoadersInitializer : MonoBehaviour
    {
        [SerializeField] private DataLoaderAbstract[] dataLoaderComponents = null;

        private void Awake()
        {
            initializeDataLoaders();
        }

        private void initializeDataLoaders()
        {
            if (null == dataLoaderComponents || 0 == dataLoaderComponents.Length) return;

            int dataLoadersCount = dataLoaderComponents.Length;

            for (int i = 0; i < dataLoadersCount; i++)
            {
                dataLoaderComponents[i].Initialize();
            }
        }
    }
}
