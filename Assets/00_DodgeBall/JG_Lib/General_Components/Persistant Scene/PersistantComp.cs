using UnityEngine;

namespace GW_Lib.Utility
{
    public class PersistantComp<T> : MonoBehaviour where T : PersistantComp<T>
    {
        public static T instance
        {
            get
            {
                if(m_instance == null)
                {
                    m_instance = PersistantScene.GetComp<T>();
                }
                return m_instance;
            }
        }
        private static T m_instance;
    }
}