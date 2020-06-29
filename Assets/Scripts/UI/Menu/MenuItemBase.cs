using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Poomf.UI
{
    public class MenuItemBase : MonoBehaviour
    {
        [SerializeField] private Button defaultMenuButton = null;

        // The animations controller of this UI item, if it's null, that means that this item has no animations
        public AUIAnimatedScreen AnimationsController { get; protected set; }
        // Indicates if this UI element is animated or not
        public bool IsAnimated { get; protected set; }

        private void Awake()
        {
            AnimationsController = GetComponent<AUIAnimatedScreen>();
        }

        private void Start()
        {
            if (null == AnimationsController)
                IsAnimated = false;
            else
            {
                IsAnimated = true;
                AnimationsController.ApplyInitialState();
            }
        }

        public Button GetDefaultButton()
        {
            if (null == defaultMenuButton)
                Debug.LogError("MenuItemBase::GetDefaultButton -> Default button was not assigned.");

            return defaultMenuButton;
        }
    }
}