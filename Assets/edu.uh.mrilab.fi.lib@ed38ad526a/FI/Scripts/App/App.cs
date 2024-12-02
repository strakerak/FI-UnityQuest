using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

using SimpleJSON;
using System.Dynamic;

namespace fi {
    public class App : MonoBehaviour {
        /// <summary>
        /// Represents the singleton of this class.
        /// </summary>
        static App INSTANCE;

        /// <summary>
        /// Toggles logging in the application.
        /// </summary>
        public bool EnableLogging = true;

        /// <summary>
        /// The message dispatcher used to dispatch server messages.
        /// </summary>
        MessageDispatcher MessageDispatcher;

        /// <summary>
        /// The prefab of the main menu that will be created and used.
        /// </summary>
        public MainMenu MainMenuPrefab;

        /// <summary>
        /// The main menu where the lists of available and active modules are shown.
        /// </summary>
        protected MainMenu MainMenu { get; set; }

        /// <summary>
        /// The prefab used to create when a new Module has been subscribed to.
        /// </summary>
        public Module ModulePrefab;

        /// <summary>
        /// The Modules that the FI is currently subscribed to.
        /// </summary>
        protected Dictionary<string, Module> SubscribedModules = new Dictionary<string, Module>();

        /// <summary>
        /// Used to store application type responses.
        /// </summary>
        ConcurrentQueue<ApplicationResponse> ApplicationResponses = new ConcurrentQueue<ApplicationResponse>();

        /// <summary>
        /// Used to store messages to log in the server.
        /// </summary>
        ConcurrentQueue<string> ServerLogs = new ConcurrentQueue<string>();

        /// <summary>
        /// There can only be a single instance of AppController. This creates the singleton.
        /// </summary>
        private void Awake() {
            if (INSTANCE != null) {
                Destroy(gameObject);
            } else {
                INSTANCE = this;
                GameObject.DontDestroyOnLoad(gameObject);
                Debug.unityLogger.logEnabled = EnableLogging;
                Debug.Log(Application.persistentDataPath);
            }
        }

        /// <summary>
        /// Sets the application to run on background and sets-up the menus.
        /// </summary>
        protected virtual void Start() {
            // Setup initial flags
            Application.runInBackground = true;

            if (MainMenuPrefab != null) {
                MainMenu = Instantiate<MainMenu>(MainMenuPrefab);
            } else {
                Debug.LogWarning("Failed to create Main Menu. No prefab was given.");
            }

            MessageDispatcher = this.GetComponent<MessageDispatcher>();
            if (MessageDispatcher == null) {
                Debug.LogError("The Message Dispatcher was not found.");
            }
        }

        /// <summary>
        /// Updates the menus when new information was received. 
        /// It also passes the next message that has been received to the appropriate object.
        /// </summary>
        protected virtual void Update() {
            if (!ApplicationResponses.IsEmpty) {
                ApplicationResponse response;
                ApplicationResponses.TryDequeue(out response);
                if (response != null) {
                    MainMenu.updateAvailableModuleList(response.AvailableModules);
                    MainMenu.updateActiveModuleList(response.ActiveModules);
                }
            }

            while (!ServerLogs.IsEmpty && ServerConnection.IsAuthenticated) {
                string log;
                ServerLogs.TryDequeue(out log);
                if (log != null) {
                    ServerConnection.sendMessage(RequestMaker.makeServerLogRequest(log));
                }
            }
        }

        /// <summary>
        /// Adds the given log to the queue.
        /// </summary>
        /// <param name="log">The message.</param>
        static public void serverLog(string log) {
            INSTANCE.ServerLogs.Enqueue(log);
        }

        /// <summary>
        /// When a new application response is received, enqueue it to be executed.
        /// </summary>
        /// <param name="applicationResponse">The application response.</param>
        public void onNewApplicationResponse(ApplicationResponse applicationResponse) {
            ApplicationResponses.Enqueue(applicationResponse);
        }

        /// <summary>
        /// Sends a request to activate a module with the given name.
        /// </summary>
        /// <param name="moduleID"></param>
        public static void activateModule(string moduleName) {
            ServerConnection.sendMessage(RequestMaker.makeApplicationRequest(EApplicationRequest.START_MODULE, moduleName, ""));
        }

        /// <summary>
        /// Sends a request to stop the module.
        /// </summary>
        /// <param name="moduleID"></param>
        public static void stopModule(string moduleID) {
            ServerConnection.sendMessage(RequestMaker.makeApplicationRequest(EApplicationRequest.STOP_MODULE, "", moduleID));
        }

        /// <summary>
        /// Handles the user wanting to subscribe a module. It creates the
        /// Module instance, where the visuals and interactions are created.
        /// </summary>
        /// <param name="moduleID"></param>
        public static void subscribeToModule(string moduleID) {
            if (INSTANCE.createModule(moduleID)) {
                ServerConnection.sendMessage(RequestMaker.makeModuleSubscriptionRequest(moduleID));
            }
        }

        /// <summary>
        /// Creates a module and its objects.
        /// </summary>
        /// <param name="moduleID">The ID of the module to create.</param>
        /// <returns>Whether the module was created successfully.</returns>
        protected virtual bool createModule(string moduleID) {
            if (SubscribedModules.ContainsKey(moduleID)) {
                Debug.LogWarning("Failed to add module, a module with that ID already exist");
                return false;
            }

            Module module = Instantiate<Module>(INSTANCE.ModulePrefab);
            module.ModuleID = moduleID;
            SubscribedModules.Add(moduleID, module);

            MessageDispatcher.NewModuleResponse.AddListener(module.onModuleMessage);

            return true;
        }

        /// <summary>
        /// Sends a request to unsubscribe from the module.
        /// </summary>
        /// <param name="moduleID"></param>
        public static void unsubscribeToModule(string moduleID) {
            if (INSTANCE.deleteModule(moduleID)) {
               ServerConnection.sendMessage(RequestMaker.makeModuleUnsubscriptionRequest(moduleID));
            }
        }

        /// <summary>
        /// Removes the module and its objects.
        /// </summary>
        /// <param name="moduleID">The ID of the module to remove.</param>
        /// <returns>Whether it was removed successfully.</returns>
        protected virtual bool deleteModule(string moduleID) {
            if (!SubscribedModules.ContainsKey(moduleID)) {
                Debug.LogWarning("Failed to delete module, a module with that ID does not exist");
                return false;
            }

            Module module = SubscribedModules[moduleID];
            SubscribedModules.Remove(moduleID);

            Destroy(module.gameObject);

            return true;
        }

        /// <summary>
        /// Lets the ServerConnection know that the connection should be terminated.
        /// </summary>
        public void OnApplicationQuit() {
            ServerConnection.stopConnection();
        }
    }
}