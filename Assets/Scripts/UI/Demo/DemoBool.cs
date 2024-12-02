using UnityEngine;
using UnityEngine.UI;

namespace fi
{
    public class DemoBool : MonoBehaviour
    {
        /// <summary>
        /// Label used to display the Interaction ID.
        /// </summary>
        public TMPro.TextMeshProUGUI InteractionIDLabel;

        /// <summary>
        /// Override setter to set Interaction ID in label when it is set.
        /// </summary>
        public string InteractionID;
        
        /// <summary>
        /// The interactable script used to update the toggle's state programmatically.
        /// </summary>
        private Toggle toggle;

        /// <summary>
        /// Scene Object
        /// </summary>
        GameObject sceneObj;

        /// <summary>
        /// The true value of the interaction. 
        /// When the toggle is being updated, it does not represent the actual value.
        /// </summary>
        public bool InteractionValue;

        void Awake()
        {
            toggle = this.transform.Find("Toggle").GetComponentInChildren<Toggle>();
            InteractionIDLabel = toggle.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            InteractionID = InteractionIDLabel.text;
            toggle.onValueChanged.AddListener(delegate { onValueChanged(toggle.isOn); });
        }

        /// <summary>
        /// Catches the user interacting with the toggle.
        /// </summary>
        void onValueChanged(bool isOn)
        {
            if (isOn == InteractionValue)
            {
                return;
            }
            InteractionValue = isOn;

            //Call Cahnge in Phase Animation
            sceneObj = GameObject.Find("Scene(Clone)");
            if (GameObject.Find("Scene(Clone)").GetComponent<CardiacScene>() != null)
            {
                CardiacScene sceneCardiac = sceneObj.GetComponent<CardiacScene>();

                if (InteractionID == "Animation")
                    sceneCardiac.changeAnimation(InteractionValue);
                else if (InteractionID == "Surface")
                    sceneCardiac.surface(InteractionValue);
            }
        }
    }
}