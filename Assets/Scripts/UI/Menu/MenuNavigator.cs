using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Poomf.UI
{
    public class MenuNavigator : MonoBehaviour
    {
        [SerializeField] private MenuAnimationsController mainAnimController = null;
        [SerializeField] private Transform menuTabsButtonsTransform = null;
        [SerializeField] private MenuItemBase[] menuItems = null;
        [SerializeField] private MenuTabButton defaultMenuButton = null;

        private PlayerInput playerInput = null;
        private MenuTabButton[] menuTabsButtons = null;
        private int? currentActiveMenuIndex = null;
        private bool initialized = false;

        #region UNITY
        private void Awake()
        {
            // playerInput = new PlayerInput();
        }

        private void OnEnable()
        {
            if (false == initialized) return;
            // playerInput.UI.Enable();
            launchDefaultMenu();
        }

        private void OnDisable()
        {
            // playerInput.UI.Disable();
        }

        private void Start()
        {
            initialize();
        }

        private void OnDestroy()
        {
            reset();
        }
        /*
        private void Update()
        {
            processPlayerInput();
        }
        */
        #endregion

        #region PRIVATE
        private void initialize()
        {
            if (false == initializeButtons() ||
                false == initializeMenus() ||
                false == bindButtonsToMenus() ||
                false == launchDefaultMenu() ||
                false == bindPlayerInput()
                )
            {
                Debug.LogError("MenuNavigator::initialize -> Initialization failed.");
                return;
            }

            initialized = true;
        }

        private bool initializeButtons()
        {
            if (null == menuTabsButtonsTransform)
            {
                Debug.LogError("MenuNavigator::initializeButtons -> menuTabsButtonsTransform was not assigned.");
                return false;
            }

            menuTabsButtons = menuTabsButtonsTransform.GetComponentsInChildren<MenuTabButton>();

            if (0 == menuTabsButtons.Length)
            {
                Debug.LogError("MenuNavigator::initializeButtons -> No buttons were found.");
                return false;
            }

            return true;
        }

        private bool initializeMenus()
        {
            if (null == menuItems)
            {
                Debug.LogError("MenuNavigator::initializeMenus -> menuItems was not assigned.");
                return false;
            }

            if (0 == menuItems.Length)
            {
                Debug.LogError("MenuNavigator::initializeMenus -> No menus were found.");
                return false;
            }

            return true;
        }

        private bool bindButtonsToMenus()
        {
            int buttonsCount = menuTabsButtons.Length;
            int menusCount = menuItems.Length;

            if (buttonsCount != menusCount)
            {
                Debug.LogWarning("MenuNavigator::bindButtonsToMenus -> buttonsCount and menusCount are not equal. Buttons count: " + buttonsCount + " Menus count: " + menusCount);
            }

            int count = buttonsCount;

            if (menusCount < buttonsCount)
                count = menusCount;

            for (int i = 0; i < count; i++)
            {
                bindButtonToMenu(menuTabsButtons[i], i);
            }

            return true;
        }

        private bool launchDefaultMenu()
        {
            if (null == defaultMenuButton)
            {
                Debug.LogError("MenuNavigator::launchDefaultMenu -> defaultMenuButton was not assigned.");
                return false;
            }

            defaultMenuButton.Select();

            return true;
        }

        private bool bindPlayerInput()
        {
            /*
            if (null == playerInput)
            {
                Debug.LogError("MenuNavigator::bindPlayerInput -> player input was not assigned.");
                return false;
            }
            */

            return true;
        }
        private void bindButtonToMenu(MenuTabButton i_button, int i_menuIndex)
        {
            i_button.Initialize(i_menuIndex);
            i_button.OnMenuButtonSelect += onButtonSelected;
        }

        private void activateMenuElement(int i_menuIndex)
        {
            // The properties that will be passed to each menu's animate functions
            AnimationProperties currentMenuAP = new AnimationProperties();
            AnimationProperties selectedMenuAP = new AnimationProperties();
            if (null != currentActiveMenuIndex)
            {
                if (i_menuIndex == currentActiveMenuIndex)
                    return;

                if (currentActiveMenuIndex > i_menuIndex)
                {
                    // The selected menu is to the left of the current menu
                    // The selected menu will come from the left, the current menu will fade to the right
                    selectedMenuAP.Direction = AnimationDirection.Left;
                    currentMenuAP.Direction = AnimationDirection.Right;
                }
                else
                {
                    // The selected menu is to the right of the current menu
                    // The selected menu will come from the right, the current menu will fade to the left
                    selectedMenuAP.Direction = AnimationDirection.Right;
                    currentMenuAP.Direction = AnimationDirection.Left;
                }
            }
            else
            {
                currentActiveMenuIndex = i_menuIndex;
            }

            // The second condition prevents this block from running the very first time this function is called
            if (IsTransitionPending() && currentActiveMenuIndex != i_menuIndex)
            {
                // A menu transition is still in effect and the user pressed menu buttons in quick succession!
                // Clear the pending animations, then show the newly selected screen
                mainAnimController.ClearQueue();
                mainAnimController.ShowScreen(menuItems[i_menuIndex].AnimationsController, selectedMenuAP);
                currentActiveMenuIndex = i_menuIndex;
                return;
            }

            if (true == menuItems[currentActiveMenuIndex.Value].gameObject.activeInHierarchy)
            {
                if (menuItems[currentActiveMenuIndex.Value].IsAnimated)
                    mainAnimController.HideScreen(menuItems[currentActiveMenuIndex.Value].AnimationsController, currentMenuAP);
                else
                    menuItems[currentActiveMenuIndex.Value].gameObject.SetActive(false);
            }

            if (false == menuItems[i_menuIndex].gameObject.activeInHierarchy)
            {
                if (menuItems[i_menuIndex].IsAnimated)
                {
                    mainAnimController.ShowScreen(menuItems[i_menuIndex].AnimationsController, selectedMenuAP);
                    currentActiveMenuIndex = i_menuIndex;
                }
                else
                    menuItems[i_menuIndex].gameObject.SetActive(true);
                currentActiveMenuIndex = i_menuIndex;
            }
        }

        private void onButtonSelected(int i_menuIndex)
        {
            activateMenuElement(i_menuIndex);
        }

        private void reset()
        {
            resetButtons();
        }

        private void resetButtons()
        {
            if (null == menuTabsButtons) return;

            for (int i = 0; i < menuTabsButtons.Length; i++)
            {
                menuTabsButtons[i].ResetButton();
            }
        }
        /*
        private void processPlayerInput()
        {
            if (false == initialized) return;

            checkIfAnyBtnIsSelected();
        }
        */
        private void checkIfAnyBtnIsSelected()
        {
            GameObject selectedButtonObj = EventSystem.current.currentSelectedGameObject;

            if (null != selectedButtonObj)
            {
                if (null != selectedButtonObj.GetComponent<Button>()) return;
            }
            // TODO: Replace with new input system
            /*
            if (Input.GetAxis("Vertical") > 0.1f ||
                Input.GetAxis("Vertical") < 0.1f ||
                Input.GetAxis("Horizontal") > 0.1f ||
                Input.GetAxis("Horizontal") < 0.1f)
            {
                if (null != currentActiveMenuIndex)
                {

                }
            }
            */
        }

        /// <summary>
        /// Is a screen transition still in effect? Use this to make sure users don't 
        /// Bug out menus by pressing buttons in quick succession
        /// </summary>
        /// <returns>
        /// True: A menu transition is still in effect
        /// </returns>
        private bool IsTransitionPending()
        {
            if (currentActiveMenuIndex != null)
                if (!menuItems[currentActiveMenuIndex.Value].gameObject.activeSelf)
                {
                    return true;
                }
            return false;
        }
        #endregion
    }
}