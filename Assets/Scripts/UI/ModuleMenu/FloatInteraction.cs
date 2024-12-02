using UnityEngine.UI;
using UnityEngine;
using System;

public class FloatInteraction : fi.FloatInteraction
{
    /// <summary>
    /// Label used to display the Interaction ID.
    /// </summary>
    public TMPro.TextMeshProUGUI InteractionIDLabel;

    /// <summary>
    /// The label used to display the current slider's value.
    /// </summary>
    TMPro.TextMeshProUGUI InteractionValueLabel;

    /// <summary>
    /// The actual slider object.
    /// </summary>
    Slider slider;

    /// <summary>
    /// Override setter to set Interaction ID in label when it is set.
    /// </summary>
    public override string InteractionID {
        get => base.InteractionID;
        set {
            base.InteractionID = value;

            if (InteractionIDLabel != null) {
                InteractionIDLabel.text = InteractionID;
            }
        }
    }

    /// <summary>
    /// The true value of the interaction.
    /// When the slider is being updated, it does not represent the true value.
    /// </summary>
    public override float InteractionValue
    {
        get {
            return base.InteractionValue;
        } set {
            base.InteractionValue = value;

            slider.value = toSliderRange(InteractionValue);
            InteractionValueLabel.text = InteractionValue.ToString("F2");
        }
    }

    protected override void Awake()
    {
        InteractionIDLabel = this.transform.Find("ID_Label").GetComponent<TMPro.TextMeshProUGUI>();
        slider = this.transform.Find("Slider").GetComponentInChildren<Slider>();
        slider.onValueChanged.AddListener(delegate { onValueUpdated(); });
        InteractionValueLabel = transform.Find("Value_Label").GetComponent<TMPro.TextMeshProUGUI>();
    }

    public void onValueUpdated()
    {
        Debug.Log(string.Format("{0} | UI | Request New Data", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));
        App.LogMessage(string.Format("{0} | UI | Request New Data", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));

        App.LogMessage(string.Format("{0} | Visual Change | Request New Data", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now), Screen.width, Screen.height));
        Debug.Log(string.Format("{0} | Visual Change | Request New Data", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now), Screen.width, Screen.height));

        float sliderValue = fromSliderRange(slider.value);
        this.changeValue(slider.value);
    }


    /// <summary>
    /// Used to compute to slider range from the min-max range set by user.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The value mapped to slider range.</returns>
    float toSliderRange(float value)
    {
        return (value - Min) / (Max - Min);
    }

    /// <summary>
    /// Used to compute from slider range to the min-max range set by the user.
    /// </summary>
    /// <param name="value">The slider value</param>
    /// <returns>The value mapped to min-max range.</returns>
    float fromSliderRange(float value)
    {
        return value * (Max - Min) + Min;
    }

}
