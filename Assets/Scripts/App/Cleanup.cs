using UnityEngine;
using System;

public class Cleanup : MonoBehaviour
{
    public static void cleanup()
    {
        if (GameObject.Find("ModulesListMenu").transform.Find("Scroll View(Clone)").gameObject != null)
        {
            Debug.Log(string.Format("{0} | Background | Delete Study Menu List", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));
            App.LogMessage(string.Format("{0} | Background | Delete Study Menu List", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));

            Destroy(GameObject.Find("ModulesListMenu").transform.Find("Scroll View(Clone)").gameObject);
        }
    }

    public static void cleanupStudy()
    {
        Debug.Log(string.Format("{0} | Background | Clear Current Study ", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));
        App.LogMessage(string.Format("{0} | Background | Clear Current Study", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));

        Destroy(GameObject.Find("DemoController(Clone)").gameObject);

        //Remove Interactions
        GameObject ScrollPanel = GameObject.Find("ModulesMenuUI").transform.Find("ScrollPanel").gameObject;
        GameObject Content = ScrollPanel.transform.Find("Viewport").transform.Find("Content").gameObject;

        // Remove From ScrollView
        Debug.Log(string.Format("Deleting {0} Interactions", Content.transform.childCount));
        foreach (Transform child in Content.transform)
        {
            Debug.Log("Deleting " + child.name);
            Destroy(child.gameObject);
        }

        //Hide Bottom Panels
        //GameObject.Find("ModulesMenuUI").transform.Find("SettingsPanel").gameObject.SetActive(false);
        ScrollPanel.SetActive(false);
        GameObject.Find("ModulesMenuUI").transform.Find("ModuleMenuAnchor").gameObject.SetActive(false);

        //ARSceneHandler.Reset();
    }
}