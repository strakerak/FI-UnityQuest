using UnityEngine;
using UnityEngine.UI;

public class Module : fi.Module
{
    // One can modify Scene and Menu positions after they have been created
    // by the base class.
    
    protected override void Awake()
    {
        base.Awake();

        if (ModuleMenu != null)
        {
            //GameObject.Find("ModulesMenuUI").transform.Find("SettingsPanel").gameObject.SetActive(true);
            GameObject ScrollPanel = GameObject.Find("ModulesMenuUI").transform.Find("ScrollPanel").gameObject;
            ScrollPanel.SetActive(true);
            ModuleMenu.transform.SetParent(ScrollPanel.transform);
            GameObject btn = GameObject.Find("ModulesMenuUI").transform.Find("ModuleMenuAnchor").gameObject;
            btn.SetActive(true);
            btn.transform.GetComponentInChildren<Text>().text = "Hide";
            


            ModuleMenu.transform.localPosition = Camera.main.ScreenToWorldPoint(ScrollPanel.transform.localPosition);
            ModuleMenu.GetComponent<RectTransform>().anchoredPosition = Camera.main.ScreenToWorldPoint(ScrollPanel.GetComponent<RectTransform>().anchoredPosition);

            if (GameObject.Find("ModulesListMenu")  != null)
                GameObject.Find("ModulesListMenu").transform.Find("Scroll View(Clone)").gameObject.SetActive(false);
            
            if (GameObject.Find("ConnectionMenu") != null && GameObject.Find("ConnectionMenu").activeSelf)
                GameObject.Find("ConnectionMenu").SetActive(false);
                
            if (GameObject.Find("SettingsMenu") != null && GameObject.Find("SettingsMenu").activeSelf)
                GameObject.Find("SettingsMenu").SetActive(false);

            if (Scene != null)
            {
                //Debug.Log(string.Format("Module::Awake Found Scene {0} {1}", Scene.gameObject.name, Scene.transform.localScale.x));
                Scene.transform.localScale = new Vector3(3F, 3F, 3F);
            }
        }
    }

    protected override void Update()
    {
        base.Update();
    }

    protected void OnDestroy()
    {
        Destroy(ModuleMenu.gameObject);
    }
}
