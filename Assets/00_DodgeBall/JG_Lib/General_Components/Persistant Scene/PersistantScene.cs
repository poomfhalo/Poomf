using UnityEngine;

namespace GW_Lib.Utility
{
    public class PersistantScene : MonoBehaviour
    {
        [SerializeField] bool dontDestroyOnLoad = true;
        public static GameObject instanceObj => instance.gameObject;
        public static PersistantScene instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = FindObjectOfType<PersistantScene>();
                }
                if (m_instance == null)
                {
                    GameObject prefab = Resources.Load<GameObject>("Persistant Scene");
                    GameObject prefabInst = Instantiate(prefab);
                    prefabInst.name = "From_Script_" + prefabInst.name;
                    m_instance = prefabInst.GetComponent<PersistantScene>();
                }
                return m_instance;
            }
        }
        private static PersistantScene m_instance;

        public static T GetComp<T>() where T : Component
        {
            return instance.GetComponentInChildren<T>();
        }

        void Awake()
        {
            if(dontDestroyOnLoad)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
    }
}