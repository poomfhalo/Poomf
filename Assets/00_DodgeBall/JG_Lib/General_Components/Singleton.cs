using UnityEngine;

namespace GW_Lib.Utility
{
    public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        [SerializeField] bool isPersistant = false;
        public static T instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = FindObjectOfType<T>();
                    if (m_instance == null)
                    {
                        Debug.LogWarning("couldn't find singleton of type " + typeof(T));
                        return m_instance;
                    }
                }
                return m_instance;
            }
        }
        private static T m_instance;
        protected virtual void Awake()
        {
            if (m_instance == null)
            {
                m_instance = this as T;
                if (isPersistant)
                {
                    DontDestroyOnLoad(m_instance);
                }
            }
            if (m_instance != null && m_instance != this)
            {
                Destroy(gameObject);
            }
        }
    }
}