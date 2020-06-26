using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Poomf.UI
{
    public class MenuTabButton : MonoBehaviour, ISelectHandler
    {
        public delegate void ButtonSelectionDelegate(int i_menuIndex);
        public ButtonSelectionDelegate OnMenuButtonSelect = null;

        private Button myButton = null;
        private int menuIndex = 0;
        private bool initialized = false;

        public void Initialize(int i_menuIndex)
        {
            if (true == initialized) return;

            myButton = GetComponent<Button>();
            menuIndex = i_menuIndex;

            initialized = true;
        }

        public void Select()
        {
            if (false == initialized || null == myButton)
                return;

            myButton.Select();
        }

        public void OnSelect(BaseEventData eventData)
        {
            if (false == initialized || null == myButton)
                return;

            if (null != OnMenuButtonSelect)
                OnMenuButtonSelect(menuIndex);
        }

        public void ResetButton()
        {
            OnMenuButtonSelect = null;
            menuIndex = 0;
            initialized = false;
        }
    }
}