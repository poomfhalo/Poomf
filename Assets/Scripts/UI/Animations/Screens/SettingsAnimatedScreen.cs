using System.Collections;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using Coffee.UIEffects;

public class SettingsAnimatedScreen : AUIAnimatedScreen
{
    // Used to fade the image covering the settings screen
    [SerializeField] private UITransitionEffect fadeEffect;
    // The min scale that the screen shrinks to if it's animating out, or grows from if it's animating in
    [SerializeField] private Vector3 shrinkScale;

    // The base duration of different tween animations
    private float animDuration = 1f;
    // Bottom left corner of the screen
    private Vector3 cornerPoint;
    // The original position
    private Vector3 basePosition;

    #region AUIAnimatedScreen
    protected override void Initialize()
    {
        ScreenID = ScreensIDs.SettingsMenuID;
        cornerPoint = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 1));
        basePosition = transform.position;
        // Settings window is initially disabled
        gameObject.SetActive(false);
    }
    public override IEnumerator AnimateIn()
    {
        if (gameObject.activeSelf)
        {
            Debug.LogWarning("AnimateIn was called in SettingsAnimatedScreen although the object was active!");
        }
        else
        {
            gameObject.SetActive(true);
            // Show the cover image
            fadeEffect.effectFactor = 1;
            // Start from the shrinked scale
            transform.localScale = shrinkScale;
            // Place it at the top center of the screen
            transform.position = cornerPoint;
            // Tween to base position
            transform.DOMove(basePosition, animDuration).SetEase(Ease.InBack);
            yield return new WaitForSeconds(animDuration);
            // Enlarge
            transform.DOScale(Vector3.one, animDuration).SetEase(Ease.InBack);
            yield return new WaitForSeconds(animDuration);
            // Fade the cover image
            // Interpolator
            float t = 0;
            while (fadeEffect.effectFactor > 0)
            {
                fadeEffect.effectFactor = Mathf.Lerp(1f, 0f, t);
                t += Time.deltaTime;
                yield return null;
            }
        }
    }
    public override IEnumerator AnimateOut()
    {
        if (!gameObject.activeSelf)
        {
            Debug.LogWarning("AnimateOut was called in SettingsAnimatedScreen although the object was inactive!");
        }
        else
        {
            // Show the cover image
            // Interpolator
            float t = 0;
            while (fadeEffect.effectFactor < 1)
            {
                fadeEffect.effectFactor = Mathf.Lerp(0f, 1f, t);
                t += Time.deltaTime;
                yield return null;
            }

            // Shrink
            transform.DOScale(shrinkScale, animDuration - 0.2f).SetEase(Ease.InBack);
            yield return new WaitForSeconds(animDuration - 0.2f);

            // Move to corner
            transform.DOMove(cornerPoint, animDuration - 0.2f).SetEase(Ease.InBack);
            yield return new WaitForSeconds(animDuration - 0.2f);
            gameObject.SetActive(false);
        }
    }
    #endregion
}
