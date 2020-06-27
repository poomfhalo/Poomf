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
    [SerializeField] protected bool initialState = false;
    public string ScreenID { get; protected set; } = "notset";

    private void Awake()
    {
        Initialize();
        animationsController.RegisterScreen(ScreenID, this);
    }

    protected abstract void Initialize();

    public abstract IEnumerator AnimateIn(AnimationProperties properties = null);
    public abstract IEnumerator AnimateOut(AnimationProperties properties = null);

}
