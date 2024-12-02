using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;

namespace fi {
    public class Module : MonoBehaviour {
        /// <summary>
        /// The prefab used to dynamically create the menu.
        /// </summary>
        public ModuleMenu ModuleMenuPrefab;

        /// <summary>
        /// The prefab used to dynamically create the Scene.
        /// </summary>
        public Scene ScenePrefab;

        /// <summary>
        /// The ID of the module.
        /// </summary>
        string moduleID;
        public virtual string ModuleID {
            get {
                return moduleID;
            } set {
                moduleID = value;
                this.gameObject.name = moduleID;

                if (ModuleMenu != null) {
                    ModuleMenu.ModuleID = moduleID;
                }
            }
        }

        /// <summary>
        /// Menu used to handle the interactions.
        /// </summary>
        protected ModuleMenu ModuleMenu { get; set; }

        /// <summary>
        /// The scene used to handle the visuals.
        /// </summary>
        protected Scene Scene { get; set; }

        /// <summary>
        /// Holds module responses that need to be processed.
        /// </summary>
        protected ConcurrentQueue<ModuleResponse> ModuleResponses { get; private set; } = new ConcurrentQueue<ModuleResponse>();

        protected virtual void Awake() {
            if (ScenePrefab) {
                Scene = Instantiate<Scene>(ScenePrefab, this.transform);
            } else {
                Debug.LogWarning("Failed to create Scene. The prefab is missing.");
            }

            if (ModuleMenuPrefab) {
                ModuleMenu = Instantiate<ModuleMenu>(ModuleMenuPrefab, this.transform);
            } else {
                Debug.LogWarning("Failed to create Module Menu. The prefab is missing.");
            }
        }

        /// <summary>
        /// Runs on every frame.
        /// </summary>
        protected virtual void Update() {
            if (!ModuleResponses.IsEmpty) {
                ModuleResponse response;
                ModuleResponses.TryDequeue(out response);
                if (response != null) {
                    if (Scene) {
                        Scene.executeVisualUpdates(response.ModuleInfo.Visuals);
                    } else {
                        Debug.LogWarning("Failed to update visuals. No Scene was created.");
                    }

                    if (ModuleMenu != null) {
                        ModuleMenu.executeInteractionUpdates(response.ModuleInfo.ModuleInteractions);
                    } else {
                        Debug.LogWarning("Failed to update interactions. No Module Menu was created.");
                    }
                }
            }
        }

        /// <summary>
        /// Receives a ModuleResponse which will be processed on the next
        /// update call.
        /// </summary>
        /// <param name="response">The response information.</param>
        public virtual void onModuleMessage(ModuleResponse response) {
            if (!response.ModuleInfo.ModuleID.Equals(ModuleID)) {
                return;
            }
            ModuleResponses.Enqueue(response);
        }
    }
}