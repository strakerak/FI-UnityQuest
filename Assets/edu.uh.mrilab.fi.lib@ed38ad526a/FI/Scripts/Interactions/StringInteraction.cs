using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace fi {
    public class StringInteraction : Interaction {
        /// <summary>
        /// The true value of the interaction. 
        /// When the input is being updated, it does not represent the actual
        /// value.
        /// </summary>
        string interactionValue = "";
        public virtual string InteractionValue {
            get {
                return interactionValue;
            } set {
                if (interactionValue.Equals(value)) {
                    return;
                }

                interactionValue = value;
            }
        }

        /// <summary>
        /// Assign the type this interaction is when the object is created.
        /// </summary>
        protected virtual void Awake() {
            ActionType = EModuleInteraction.STRING;
        }

        /// <summary>
        /// Changes the value of the interaction, sending a message to the
        /// server to apply it. If the value set is equal to the current value,
        /// nothing happens.
        /// </summary>
        /// <param name="value">The new value.</param>
        public virtual void changeValue(string value) {
            if (InteractionValue == value) {
                return;
            }
            InteractionValue = value;

            ServerConnection.sendMessage(RequestMaker.makeModuleInteractionRequest(ModuleID, InteractionID, InteractionValue));
        }
    }
}