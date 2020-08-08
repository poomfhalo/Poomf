using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Poomf.Data;
using UnityEngine.Networking;
using System.Threading.Tasks;

public static class AccountManager
{
    #region Constants
    const string scriptsLocation = "http://naturechase-com.stackstaging.com/Poomf/Scripts";
    const string loginHelperLocation = scriptsLocation + "/loginhelper.php?";
    const string es3CloudLocation = scriptsLocation + "/ES3Cloud.php";
    #endregion

    // The user name of the currently logged in user.
    static string username = String.Empty;
    // Used to sync cloud saves
    public static ES3Cloud cloud = new ES3Cloud(es3CloudLocation, "db0572a04fa0");
    #region Properties
    public static string Username { get { return username; } private set { username = value; } }
    #endregion

    /// <summary>
    /// Attempts to login using the supplied username.
    /// New users will be automaticall added to the Database server-side.
    /// </summary>
    /// <param name="statusCallback">
    /// Callback that's used to handle status messages, like displaying them in a text box.
    /// </param>
    /// <param name="statusCallback">
    /// Callback that's called when the login attempt is done, used to handle the attempt's result.
    /// </param>
    public static IEnumerator Login(string name, Action<string> statusCallback, Action<LoginResult> resultCallback)
    {
        // Create a new web request
        UnityWebRequest www = UnityWebRequest.Post(loginHelperLocation, name);
        // Display status
        statusCallback("Attempting to login...");
        // Wait for the request
        yield return www.SendWebRequest();

        // Check for errors
        if (www.isHttpError || www.isNetworkError)
        {
            statusCallback(www.error);
            resultCallback(LoginResult.Failed);
        }
        else if (www.downloadHandler.text == "-1")
        {
            // TODO: REMOVE AFTER TESTING PHASE
            // Duplicates found in the database!
            statusCallback("Duplicate matches found in the database!");
            resultCallback(LoginResult.Failed);
        }
        else if (www.downloadHandler.text == "0")
        {
            // New user
            username = name;
            statusCallback("Welcome to Poomf!");
            resultCallback(LoginResult.NewUser);
        }
        else if (www.downloadHandler.text == "1")
        {
            // Existing user
            username = name;
            statusCallback("Welcome back, " + name + "!");
            resultCallback(LoginResult.ExistingUser);
        }
        else
        {
            // Should never happen
            statusCallback(www.error);
            resultCallback(LoginResult.Failed);
        }
    }

    public static async Task SyncAllData(Action<string> statusCallback = null)
    {
        statusCallback?.Invoke("Syncing Skin Data...");
        await SyncCharaSkinData();
        statusCallback?.Invoke("Syncing Save Data...");
        await SyncSaveFile();
    }

    public static async Task SyncCharaSkinData(Action<string> statusCallback = null)
    {
        Debug.Log("Syncing");
        await cloud.Sync(SaveManager.relativeSkinDataPath, username);
        if (cloud.isError)
            Debug.LogError(cloud.error);
    }

    public static async Task SyncSaveFile(Action<string> statusCallback = null)
    {
        await cloud.Sync(ES3Settings.defaultSettings.path, username);
        if (cloud.isError)
            Debug.LogError(cloud.error);
    }
}
