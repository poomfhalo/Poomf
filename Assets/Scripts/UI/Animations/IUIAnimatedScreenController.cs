using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides functions that control an animated screen, without giving access to anything else
/// </summary>
public interface IUIAnimatedScreenController
{
    // Animate the screen in or out with optional parameters describing their animation
    IEnumerator AnimateIn(AnimationProperties properties = null);
    IEnumerator AnimateOut(AnimationProperties properties = null);
}
