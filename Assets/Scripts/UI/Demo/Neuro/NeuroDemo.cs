using UnityEngine;
using UnityEngine.UI;

namespace fi
{
    public class NeuroDemo : MonoBehaviour
    {
        public NeuroScene ScenePrefab;
        public GameObject DemoMenuPrefab;
        public GameObject DemoSliderPrefab;

        GameObject ModulesMenuUI;

        protected NeuroScene sceneObj;
        // Start is called before the first frame update
        void Start()
        {
            ModulesMenuUI = GameObject.Find("ModulesMenuUI").gameObject;

            GameObject ScrollPanel = GameObject.Find("ModulesMenuUI").transform.Find("ScrollPanel").gameObject;
            GameObject btn = ModulesMenuUI.transform.Find("ModuleMenuAnchor").gameObject;

            if (GameObject.Find("Scene(Clone)") != null)
            {
                GameObject study = GameObject.Find("Scene(Clone)").transform.parent.gameObject;
                //ClearAll Active Scenes
                Debug.Log(string.Format("Found Active Study: {0}", study.name));
                if (study.name == "DemoController(Clone)")
                {
                    Debug.Log("Cleanup Active Demo");
                    Cleanup.cleanupStudy();
                }
                else
                {
                    //Cleanup online study
                    Debug.Log(string.Format("Cleanup Online Study: {0}", study.name));

                    //Remove Interactions
                    GameObject Content = ScrollPanel.transform.Find("Viewport").transform.Find("Content").gameObject;

                    // Remove From ScrollView
                    Debug.Log(string.Format("Deleting {0} Interactions", Content.transform.childCount));
                    foreach (Transform child in Content.transform)
                    {
                        Debug.Log("Deleting " + child.name);
                        Destroy(child.gameObject);
                    }

                    //Hide Bottom Panels
                    //ScrollPanel.transform.parent.transform.Find("SettingsPanel").gameObject.SetActive(false);
                    ScrollPanel.SetActive(false);
                    btn.SetActive(false);

                    App.unsubscribeToModule(study.GetComponentInChildren<Module>().ModuleID);
                }

                //ARSceneHandler.Reset();
            }

            ARSceneHandler.Reset();

            sceneObj = Instantiate<NeuroScene>(ScenePrefab, this.transform);

            Debug.Log(string.Format("Main::Start:: {0}", sceneObj));

            //sceneObj.gameObject.name = "Scene(Clone)";

            ModulesMenuUI = GameObject.Find("ModulesMenuUI").gameObject;
            //ModulesMenuUI.transform.Find("SettingsPanel").gameObject.SetActive(true);
          
            ScrollPanel.SetActive(true);
            btn.SetActive(true);
            btn.transform.GetComponentInChildren<Text>().text = "Hide";

            GameObject demoTitle = Instantiate<GameObject>(DemoMenuPrefab, ScrollPanel.transform.Find("Viewport").transform.Find("Content").transform);
            GameObject demoSlider = Instantiate<GameObject>(DemoSliderPrefab, ScrollPanel.transform.Find("Viewport").transform.Find("Content").transform);
            demoSlider.GetComponent<DemoSlider>().Min = 1;
            demoSlider.GetComponent<DemoSlider>().Max = 166;
            demoSlider.GetComponent<DemoSlider>().InteractionValueLabel.text = "1";
        }
    }
}