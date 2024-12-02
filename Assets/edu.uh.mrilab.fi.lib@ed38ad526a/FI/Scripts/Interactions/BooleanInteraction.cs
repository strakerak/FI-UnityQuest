using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace fi {
    public class BooleanInteraction : Interaction {
        /// <summary>
        /// The true value of the interaction. 
        /// When the toggle is being updated, it does not represent the actual value.
        /// </summary>
        bool interactionValue = false;
        public virtual bool InteractionValue {
            get {
                return interactionValue;
            } set {
                if (interactionValue == value) {
                    return;
                }
                interactionValue = value;
            }
        }

        /// <summary>
        /// Assign the type this interaction is when the object is created.
        /// </summary>
        protected virtual void Awake() {
            ActionType = EModuleInteraction.BOOL;
        }

        /// <summary>
        /// Changes the value of the interaction, sending a message to the
        /// server to apply it. If the value set is equal to the current value,
        /// nothing happens.
        /// </summary>
        /// <param name="toggleState">The new value.</param>
        public virtual void changeValue(bool isOn) {
            if (isOn == InteractionValue) {
                return;
            }
            InteractionValue = isOn;

            ServerConnection.sendMessage(RequestMaker.makeModuleInteractionRequest(ModuleID, InteractionID, InteractionValue));
        }
    }
}