using UnityEngine;
using System;

public class ModuleMenu : fi.ModuleMenu
{
    /// <summary>
    /// Label used to show the Module ID.
    /// </summary>
    TMPro.TextMeshProUGUI ModuleMenuLabel;

    /// <summary>
    /// Override the setter to change the Module ID's shown in the
    /// menu's title.
    /// </summary>
    public override string ModuleID
    {
        get
        {
            return base.ModuleID;
        }
        set
        {
            base.ModuleID = value;

            if (ModuleMenuLabel != null)
            {
                ModuleMenuLabel.text = string.Format("{0} Menu", ModuleID);
            }
        }
    }

    /// <summary>
    /// Lets the App know that this module should be unsubscribed from.
    /// </summary>
    public void unsubscribe()
    {

        Debug.Log(string.Format("{0} | UI | Unsubscribing to Module", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));
        App.LogMessage(string.Format("{0} | UI | Unsubscribing to Module", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));

        //Remove Interactions
        GameObject ScrollPanel = GameObject.Find("ModulesMenuUI").transform.Find("ScrollPanel").gameObject;
        GameObject Content = ScrollPanel.transform.Find("Viewport").transform.Find("Content").gameObject;

        // Remove From ScrollView
        Debug.Log(string.Format("{0} | Background | Deleting {1} Interactions", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now), Content.transform.childCount));
        App.LogMessage(string.Format("{0} | Background | Deleting Interactions", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));

        foreach (Transform child in Content.transform)
        {
            Debug.Log("Deleting " + child.name);
            Destroy(child.gameObject);
        }

        //Hide Bottom Panels
        //GameObject.Find("ModulesMenuUI").transform.Find("SettingsPanel").gameObject.SetActive(false);
        ScrollPanel.SetActive(false);
        GameObject.Find("ModulesMenuUI").transform.Find("ModuleMenuAnchor").gameObject.SetActive(false);

        Debug.Log(string.Format("{0} | To Server | Unsubscribing to Module", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));
        App.LogMessage(string.Format("{0} | To Server | Unsubscribing to Module", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));

        // Call UnsubscribeToModule
        App.unsubscribeToModule(ModuleID);
        ARSceneHandler.Reset();
    }

    override protected void arrangeInteractions()
    {
        GameObject Content = GameObject.Find("ModulesMenuUI").transform.Find("ScrollPanel").transform.Find("Viewport").transform.Find("Content").gameObject;

        Debug.Log("Arranging Interactions");
        // Add To ScrollView
        int childcnt = this.transform.childCount;
        for (int i = 0; i < childcnt; i++)
        {
            Transform child = this.transform.GetChild(0);
            child.SetParent(Content.transform);
        }
    }
}