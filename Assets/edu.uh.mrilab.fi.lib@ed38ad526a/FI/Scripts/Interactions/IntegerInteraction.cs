using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace fi {
    public class IntegerInteraction : Interaction {
        /// <summary>
        /// The true value of the interaction.
        /// When the slider is being updated, it does not represent the true
        /// value.
        /// </summary>
        int interactionValue;
        public virtual int InteractionValue {
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
        /// The minimum allowed value when the constraint is MIN or RANGE.
        /// </summary>
        public virtual int Min { get; set; } = 0;

        /// <summary>
        /// The maximum allowed value when the constraint is MAX or RANGE.
        /// </summary>
        public virtual int Max { get; set; } = 1;

        /// <summary>
        /// Assign the type this interaction is when the object is created.
        /// </summary>
        protected virtual void Awake() {
            ActionType = EModuleInteraction.INTEGER;
        }

        /// <summary>
        /// Changes the value of the interaction, sending a message to the
        /// server to apply it. If the value set is equal to the current value,
        /// nothing happens.
        /// </summary>
        /// <param name="value">The new value.</param>
        public virtual void changeValue(int value) {
            if (InteractionValue == value) {
                return;
            }
            InteractionValue = value;

            ServerConnection.sendMessage(RequestMaker.makeModuleInteractionRequest(ModuleID, InteractionID, InteractionValue));
        }
    }
}