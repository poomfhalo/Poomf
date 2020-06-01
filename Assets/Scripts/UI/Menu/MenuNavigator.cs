using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuNavigator : MonoBehaviour
{
    [SerializeField] private Transform menuTabsButtonsTransform = null;
    [SerializeField] private Transform menusRootTransform = null;
    [SerializeField] private MenuTabButton defaultMenuButton = null;

    private MenuTabButton[] menuTabsButtons = null;
    private GameObject[] menus = null;
    private int? currentActiveMenuIndex = null;

    private void Start()
    {
        initialize();
    }

    private void OnDestroy()
    {
        reset();
    }

    private void initialize()
    {
        if (false == initializeButtons() ||
            false == initializeMenus() ||
            false == bindButtonsToMenus() ||
            false == launchDefaultMenu())
        {
            Debug.LogError("MenuNavigator::initialize -> Initialization failed.");
            return;
        }
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
        if (null == menusRootTransform)
        {
            Debug.LogError("MenuNavigator::initializeMenus -> menusRootTransform was not assigned.");
            return false;
        }

        int menusCount = menusRootTransform.childCount;

        if (0 == menusCount)
        {
            Debug.LogError("MenuNavigator::initializeMenus -> No menus were found.");
            return false;
        }

        menus = new GameObject[menusCount];

        for (int i = 0; i < menusCount; i++)
        {
            menus[i] = menusRootTransform.GetChild(i).gameObject;
        }

        return true;
    }

    private bool bindButtonsToMenus()
    {
        int buttonsCount = menuTabsButtons.Length;
        int menusCount = menus.Length;

        if (buttonsCount != menusCount)
        {
            Debug.LogWarning("MenuNavigator::bindButtonsToMenus -> buttonsCount and menusCount are not equal.");
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

    private void bindButtonToMenu(MenuTabButton i_button, int i_menuIndex)
    {
        i_button.Initialize(i_menuIndex);
        i_button.OnMenuButtonSelect += onButtonSelected;
    }

    private void activateMenuElement(int i_menuIndex)
    {
        if (null != currentActiveMenuIndex)
        {
            if (i_menuIndex == currentActiveMenuIndex)
                return;
        }
        else
        {
            currentActiveMenuIndex = i_menuIndex;
        }

        if (true == menus[currentActiveMenuIndex.Value].activeInHierarchy)
        {
            menus[currentActiveMenuIndex.Value].SetActive(false);
        }

        if (false == menus[i_menuIndex].activeInHierarchy)
        {
            menus[i_menuIndex].SetActive(true);
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
}