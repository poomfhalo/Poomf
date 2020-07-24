using UnityEngine;

namespace Poomf.Data
{
    public class SaveManager : MonoBehaviour
    {
        public static void SaveData<T>(string i_key, T i_value)
        {
            ES3.Save(i_key, i_value);
        }

        public static void SaveArrayData<T>(string i_key, T[] i_value)
        {
            ES3.Save(i_key, i_value);
        }

        public static T GetData<T>(string i_key, T i_defaultValue)
        {
            T data = default;

            if (ES3.KeyExists(i_key))
            {
                data = ES3.Load<T>(i_key);
            }
            else
            {
                SaveData(i_key, i_defaultValue);
                return i_defaultValue;
            }

            return data;
        }

        public static T[] GetArrayData<T>(string i_key, T[] i_defaultValue)
        {
            T[] data = default;

            if (ES3.KeyExists(i_key))
            {
                data = ES3.Load<T[]>(i_key);
            }
            else
            {
                SaveArrayData(i_key, i_defaultValue);
                return i_defaultValue;
            }

            return data;
        }
    }
}
