using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Collections;

public class GameMenuController : MonoBehaviour
{
    public InputAction cancelInput = new InputAction();
    public InputAction switchUIVisiblity = new InputAction();
    public Action CancelFunc = null;
    public string exitScene = "Menu";
    [Header("Read Only")]
    [SerializeField] bool isLocked = false;

    PC pc = null;

    void Start()
    {
        StartCoroutine(DelayedStart());
    }
    IEnumerator DelayedStart()
    {
        yield return new WaitUntil(() => DodgeballGameManager.instance.GetLocalPlayer != null);
        pc = DodgeballGameManager.instance.GetLocalPlayer.GetComponent<PC>();

        if (!pc || !pc.enabled)
        {
            enabled = false;
            isLocked = true;
            yield break;
        }

        pc.onLockUpdated += OnLockUpdated;
        cancelInput.performed += OnCancel;
        switchUIVisiblity.performed += OnSwitchVisiblity;
        switchUIVisiblity.Enable();
        cancelInput.Enable();
    }

    private void OnSwitchVisiblity(InputAction.CallbackContext ctx)
    {
        if(!ctx.performed)
            return;
        if (isLocked)
            return;

        MatchStateUI matchStateUI = FindObjectOfType<MatchStateUI>();
        if (!matchStateUI)
            return;
        matchStateUI.SwitchVisiblity();
    }

    private void OnCancel(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed)
            return;
        if (isLocked)
            return;
        CancelFunc?.Invoke();
        SceneManager.LoadScene(exitScene);
    }

    private void OnLockUpdated(bool newState)
    {
        isLocked = newState;
    }
}