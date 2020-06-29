using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AUIAnimatedScreen : MonoBehaviour
{
    [Tooltip("The base duration that each animation phase takes. Lower = faster animations.")]
    [SerializeField] protected float animDuration = 1f;
    [Tooltip("Is this screen initially enabled (true) or disabled (false)?")]
    [SerializeField] private bool initialState = false;

    protected bool initialized = false;

    public void ApplyInitialState()
    {
        gameObject.SetActive(initialState);
    }

    /// <summary>
    /// Should be called by the animated screen's controller
    /// </summary>
    public abstract void Initialize();

    public abstract IEnumerator AnimateIn(AnimationProperties properties = null);
    public abstract IEnumerator AnimateOut(AnimationProperties properties = null);

}
