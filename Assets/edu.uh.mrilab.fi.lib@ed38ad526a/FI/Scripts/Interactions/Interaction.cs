using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace fi {
    public class Interaction : MonoBehaviour {
        /// <summary>
        /// The ID of the Module that this interaction belongs to.
        /// </summary>
        public string ModuleID { get; set; } = "";

        /// <summary>
        /// The ID of this interaction. Also assigned as the GameObject's name.
        /// </summary>
        string interactionID = "";
        public virtual string InteractionID {
            get {
                return interactionID;
            } set {
                interactionID = value;
                this.gameObject.name = interactionID;
            }
        }

        /// <summary>
        /// The Type this interaction is, should be assigned by derived 
        /// classes.
        /// </summary>
        public EModuleInteraction ActionType { get; protected set; }
    }
}