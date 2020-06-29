using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class ToggleButton : MonoBehaviour
{
    public event Action<ToggleButton> OnSelected;
    public string text => m_text.text;

    [SerializeField] Image selectedImage = null;
    [SerializeField] Image deSelectedImage = null;
    [SerializeField] Button button = null;
    [SerializeField] TextMeshProUGUI m_text = null;
    [SerializeField] string textVal = "";

    [Header("Read Only")]
    public bool IsSelected = false;

    void Start()
    {
        button.onClick.AddListener(Select);

        if (string.IsNullOrEmpty(textVal))
            m_text.text = "DEF";
        else
            m_text.text = textVal.ToUpper();
    }

    public void Select()
    {
        selectedImage.gameObject.SetActive(true);
        deSelectedImage.gameObject.SetActive(false);
        OnSelected?.Invoke(this);
    }
    public void DeSelect()
    {
        selectedImage.gameObject.SetActive(false);
        deSelectedImage.gameObject.SetActive(true);
    }
}