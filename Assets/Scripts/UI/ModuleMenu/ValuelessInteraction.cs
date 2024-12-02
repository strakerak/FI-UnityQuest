using UnityEngine;
using System;

public class ValuelessInteraction : fi.ValuelessInteraction
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

    public void onClick()
    {
        Debug.Log(string.Format("{0} | UI | Request New Data", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));
        App.LogMessage(string.Format("{0} | UI | Request New Data", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));

        App.LogMessage(string.Format("{0} | Visual Change | Request New Data", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now), Screen.width, Screen.height));
        Debug.Log(string.Format("{0} | Visual Change | Request New Data", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now), Screen.width, Screen.height));
        this.trigger();
    }
}
