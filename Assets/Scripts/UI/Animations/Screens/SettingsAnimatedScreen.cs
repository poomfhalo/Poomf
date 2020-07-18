using System.Collections;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using Coffee.UIEffects;

public class SettingsAnimatedScreen : AUIAnimatedScreen
{
    // Used to fade the image covering the settings screen
    [SerializeField] private UITransitionEffect menuCoverFade = null;
    [SerializeField] private UITransitionEffect backgroundFade = null;
    // The min scale that the screen shrinks to if it's animating out, or grows from if it's animating in
    [SerializeField] private Vector3 shrinkScale = Vector3.zero;
    [Tooltip("The speed at which the menu fades/appears.")]
    [SerializeField] private float fadeSpeed = 1f;


    // Bottom left corner of the screen
    private Vector3 cornerPoint;

    #region AUIAnimatedScreen
    public override void Initialize()
    {
        if (initialized)
            return;
        cornerPoint = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 1));
        // Settings window is initially disabled
        ApplyInitialState();
        initialized = true;
    }
    public override IEnumerator AnimateIn(AnimationProperties properties = null)
    {
        if (gameObject.activeSelf)
        {
            Debug.LogWarning("AnimateIn was called in SettingsAnimatedScreen although the object was active!");
        }
        else
        {
            gameObject.SetActive(true);
            // Show the cover image
            menuCoverFade.effectFactor = 1;
            // Hide the background image
            backgroundFade.effectFactor = 0;
            // Start from the shrinked scale
            transform.localScale = shrinkScale;
            // Place it at the top center of the screen
            transform.position = cornerPoint;
            // Tween to base position
            transform.DOMove(basePosition.position, animDuration).SetEase(Ease.InBack);
            yield return new WaitForSeconds(animDuration);
            // Enlarge
            transform.DOScale(Vector3.one, animDuration).SetEase(Ease.InBack);
            yield return new WaitForSeconds(animDuration);
            // Fade the cover image, show the background
            // Interpolator
            float t = 0;
            while (menuCoverFade.effectFactor > 0 || backgroundFade.effectFactor < 1)
            {
                menuCoverFade.effectFactor = Mathf.Lerp(1f, 0f, t);
                backgroundFade.effectFactor = Mathf.Lerp(0f, 1f, t);
                t += Time.deltaTime * fadeSpeed;
                yield return null;
            }
        }
    }
    public override IEnumerator AnimateOut(AnimationProperties properties = null)
    {
        if (!gameObject.activeSelf)
        {
            Debug.LogWarning("AnimateOut was called in SettingsAnimatedScreen although the object was inactive!");
        }
        else
        {
            // Show the cover image, hide the background
            // Interpolator
            float t = 0;
            while (menuCoverFade.effectFactor < 1 || backgroundFade.effectFactor > 0)
            {
                backgroundFade.effectFactor = Mathf.Lerp(1f, 0f, t);
                menuCoverFade.effectFactor = Mathf.Lerp(0f, 1f, t);
                t += Time.deltaTime * fadeSpeed;
                yield return null;
            }

            // Shrink
            transform.DOScale(shrinkScale, animDuration).SetEase(Ease.InBack);
            yield return new WaitForSeconds(animDuration);

            // Move to corner
            transform.DOMove(cornerPoint, animDuration).SetEase(Ease.InBack);
            yield return new WaitForSeconds(animDuration);
            gameObject.SetActive(false);
        }
    }
    #endregion
}
