using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Holds optional properties for menu animations, like fading to or from a certain direction
public class AnimationProperties
{
    // The direction of the menu that our current menu is being swapped with.
    // Ex: if locker is active and we want to show lobby which is to its left,
    // We will send "right" to locker's AnimateOut and "left" to lobby's
    // AnimateIn
    public AnimationDirection Direction { set; get; } = AnimationDirection.Left;
}

public enum AnimationDirection
{
    Left,
    Right
}
