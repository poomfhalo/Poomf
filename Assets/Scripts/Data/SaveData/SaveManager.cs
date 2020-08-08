using UnityEngine;

namespace Poomf.Data
{
    public class SaveManager : MonoBehaviour
    {
        #region Keys
        public const string charaSkinKey = "chara_skin_data";
        public const string audioSettingsKey = "audio_settings";
        public const string videoSettingsKey = "video_settings";

        #endregion

        #region Relative Paths
        public static string relativeSkinDataPath = "SkinData.es3";
        // Settings are global, not user specific
        public const string relativeSettingsPath = "Settings.es3";
        #endregion

        /// <summary>
        /// Sets the relative save paths to a folder named after the player's name
        /// </summary>
        public static void SetRelativeDataPaths()
        {
            ES3Settings.defaultSettings.path = "Saves/" + AccountManager.Username + "/Save.es3";
            relativeSkinDataPath = "Saves/" + AccountManager.Username + "/" + relativeSkinDataPath;
        }

        public static void SaveData<T>(string i_key, T i_value)
        {
            ES3.Save(i_key, i_value);
        }

        public static void SaveData<T>(string i_key, T i_value, string relativeFilePath)
        {
            ES3.Save(i_key, i_value, relativeFilePath);
        }

        public static void SaveData<T>(string i_key, T i_value, ES3Settings customSettings)
        {
            ES3.Save(i_key, i_value, customSettings);
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

        public static T GetData<T>(string i_key, T i_defaultValue, string relativeFilePath)
        {
            T data = default;

            if (ES3.KeyExists(i_key, relativeFilePath))
            {
                data = ES3.Load<T>(i_key, relativeFilePath);
            }
            else
            {
                SaveData(i_key, i_defaultValue, relativeFilePath);
                return i_defaultValue;
            }

            return data;
        }

        public static T GetData<T>(string i_key, T i_defaultValue, ES3Settings customSettings)
        {
            T data = default;

            if (ES3.KeyExists(i_key))
            {
                data = ES3.Load<T>(i_key, customSettings);
            }
            else
            {
                SaveData(i_key, i_defaultValue, customSettings);
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
