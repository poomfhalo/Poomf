using UnityEngine;

namespace Poomf.Data
{
    public abstract class DataLoaderAbstract : MonoBehaviour, IDataLoader
    {
        #region UNITY
        private void OnDestroy()
        {
            Reset();
        }
        #endregion

        #region IDataLoader
        public void Initialize()
        {
            updateData();
            subscribeToEvents();
        }

        public void Reset()
        {
            unsubscribeFromEvents();
        }

        #endregion

        #region PROTECTED API
        protected virtual void updateData()
        {
        }

        protected abstract void subscribeToEvents();
        protected abstract void unsubscribeFromEvents();

        #endregion
    }
}