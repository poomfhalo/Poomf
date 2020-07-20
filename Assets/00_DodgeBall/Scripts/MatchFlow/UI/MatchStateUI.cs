using UnityEngine;
using DG.Tweening;

public class MatchStateUI : MonoBehaviour
{
    [SerializeField] RoundsGroupUI teamA = null;
    [SerializeField] RoundsGroupUI teamB = null;
    [SerializeField] CanvasGroup containerGroup = null;
    [SerializeField] float animatingDur = 0.15f;

    [Header("Read Only")]
    [SerializeField] bool isVisible = true;

    bool isFading = false;

    void Awake()
    {
        teamA.Initialize(MatchState.Instance.TotalRoundsCount, MatchState.Instance.GetTeamAScore);
        teamB.Initialize(MatchState.Instance.TotalRoundsCount, MatchState.Instance.GetTeamBScore);
    }

    public void SwitchVisiblity()
    {
        if(isVisible)
        {
            Hide();
        }
        else
        {
            Show();
        }
    }
    public void Show()
    {
        if (isFading)
            return;
        isFading = true;
        containerGroup.DOFade(1, animatingDur).SetEase(Ease.InOutSine).OnComplete(()=> {
            isFading = false;
            isVisible = true;
        });
    }
    public void Hide()
    {
        if (isFading)
            return;
        isFading = true;
        containerGroup.DOFade(0, animatingDur).SetEase(Ease.InOutSine).OnComplete(()=> {
            isFading = false;
            isVisible = false;
        });
    }
}