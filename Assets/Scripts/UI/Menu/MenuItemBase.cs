using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

namespace Poomf.UI
{
    public class MenuItemBase : MonoBehaviour
    {
        [SerializeField] private Button defaultMenuButton = null;

        [SerializeField] protected CinemachineVirtualCamera defaultCamera = null;

        // The animations controller of this UI item, if it's null, that means that this item has no animations
        public AUIAnimatedScreen AnimationsController { get; protected set; }
        // Indicates if this UI element is animated or not
        public bool IsAnimated { get; protected set; }

        protected virtual void Awake()
        {
            AnimationsController = GetComponent<AUIAnimatedScreen>();
            if (null == AnimationsController)
                IsAnimated = false;
            else
            {
                IsAnimated = true;
                AnimationsController.Initialize();
            }
        }

        protected virtual void OnEnable()
        {
            // Enable the menu item's camera
            if (defaultCamera != null)
                defaultCamera.gameObject.SetActive(true);
        }

        protected virtual void OnDisable()
        {
            // Disable the menu item's camera
            if (defaultCamera != null)
                defaultCamera.gameObject.SetActive(false);
        }

        public Button GetDefaultButton()
        {
            if (null == defaultMenuButton)
                Debug.LogError("MenuItemBase::GetDefaultButton -> Default button was not assigned.");

            return defaultMenuButton;
        }
    }
}