using System.Collections;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// General script for all main menu animated screens
/// </summary>
public class MainMenuAnimatedScreen : AUIAnimatedScreen
{
    [SerializeField] private Vector3 shrinkScale = Vector3.zero;
    // The edges of the screen that the screens will fade in/out to or from
    [SerializeField] Transform midLeftEdge;
    [SerializeField] Transform midRightEdge;

    #region AUIAnimatedScreen
    public override void Initialize()
    {
        if (initialized)
            return;
        //leftEdge = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height / 2, 1));
        //rightEdge = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height / 2, 1));
        ApplyInitialState();
        initialized = true;
    }
    public override IEnumerator AnimateIn(AnimationProperties properties = null)
    {
        if (gameObject.activeSelf)
        {
            Debug.LogWarning("AnimateIn was called in MainMenuAnimatedScreen although the object was active!");
        }
        else if (properties == null)
        {
            Debug.LogWarning("MainMenuAnimatedScreens need properties! Please pass a properties parameter.");
        }
        else
        {
            gameObject.SetActive(true);
            // Determine which point to animate from
            Vector3 pointToAnimateFrom = GetAnimatePoint(properties.Direction);

            // Start from the shrinked scale
            transform.localScale = shrinkScale;
            // Place it at the starting point
            transform.position = pointToAnimateFrom;
            // Tween to base position
            transform.DOMove(basePosition.position, animDuration).SetEase(Ease.InBack);
            yield return new WaitForSeconds(animDuration);
            // Enlarge
            transform.DOScale(Vector3.one, animDuration).SetEase(Ease.InBack);
            yield return new WaitForSeconds(animDuration);
        }
    }
    public override IEnumerator AnimateOut(AnimationProperties properties = null)
    {
        if (!gameObject.activeSelf)
        {
            Debug.LogWarning("AnimateOut was called in MainMenuAnimatedScreen although the object was inactive!");
        }
        else if (properties == null)
        {
            Debug.LogWarning("MainMenuAnimatedScreens need properties! Please pass a properties parameter.");
        }
        else
        {
            // Determine which point to animate to
            Vector3 pointToAnimateTo = GetAnimatePoint(properties.Direction);
            // Shrink
            transform.DOScale(shrinkScale, animDuration).SetEase(Ease.InBack);
            yield return new WaitForSeconds(animDuration);

            // Move to corner
            transform.DOMove(pointToAnimateTo, animDuration).SetEase(Ease.InBack);
            yield return new WaitForSeconds(animDuration);
            gameObject.SetActive(false);
        }
    }
    #endregion

    // Return the point to animate out/in to/from based on the supplied direction
    private Vector3 GetAnimatePoint(AnimationDirection dir)
    {
        if (dir == AnimationDirection.Left)
        {
            return midLeftEdge.position;
        }
        else if (dir == AnimationDirection.Right)
        {
            return midRightEdge.position;
        }
        else
        {
            return transform.position;
        }
    }
}
