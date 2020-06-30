using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class N_LoginMenuAnims : AUIAnimatedScreen
{
    [Header("Container Anim")]
    [SerializeField] Transform container = null;
    [SerializeField] float containerDur = 0.2f;
    [Header("BG Anim")]
    [SerializeField] Image bg = null;
    [SerializeField] float bgDur = 0.3f;

    public override void Initialize()
    {
        container.localScale = Vector3.zero;
        Color c = bg.color;
        c.a = 0;
        bg.color = c;
    }

    public override IEnumerator AnimateIn(AnimationProperties properties = null)
    {
        container.DOScale(Vector3.one, containerDur);
        bg.DOFade(1, bgDur);
        float longest = Mathf.Max(bgDur, containerDur);
        yield return new WaitForSeconds(longest);
    }
    public override IEnumerator AnimateOut(AnimationProperties properties = null)
    {
        bg.DOFade(0, bgDur);
        bg.transform.DOScale(Vector3.zero, bgDur);
        container.DOScale(Vector3.zero, containerDur);
        float longest = Mathf.Max(bgDur, containerDur);
        yield return new WaitForSeconds(longest);
    }
}