using UnityEngine.UI;
using UnityEngine;
using System;

public class BooleanInteraction : fi.BooleanInteraction
{
    /// <summary>
    /// Label used to display the Interaction ID.
    /// </summary>
    public TMPro.TextMeshProUGUI InteractionIDLabel;

    /// <summary>
    /// Override setter to set Interaction ID in label when it is set.
    /// </summary>
    public override string InteractionID {
        get {
            return base.InteractionID;
        } set {
            base.InteractionID = value;
            if (InteractionIDLabel != null) {
                InteractionIDLabel.text = value;
            }
        }
    }

    /// <summary>
    /// The interactable script used to update the toggle's state programmatically.
    /// </summary>
    private Toggle toggle;

    /// <summary>
    /// The true value of the interaction. 
    /// When the toggle is being updated, it does not represent the actual value.
    /// </summary>
    public override bool InteractionValue
    {
        get {
            return base.InteractionValue;
        } set {
            base.InteractionValue = value;

            toggle.isOn = InteractionValue;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        
        toggle = this.transform.GetComponentInChildren<Toggle>();
        InteractionIDLabel = toggle.GetComponentInChildren<TMPro.TextMeshProUGUI>();

        toggle.onValueChanged.AddListener(delegate { onValueChanged(); });
    }

    /// <summary>
    /// Catches the user interacting with the toggle.
    /// </summary>
    void onValueChanged()
    {
        Debug.Log(string.Format("{0} | UI | Request New Data", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));
        App.LogMessage(string.Format("{0} | UI | Request New Data", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));

        App.LogMessage(string.Format("{0} | Visual Change | Request New Data", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now), Screen.width, Screen.height));
        Debug.Log(string.Format("{0} | Visual Change | Request New Data", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now), Screen.width, Screen.height));

        this.changeValue(toggle.isOn);
    }
}
