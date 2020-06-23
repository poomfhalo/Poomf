using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Settings that are controlled with sliders, like volume
public class SliderControlledSetting : MonoBehaviour
{
    // The slider thats's used to control this setting
    [SerializeField] private Slider sliderComponent;
    // The text value corresponding to this setting (could be null)
    [SerializeField] private Text textValue;
    // Used when setting the text value, multiplies the text value by a multiplier to display the desired range
    [SerializeField] private float multiplier;

    #region Setters/Getters
    public Slider SliderComponent { get { return sliderComponent; } }
    public Text TextValue { get { return textValue; } }
    public float Multiplier { get { return multiplier; } set { multiplier = value; } }
    #endregion

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
