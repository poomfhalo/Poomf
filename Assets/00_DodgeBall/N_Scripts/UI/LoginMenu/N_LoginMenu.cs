using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using System;
using UnityEngine.SceneManagement;
using Poomf.Data;

public enum LoginResult
{
    Failed,
    ExistingUser,
    NewUser
}

public class N_LoginMenu : MonoBehaviour
{
    //public event Action<string> onNameSet = null;
    [SerializeField] TMP_InputField nickName = null;
    [SerializeField] Text statusText = null;
    [SerializeField] Button login = null;
    // [SerializeField] MenuAnimationsController animController = null;

    event Action<string> statusMessagesHandler = null;
    event Action<LoginResult> loginResultHandler = null;

    void Start()
    {
        login.onClick.AddListener(OnLoginPressed);
        // Callback used to display status messages when loading/loggin in/etc
        statusMessagesHandler = DisplayStatusMessage;
        // Callback that's called when the account manager is done trying to login.
        loginResultHandler = OnLoginAttemptDone;
        // Clear status text
        statusText.text = string.Empty;
        // animController.ShowScreen(animController.LoginAnimatedScreen);
    }

    private void OnLoginPressed()
    {
        if (string.IsNullOrEmpty(nickName.text))
            return;

        // Disable the Login button
        login.interactable = false;
        StartCoroutine(AccountManager.Login(nickName.text, statusMessagesHandler, loginResultHandler));
        // PhotonNetwork.NickName = nickName.text;

        // animController.HideScreen(animController.LoginAnimatedScreen);
        // onNameSet?.Invoke(nickName.text);
    }

    void DisplayStatusMessage(string message)
    {
        statusText.text = message;
    }

    void OnLoginAttemptDone(LoginResult result)
    {
        if (result == LoginResult.Failed)
        {
            // Failure specific code here
            Debug.Log("Login Failed!");
            // Clear the Login input field
            nickName.text = String.Empty;
            // Re-enable the login button
            login.interactable = true;
            return;
        }

        // Set the relative data paths
        SaveManager.SetRelativeDataPaths();

        if (result == LoginResult.ExistingUser)
        {
            // Existing user specific code here
            Debug.Log("Existing user logged in!");
            // Sync the player's data
            //await AccountManager.SyncAllData(statusMessagesHandler);
        }
        else if (result == LoginResult.NewUser)
        {
            // New user specific code here
            Debug.Log("New user logged in!");
            // Ask for gender here
        }

        // Will always happen if login is successful
        PhotonNetwork.NickName = nickName.text;
        // Load the Menu scene
        SceneFader.instance.FadeIn(1f, () => { SceneManager.LoadScene("Menu"); });
        //onNameSet?.Invoke(nickName.text);
    }
}