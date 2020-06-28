using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AUIAnimatedScreen : MonoBehaviour, IUIAnimatedScreenController
{
    // The animations controller that's taking care of this screen
    [SerializeField] private MenuAnimationsController animationsController;
    // The base duration of different animations
    [SerializeField] protected float animDuration = 1f;
    [Tooltip("Is this screen initially enabled (true) or disabled (false)?")]
    [SerializeField] private bool initialState = false;
    public string ScreenID { get; set; } = "notset";

    private void Awake()
    {
        Initialize();
        Register();
    }

    // Adds this screen to the animations controller's list.
    public void Register()
    {
        if (ScreenID != "notset")
            animationsController.RegisterScreen(ScreenID, this);
    }
    public void ApplyInitialState()
    {
        gameObject.SetActive(initialState);
    }
    protected abstract void Initialize();

    public abstract IEnumerator AnimateIn(AnimationProperties properties = null);
    public abstract IEnumerator AnimateOut(AnimationProperties properties = null);

}
