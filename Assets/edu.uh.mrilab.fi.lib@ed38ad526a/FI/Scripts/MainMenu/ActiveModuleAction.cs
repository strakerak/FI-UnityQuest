using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace fi {
    public class ActiveModuleAction : MonoBehaviour {

        /// <summary>
        /// The ID of the module.
        /// </summary>
        public virtual string ModuleID { get; set; }

        /// <summary>
        /// The name of the module.
        /// </summary>
        public virtual string ModuleName { get; set; }

        /// <summary>
        /// The acronym of the module.
        /// </summary>
        public virtual string ModuleAcronym { get; set; }

        /// <summary>
        /// Triggers the SubscribeToModule event with its corresponding module
        /// ID.
        /// </summary>
        public void onSubscribeToModuleClick() {
            App.subscribeToModule(ModuleID);
        }

        /// <summary>
        /// Triggers the UnsubscribeToModule event with its corresponding
        /// module ID.
        /// </summary>
        public void onUnsubscribeFromModuleClick() {
            App.unsubscribeToModule(ModuleID);
        }

        /// <summary>
        /// Triggers the StopModule event with its corresponding
        /// module ID.
        /// </summary>
        public void onStopModuleClick() {
            App.stopModule(ModuleID);
        }
    }
}