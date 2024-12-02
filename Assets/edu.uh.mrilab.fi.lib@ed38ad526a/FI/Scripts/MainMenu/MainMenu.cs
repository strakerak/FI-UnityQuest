using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace fi {
    public class MainMenu : MonoBehaviour {

        /// <summary>
        /// Updates the list of available modules. Override this method
        /// to apply logic of how these actions should be available.
        /// </summary>
        /// <param name="modules">The list of available modules.</param>
        public virtual void updateAvailableModuleList(AvailableModule[] modules) {}

        /// <summary>
        /// Updates the list of active modules. Override this method
        /// to apply logic of how these actions should be available.
        /// </summary>
        /// <param name="modules">The list of active modules.</param>
        public virtual void updateActiveModuleList(ActiveModule[] modules) {}
    }
}