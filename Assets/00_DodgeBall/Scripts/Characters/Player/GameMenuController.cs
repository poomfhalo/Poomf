using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameMenuController : MonoBehaviour
{
    public InputAction cancelInput = new InputAction();
    public Action CancelFunc = null;
    public string exitScene = "Menu";
    [Header("Read Only")]
    [SerializeField] bool isLocked = false;

    PC pc = null;

    void Start()
    {
        pc = GetComponent<PC>();

        if (!pc || !pc.enabled)
        {
            enabled = false;
            isLocked = true;
            return;
        }

        pc.onLockUpdated += OnLockUpdated;
        cancelInput.performed += OnCancel;
        cancelInput.Enable();
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