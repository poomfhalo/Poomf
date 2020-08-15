using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening;

public class ConfirmationMenu : MonoBehaviour
{
    public RectTransform noPos => no.GetComponent<RectTransform>();
    public RectTransform yesPos => yes.GetComponent<RectTransform>();
    [SerializeField] Button yes = null, no = null;
    [SerializeField] TextMeshProUGUI title = null, details = null;
    [SerializeField] bool autoRefreshNegihbours = true;

    Action yesCall = null, noCall = null;
    bool lastInputState = false;
    Action lastOnIsIn = null;

    void Awake()
    {
        yes.onClick.AddListener(OnYes);
        no.onClick.AddListener(OnNo);

        SnapHide();
    }
    public void CallConfirm(Action onYes, Action onNo, string title, string details, bool startHandsOnYes, Action onIsIn)
    {
        lastOnIsIn = onIsIn;

        Show();
        yesCall = onYes;
        noCall = onNo;

        if (this.title)
            this.title.text = title;
        if (this.details)
            this.details.text = details;

        RectTransform t = null;
        if (startHandsOnYes)
            t = yesPos;
        else
            t = noPos;

        //Set up the currently selected keyboard input for this menu here?
    }
    public void Hide()
    {
        transform.localScale = new Vector3(1, 1, 1);

        transform.GetComponent<CanvasGroup>().DOFade(0, 0.45f).SetEase(Ease.InOutSine);

        transform.DOScale(new Vector3(1, 0.2f, 1), 0.1f).SetEase(Ease.InOutSine).OnComplete(() => {
            transform.DOScale(new Vector3(0.2f, 0.2f, 1), 0.2f).SetEase(Ease.InOutSine).OnComplete(()=> {
                transform.DOScale(new Vector3(0, 0, 1), 0.1f).SetEase(Ease.InOutSine).OnComplete(() => {
                    //On Hide Finished, perhaps we can play SFX here?
                });
            });
        });
    }
    public void SnapHide()
    {
        transform.localScale = Vector3.zero;
        transform.GetComponent<CanvasGroup>().alpha = 0;
    }
    public void SnapShow()
    {
        transform.localScale = Vector3.one;
        transform.GetComponent<CanvasGroup>().alpha = 1;
    }
    public void SetYesNoTexts(string yesText, string noText)
    {
        yes.GetComponentInChildren<TextMeshProUGUI>().text = yesText;
        no.GetComponentInChildren<TextMeshProUGUI>().text = noText;
    }

    private void Show()
    {
        SnapHide();

        transform.GetComponent<CanvasGroup>().DOFade(1, 0.45f).SetEase(Ease.InOutSine);

        transform.DOScale(new Vector3(0.2f, 0.2f, 1), 0.1f).SetEase(Ease.InOutSine).OnComplete(() => {
            transform.DOScale(new Vector3(1, 0.2f, 1), 0.2f).SetEase(Ease.InOutSine).OnComplete(() => {
                transform.DOScale(new Vector3(1, 1, 1), 0.1f).SetEase(Ease.InOutSine).OnComplete(() => {
                    if(autoRefreshNegihbours)
                    {
                        //if keyboard initialization, is dependentation, do something about it here?
                    }
                    lastOnIsIn?.Invoke();
                });
            });
        });

        //if we have 
    }
    private void OnYes()
    {
        //When yes is clicked, do stuff, play SFX?
        yesCall?.Invoke();
    }
    private void OnNo()
    {
        //the moment No is clicked, play SFX?
        noCall?.Invoke();
    }
}