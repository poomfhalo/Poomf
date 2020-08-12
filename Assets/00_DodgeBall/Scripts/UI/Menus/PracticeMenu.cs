using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PracticeMenu : MonoBehaviour
{
    public event Action E_OnBack = null;
    [SerializeField] ConfirmationMenu confirmationMenu = null;
    [SerializeField] Button backButton = null;

    void Start()
    {
        backButton.onClick.AddListener(Hide);
    }
    public void Show()
    {
        Action yes = () => {//Yes is Denoated to practice 
            SceneFader.instance.FadeIn(1, () => SceneManager.LoadScene("SP_Room"));
        };
        Action no = () => {//No is Denoated to tutorial
            Debug.LogWarning("Have Not Implemented Yet");
        };

        confirmationMenu.SetYesNoTexts("Practice", "Tutorial");
        confirmationMenu.CallConfirm(yes, no, "Single Player", "Do you want to go to practice, or tutorial?", false, null);

    }
    public void Hide()
    {
        confirmationMenu.Hide();
        E_OnBack?.Invoke();
    }
}