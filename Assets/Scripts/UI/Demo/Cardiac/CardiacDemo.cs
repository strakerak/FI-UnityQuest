using UnityEngine;
using UnityEngine.UI;

namespace fi
{ 
    public class CardiacDemo : MonoBehaviour
    {
        public CardiacScene ScenePrefab;
        public GameObject DemoMenuPrefab;
        public GameObject DemoSliderPrefab;
        public GameObject DemoBoolPrefab;

        GameObject ModulesMenuUI;

        protected CardiacScene sceneObj;
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

            sceneObj = Instantiate<CardiacScene>(ScenePrefab, this.transform);

            //Debug.Log(string.Format("Main::Start:: {0}", sceneObj));

            //sceneObj.gameObject.name = "Scene(Clone)";

            ModulesMenuUI = GameObject.Find("ModulesMenuUI").gameObject;
            //ModulesMenuUI.transform.Find("SettingsPanel").gameObject.SetActive(true);
            ScrollPanel.SetActive(true);
            btn.SetActive(true);
            btn.transform.GetComponentInChildren<Text>().text = "Hide";

            GameObject demoTitle = Instantiate<GameObject>(DemoMenuPrefab, ScrollPanel.transform.Find("Viewport").transform.Find("Content").transform);
            GameObject dataSlider = Instantiate<GameObject>(DemoSliderPrefab, ScrollPanel.transform.Find("Viewport").transform.Find("Content").transform);
            dataSlider.GetComponent<DemoSlider>().InteractionValueLabel.text = "1";
            dataSlider.GetComponent<DemoSlider>().Min = 1;
            dataSlider.GetComponent<DemoSlider>().Max = 10;

            GameObject phaseSlider = Instantiate<GameObject>(DemoSliderPrefab, ScrollPanel.transform.Find("Viewport").transform.Find("Content").transform);
            phaseSlider.GetComponent<DemoSlider>().InteractionValueLabel.text = "1";
            phaseSlider.GetComponent<DemoSlider>().Min = 1;
            phaseSlider.GetComponent<DemoSlider>().Max = 25;
            phaseSlider.GetComponent<DemoSlider>().InteractionIDLabel.text = "Phase";

            
            GameObject animateCB = Instantiate<GameObject>(DemoBoolPrefab, ScrollPanel.transform.Find("Viewport").transform.Find("Content").transform);
            animateCB.GetComponent<DemoBool>().InteractionValue = false;
            animateCB.GetComponentInChildren<Toggle>().isOn = false;
            animateCB.GetComponent<DemoBool>().InteractionIDLabel.text = "Animation";
            animateCB.GetComponent<DemoBool>().InteractionID = "Animation";
            

            GameObject surfaceCB = Instantiate<GameObject>(DemoBoolPrefab, ScrollPanel.transform.Find("Viewport").transform.Find("Content").transform);
            surfaceCB.GetComponent<DemoBool>().InteractionValue = true;
            surfaceCB.GetComponentInChildren<Toggle>().isOn = true;
            surfaceCB.GetComponent<DemoBool>().InteractionIDLabel.text = "Surface";
            surfaceCB.GetComponent<DemoBool>().InteractionID = "Surface";

        }
    }
}
