using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Settings that are controlled with sliders, like volume
public class SliderControlledSetting : MonoBehaviour
{
    // The slider thats's used to control this setting
    [SerializeField] private Slider sliderComponent = null;
    // The text value corresponding to this setting (could be null)
    [SerializeField] private Text textValue = null;
    // Used when setting the text value, multiplies the text value by a multiplier to display the desired range
    [SerializeField] private float multiplier;


    #region Setters/Getters
    public Slider SliderComponent { get { return sliderComponent; } }
    public Text TextValue { get { return textValue; } }
    public float Multiplier { get { return multiplier; } set { multiplier = value; } }
    #endregion

    public void Initialize(float sliderValue)
    {
        sliderComponent.value = sliderValue;
        textValue.text = (sliderValue * multiplier).ToString();
    }

    // called when the slider value is changed
    public void UpdateTextValue(float sliderValue)
    {
        textValue.text = (sliderValue * multiplier).ToString();
    }

    // Called when we want to set the slider to a certain value. Changes the text value as well
    public void SetValue(float value)
    {
        sliderComponent.value = value;
        UpdateTextValue(value);
    }
}
