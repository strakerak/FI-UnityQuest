using UnityEngine;
using UnityEngine.UI;

namespace fi {

    public class DemoSlider : MonoBehaviour
    {
        /// <summary>
        /// Label used to display the Interaction ID.
        /// </summary>
        public TMPro.TextMeshProUGUI InteractionIDLabel;

        /// <summary>
        /// The label used to display the current slider's value.
        /// </summary>
        public TMPro.TextMeshProUGUI InteractionValueLabel;

        /// <summary>
        /// The actual slider object.
        /// </summary>
        Slider slider;

        /// <summary>
        /// Scene Object
        /// </summary>
        GameObject sceneObj;

        /// <summary>
        /// The true value of the interaction.
        /// When the slider is being updated, it does not represent the true value.
        /// </summary>

        public int Min { get; set; }
        public int Max { get; set; }

        int interactionValue;
        public virtual int InteractionValue
        {
            get
            {
                return interactionValue;
            }
            set
            {
                if (interactionValue == value)
                {
                    return;
                }
                interactionValue = value;
            }
        }

        /// <summary>
        /// Initialize the interaction.
        /// </summary>
        void Awake()
        {
            InteractionIDLabel = this.transform.Find("ID_Label").GetComponent<TMPro.TextMeshProUGUI>();
            slider = this.transform.Find("Slider").GetComponentInChildren<Slider>();
            slider.onValueChanged.AddListener(delegate { onValueUpdated(); });
            InteractionValueLabel = transform.Find("Value_Label").GetComponent<TMPro.TextMeshProUGUI>();
            InteractionValueLabel.text = slider.value.ToString();
        }

        public void onValueUpdated()
        {
            int sliderValue = fromSliderRange(slider.value);

            InteractionValueLabel.text = sliderValue.ToString();

            if (InteractionValue == sliderValue)
            {
                return;
            }
            InteractionValue = sliderValue;

            sceneObj = GameObject.Find("Scene(Clone)");
            if (GameObject.Find("Scene(Clone)").GetComponent<NeuroScene>() != null)
            {
                NeuroScene sceneNeuro = sceneObj.GetComponent<NeuroScene>();
                sceneNeuro.dataChange(sliderValue);
            }
            else
            {
                CardiacScene sceneCardiac = sceneObj.GetComponent<CardiacScene>();

                if (InteractionIDLabel.text == "Slice")
                    sceneCardiac.dataChange(sliderValue);
                else if (InteractionIDLabel.text == "Phase")
                    sceneCardiac.phaseChange(sliderValue);
            }
        }

        /// <summary>
        /// Used to compute from slider range to the min-max range set by the user.
        /// </summary>
        /// <param name="value">The slider value</param>
        /// <returns>The value mapped to min-max range.</returns>
        int fromSliderRange(float value)
        {
            return (int)Mathf.Round(value * (Max - Min) + Min);
        }
    }
}