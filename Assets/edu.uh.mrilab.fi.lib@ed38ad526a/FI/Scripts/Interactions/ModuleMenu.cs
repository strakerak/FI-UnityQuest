using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace fi {
    public class ModuleMenu : MonoBehaviour {
        /// <summary>
        /// The prefab used to dynamically create the valueless interaction.
        /// </summary>
        public ValuelessInteraction ValuelessInteractionPrefab;

        /// <summary>
        /// The prefab used to dynamically create the bool interaction.
        /// </summary>
        public BooleanInteraction BoolInteractionPrefab;

        /// <summary>
        /// The prefab used to dynamically create the integer interaction.
        /// </summary>
        public IntegerInteraction IntegerInteractionPrefab;

        /// <summary>
        /// The prefab used to dynamically create the float interaction.
        /// </summary>
        public FloatInteraction FloatInteractionPrefab;

        /// <summary>
        /// The prefab used to dynamically create the string interaction.
        /// </summary>
        public StringInteraction StringInteractionPrefab;

        /// <summary>
        /// The prefab used to dynamically create the select interaction.
        /// </summary>
        public SelectInteraction SelectInteractionPrefab;

        /// <summary>
        /// The ID of the module this ModuleMenu is being used with.
        /// </summary>
        public virtual string ModuleID { get; set; }

        /// <summary>
        /// Data structure for the module interactions.
        /// </summary>
        protected Dictionary<string, Interaction> ModuleInteractions { get; private set; } = new Dictionary<string, Interaction>();

        /// <summary>
        /// Applies the given interaction updates.
        /// </summary>
        /// <param name="interactions">The interaction updates.</param>
        virtual public void executeInteractionUpdates(ModuleInteraction[] interactions) {
            Debug.Log(string.Format("Executing {0} module interaction updates", interactions.Length));
            bool adjustPositions = false;
            foreach (ModuleInteraction interaction in interactions) {
                interaction.ResponseIDInt = interaction.InteractionJson["ResponseID"].AsInt;

                switch (interaction.ResponseID) {
                    case EModuleResponse.ADD_MODULE_INTERACTION:
                        this.executeAddInteraction(interaction);
                        adjustPositions = true;
                        break;
                    case EModuleResponse.UPDATE_MODULE_INTERACTION:
                        this.executeUpdateInteraction(interaction);
                        break;
                    case EModuleResponse.REMOVE_MODULE_INTERACTION:
                        this.executeRemoveInteraction(interaction);
                        adjustPositions = true;
                        break;
                }
            }

            if (adjustPositions) {
                this.arrangeInteractions();
            }
        }

        virtual protected void executeAddInteraction(ModuleInteraction interaction) {
            interaction.ID = interaction.InteractionJson["ID"];
            interaction.TypeInt = interaction.InteractionJson["Type"].AsInt;
            interaction.Value = interaction.InteractionJson["Value"];
            interaction.ConstraintInt = interaction.InteractionJson["Constraint"].AsInt;
            Debug.Log(string.Format("Adding [{0}] interaction [{1}] constrained by [{2}] with value [{3}]",
                interaction.Type, interaction.ID, interaction.Constraint, interaction.Value.ToString()));

            switch (interaction.Type) {
                case EModuleInteraction.VALUELESS:
                    if (ValuelessInteractionPrefab == null) {
                        Debug.LogWarning(string.Format("Failed to add interaction [{0}] because no Valueless interaction prefab was assigned.", interaction.ID));
                        break;
                    }

                    ValuelessInteraction valuelessInteraction = Instantiate(ValuelessInteractionPrefab, this.transform);
                    valuelessInteraction.InteractionID = interaction.ID;
                    valuelessInteraction.ModuleID = ModuleID;
                    ModuleInteractions.Add(valuelessInteraction.InteractionID, valuelessInteraction);
                    break;
                case EModuleInteraction.BOOL:
                    if (BoolInteractionPrefab == null) {
                        Debug.LogWarning(string.Format("Failed to add interaction [{0}] because no Bool interaction prefab was assigned.", interaction.ID));
                        break;
                    }

                    BooleanInteraction booleanInteraction = Instantiate(BoolInteractionPrefab, this.transform);
                    booleanInteraction.InteractionID = interaction.ID;
                    booleanInteraction.ModuleID = ModuleID;
                    booleanInteraction.InteractionValue = interaction.Value.AsBool;
                    ModuleInteractions.Add(booleanInteraction.InteractionID, booleanInteraction);
                    break;
                case EModuleInteraction.INTEGER:
                    if (interaction.Constraint == EModuleInteractionConstraint.RANGE) {
                        if (IntegerInteractionPrefab == null) {
                            Debug.LogWarning(string.Format("Failed to add interaction [{0}] because no Integer interaction prefab was assigned.", interaction.ID));
                            break;
                        }

                        interaction.MinValue = interaction.InteractionJson["MinValue"];
                        interaction.MaxValue = interaction.InteractionJson["MaxValue"];

                        IntegerInteraction integerInteraction = Instantiate(IntegerInteractionPrefab, this.transform);
                        integerInteraction.ModuleID = ModuleID;
                        integerInteraction.InteractionID = interaction.ID;
                        integerInteraction.Min = interaction.MinValue.AsInt;
                        integerInteraction.Max = interaction.MaxValue.AsInt;
                        integerInteraction.InteractionValue = interaction.Value.AsInt;
                        ModuleInteractions.Add(integerInteraction.InteractionID, integerInteraction);
                    } else {
                        // TOOD: Do other constraints
                        Debug.LogWarning(string.Format("Failed to add interaction [{0}] with constraint [{1}] because this feature is under development.", interaction.ID, interaction.Constraint));
                    }
                    break;
                case EModuleInteraction.FLOAT:
                    if (interaction.Constraint == EModuleInteractionConstraint.RANGE) {
                        if (FloatInteractionPrefab == null) {
                            Debug.LogWarning(string.Format("Failed to add interaction [{0}] because no Float interaction prefab was assigned.", interaction.ID));
                            break;
                        }

                        interaction.MinValue = interaction.InteractionJson["MinValue"];
                        interaction.MaxValue = interaction.InteractionJson["MaxValue"];

                        FloatInteraction floatInteraction = Instantiate(FloatInteractionPrefab, this.transform);
                        floatInteraction.InteractionID = interaction.ID;
                        floatInteraction.ModuleID = ModuleID;
                        floatInteraction.Min = interaction.MinValue.AsFloat;
                        floatInteraction.Max = interaction.MaxValue.AsFloat;
                        floatInteraction.InteractionValue = interaction.Value.AsFloat;
                        ModuleInteractions.Add(floatInteraction.InteractionID, floatInteraction);
                    } else {
                        // TOOD: Do other constraints
                        Debug.LogWarning(string.Format("Failed to add interaction [{0}] with constraint [{1}] because this feature is under development.", interaction.ID, interaction.Constraint));
                    }
                    break;
                case EModuleInteraction.STRING:
                    if (StringInteractionPrefab == null) {
                        Debug.LogWarning(string.Format("Failed to add interaction [{0}] because no String interaction prefab was assigned.", interaction.ID));
                        break;
                    }

                    StringInteraction stringInteraction = Instantiate(StringInteractionPrefab, this.transform);
                    stringInteraction.InteractionID = interaction.ID;
                    stringInteraction.ModuleID = ModuleID;
                    stringInteraction.InteractionValue = interaction.Value;
                    ModuleInteractions.Add(stringInteraction.InteractionID, stringInteraction);

                    break;
                case EModuleInteraction.SELECT:
                    if (SelectInteractionPrefab == null) {
                        Debug.LogWarning(string.Format("Failed to add interaction [{0}] because no Select interaction prefab was assigned.", interaction.ID));
                        break;
                    }

                    interaction.ValueList = interaction.InteractionJson["ValueOptions"].AsArray;

                    SelectInteraction selectInteraction = Instantiate(SelectInteractionPrefab, this.transform);
                    selectInteraction.InteractionID = interaction.ID;
                    selectInteraction.ModuleID = ModuleID;

                    List<string> options = new List<string>();
                    for (int i = 0; i < interaction.ValueList.Count; i++) {
                        options.Add(interaction.ValueList[i]);
                    }
                    selectInteraction.Options = options;
                    selectInteraction.InteractionValue = interaction.Value.AsInt;
                    
                    ModuleInteractions.Add(selectInteraction.InteractionID, selectInteraction);

                    break;
                default:
                    break;
            }
        }

        virtual protected void executeUpdateInteraction(ModuleInteraction interactionInfo) {
            interactionInfo.ID = interactionInfo.InteractionJson["ID"];
            interactionInfo.TypeInt = interactionInfo.InteractionJson["Type"].AsInt;
            interactionInfo.Value = interactionInfo.InteractionJson["Value"];
            interactionInfo.ConstraintInt = interactionInfo.InteractionJson["Constraint"].AsInt;

            Debug.Log(string.Format("Updating Interaction [{0}] with value [{1}]", interactionInfo.ID, interactionInfo.Value));

            Interaction interaction = ModuleInteractions[interactionInfo.ID];
            if (interaction.ActionType == EModuleInteraction.BOOL) {
                BooleanInteraction toggle = interaction as BooleanInteraction;
                if (toggle != null) {
                    toggle.InteractionValue = interactionInfo.Value.AsBool;
                }
            } else if (interaction.ActionType == EModuleInteraction.INTEGER) {
                IntegerInteraction slider = interaction as IntegerInteraction;
                if (slider != null) {
                    interactionInfo.UpdateConstraint = interactionInfo.InteractionJson["UpdateConstraint"].AsBool;
                    if (interactionInfo.UpdateConstraint) {
                        interactionInfo.MinValue = interactionInfo.InteractionJson["MinValue"];
                        interactionInfo.MaxValue = interactionInfo.InteractionJson["MaxValue"];

                        slider.Min = interactionInfo.MinValue.AsInt;
                        slider.Max = interactionInfo.MaxValue.AsInt;
                    }
                    slider.InteractionValue = interactionInfo.Value.AsInt;
                }
            } else if (interaction.ActionType == EModuleInteraction.FLOAT) {
                FloatInteraction slider = interaction as FloatInteraction;
                if (slider != null) {
                    interactionInfo.UpdateConstraint = interactionInfo.InteractionJson["UpdateConstraint"].AsBool;
                    if (interactionInfo.UpdateConstraint) {
                        interactionInfo.MinValue = interactionInfo.InteractionJson["MinValue"];
                        interactionInfo.MaxValue = interactionInfo.InteractionJson["MaxValue"];


                        slider.Min = interactionInfo.MinValue.AsFloat;
                        slider.Max = interactionInfo.MaxValue.AsFloat;
                    }
                    slider.InteractionValue = interactionInfo.Value.AsFloat;
                }
            } else if (interaction.ActionType == EModuleInteraction.STRING) {
                StringInteraction textField = interaction as StringInteraction;
                if (textField != null) {
                    textField.InteractionValue = interactionInfo.Value;
                }
            } else if (interaction.ActionType == EModuleInteraction.SELECT) {
                SelectInteraction comboBox = interaction as SelectInteraction;
                if (comboBox != null) {

                    interactionInfo.ValueList = interactionInfo.InteractionJson["ValueOptions"].AsArray;

                    List<string> options = new List<string>();
                    for (int i = 0; i < interactionInfo.ValueList.Count; i++) {
                        options.Add(interactionInfo.ValueList[i]);
                    }
                    comboBox.Options = options;
                    comboBox.InteractionValue = interactionInfo.Value.AsInt;
                }
            } else {
                Debug.LogWarning(string.Format("Failed to update interaction [{0}] because its type is unknown", interactionInfo.ID));
            }
        }


        virtual protected void executeRemoveInteraction(ModuleInteraction interactionInfo) {
            interactionInfo.ID = interactionInfo.InteractionJson["ID"];
            Interaction interaction = ModuleInteractions[interactionInfo.ID];
            ModuleInteractions.Remove(interactionInfo.ID);

            Destroy(interaction);
            // TODO: This function needs to be tested.
        }

        /// <summary>
        /// This function is called when interactions are added or removed.
        /// Override this function in derived classes to provide logic
        /// in how these should be arranged in their corresponding menu.
        /// </summary>
        virtual protected void arrangeInteractions() { }
    }
}