using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyAnimatedScreen : AMainMenuAnimatedScreen
{
    protected override void Initialize()
    {
        ScreenID = ScreensIDs.LobbyID;
        base.Initialize();
    }
}
