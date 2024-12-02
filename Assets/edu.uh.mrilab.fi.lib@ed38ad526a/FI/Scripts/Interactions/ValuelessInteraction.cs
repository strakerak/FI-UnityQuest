using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace fi {
    public class ValuelessInteraction : Interaction {
        /// <summary>
        /// Assign the type this interaction is when the object is created.
        /// </summary>
        protected virtual void Awake() {
            ActionType = EModuleInteraction.VALUELESS;
        }

        /// <summary>
        /// Triggers the interaction, sending a message to the server. 
        /// </summary>
        /// <param name="toggleState">The new value.</param>
        public virtual void trigger() {
            ServerConnection.sendMessage(RequestMaker.makeModuleInteractionRequest(ModuleID, InteractionID, ""));
        }
    }
}