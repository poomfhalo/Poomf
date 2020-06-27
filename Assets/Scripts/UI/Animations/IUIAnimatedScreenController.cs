using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides functions that control an animated screen, without giving access to anything else
/// </summary>
public interface IUIAnimatedScreenController
{
    IEnumerator AnimateIn();
    IEnumerator AnimateOut();
}
