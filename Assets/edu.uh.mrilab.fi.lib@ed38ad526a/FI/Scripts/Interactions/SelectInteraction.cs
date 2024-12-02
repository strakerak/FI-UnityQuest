using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// TODO: This class is yet to be implemented and tested.

namespace fi {
    public class SelectInteraction : Interaction {
        /// <summary>
        /// The true value of the interaction. 
        /// When the dropdown is being updated, it does not represent the actual value.
        /// </summary>
        int interactionValue;
        public virtual int InteractionValue {
            get {
                return interactionValue;
            } set {
                if (interactionValue == value) {
                    return;
                }

                if (Options.Count == 0) {
                    interactionValue = -1;
                } else if (value < 0 || value >= Options.Count) {
                    interactionValue = 0;
                } else {
                    interactionValue = value;
                }
            }
        }

        /// <summary>
        /// This is the currently selected option text.
        /// </summary>
        public virtual string InteractionOption {
            get {
                if (Options.Count == 0) {
                    return "";
                } else if (InteractionValue < 0 || InteractionValue >= Options.Count) {
                    return "";
                } else {
                    return Options[InteractionValue];
                }
            }
        }

        /// <summary>
        ///  The list of options that this select interaction can take. If the
        ///  current value is out of bounds according to these options, it will
        ///  be set to 0. The getter returns a copy of the list. The setter
        ///  copies the items into the internal list.
        /// </summary>
        private List<string> options = new List<string>();
        public virtual List<string> Options {
            get {
                List<string> copy = new List<string>();
                foreach (string opt in options) {
                    copy.Add(opt);
                }

                return copy;
            } set {
                options.Clear();
                foreach (string opt in value) {
                    options.Add(opt);
                }

                if (options.Count == 0) {
                    InteractionValue = -1;
                } else if (InteractionValue < 0 || interactionValue >= options.Count) {
                    InteractionValue = 0;
                }
            }
        }

        /// <summary>
        /// Initialize the interaction.
        /// </summary>
        protected virtual void Awake() {
            ActionType = EModuleInteraction.SELECT;
        }

        /// <summary>
        /// Selects the value by its option index.
        /// </summary>
        /// <param name="value">The value of the selected option.</param>
        public virtual void changeValue(int value) {
            if (InteractionValue == value) {
                return;
            }
            InteractionValue = value;

            ServerConnection.sendMessage(RequestMaker.makeModuleInteractionRequest(ModuleID, InteractionID, InteractionValue));
        }
    }
}