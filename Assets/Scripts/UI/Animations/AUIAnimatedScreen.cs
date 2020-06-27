using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AUIAnimatedScreen : MonoBehaviour, IUIAnimatedScreenController
{
    // The animations controller that's taking care of this screen
    [SerializeField] private MenuAnimationsController animationsController;
    public string ScreenID { get; protected set; }

    private void Awake()
    {
        Initialize();
        animationsController.RegisterScreen(ScreenID, this);
    }
    
    protected abstract void Initialize();
    public abstract IEnumerator AnimateIn();
    public abstract IEnumerator AnimateOut();

}
