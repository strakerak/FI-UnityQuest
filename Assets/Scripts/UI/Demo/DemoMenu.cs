using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoMenu : MonoBehaviour
{
    public void onCloseButton()
    {
        /*Destroy(GameObject.Find("DemoController(Clone)").gameObject);

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
        GameObject.Find("ModulesMenuUI").transform.Find("SettingsPanel").gameObject.SetActive(false);
        ScrollPanel.SetActive(false);

        ARSceneHandler.Reset();
        */

        Debug.Log("Cleanup Active Demo");
        Cleanup.cleanupStudy();
        //ARSceneHandler.Reset();
    }
}
