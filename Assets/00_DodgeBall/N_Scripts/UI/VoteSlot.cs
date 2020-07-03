using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VoteSlot : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler,IPointerClickHandler
{
    public Sprite CoverSprite => coverSprite;

    public string SceneName => sceneName;

    public event Action<VoteSlot> E_OnSelected = null;
    public event Action<VoteSlot> E_OnPointerEntered = null;
    [Header("Constants")]
    [SerializeField] Image selectedImage = null;
    [SerializeField] Image deSelectedImage = null;
    [SerializeField] Transform votesHead = null;
    [SerializeField] GameObject votePrefab = null;
    [SerializeField] Image iconImage = null;
    [Header("Variables")]
    [SerializeField] Sprite iconSprite = null;
    [SerializeField] Sprite coverSprite = null;
    [SerializeField] string sceneName = "Stadium";

    void Start()
    {
        iconImage.sprite = iconSprite;
        Transform[] children = new Transform[votesHead.childCount];

        for(int i = 0;i<votesHead.childCount;i++)
        {
            children[i] = votesHead.GetChild(i);
        }
        for(int i = 0;i<children.Length;i++)
        {
            Destroy(children[i].gameObject);
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        E_OnPointerEntered?.Invoke(this);
        transform.DOScale(Vector3.one * 1.2f, 0.1f).SetEase(Ease.InOutSine);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOScale(Vector3.one, 0.1f).SetEase(Ease.InOutSine);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        Select();
        E_OnSelected?.Invoke(this);
    }
    private void Select()
    {
        selectedImage.gameObject.SetActive(true);
        deSelectedImage.gameObject.SetActive(false);
    }
    public void DeSelect()
    {
        selectedImage.gameObject.SetActive(false);
        deSelectedImage.gameObject.SetActive(true);
    }
}