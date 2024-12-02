using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace fi {
    public class AvailableModuleAction : MonoBehaviour {
        /// <summary>
        /// The name of the module.
        /// </summary>
        public virtual string ModuleName { get; set; }

        /// <summary>
        /// The acronym of the module.
        /// </summary>
        public string ModuleAcronym { get; set; }

        /// <summary>
        /// Triggers the ActivateModule event with its corresponding module
        /// name.
        /// </summary>
        public void activateModule() {
            App.activateModule(ModuleName);
        }
    }
}