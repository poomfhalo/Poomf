using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;

public static class AccountManager
{
    #region Constants
    const string scriptsLocation = "http://naturechase-com.stackstaging.com/Poomf/Scripts";
    const string loginHelperLocation = scriptsLocation + "/loginhelper.php?";
    #endregion

    // The user name of the currently logged in user.
    static string username = String.Empty;

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
            statusCallback("Welcome to Poomf!");
            resultCallback(LoginResult.NewUser);
            username = name;
        }
        else if (www.downloadHandler.text == "1")
        {
            // Existing user
            statusCallback("Welcome back, " + name + "!");
            resultCallback(LoginResult.ExistingUser);
            username = name;
        }
        else
        {
            // Should never happen
            statusCallback(www.error);
            resultCallback(LoginResult.Failed);
        }
    }
}
