using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace fi {
    /// <summary>
    /// Contains information about a server connection.
    /// </summary>
    [System.Serializable]
    public class Connection {
        /// <summary>
        /// A name for this connection.
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// The password of the server.
        /// </summary>
        public string Password { get; set; } = "admin";

        /// <summary>
        /// The IP of the connection.
        /// </summary>
        public string IP { get; set; } = "";

        /// <summary>
        /// The port of the connection.
        /// </summary>
        public int Port { get; set; } = 9000;
    }

    /// <summary>
    /// In charge of storing/loading app configuration.
    /// </summary>
    public class Settings : MonoBehaviour {
        /// <summary>
        /// The only instance of Settings.
        /// </summary>
        static Settings INSTANCE;

        /// <summary>
        /// The saved connections.
        /// </summary>
        List<Connection> Connections { get; set; } = new List<Connection>();

        /// <summary>
        /// The index of the connection that is used as default.
        /// </summary>
        private int DefaultConnectionIndex { get; set; } = 0;

        /// <summary>
        /// Create the Settings singleton and load pre-stored settings.
        /// </summary>
        void Awake() {
            if (INSTANCE != null) {
                Destroy(gameObject);
            } else {
                INSTANCE = this;
                GameObject.DontDestroyOnLoad(gameObject);
                loadSettings();
            }
        }

        /// <summary>
        /// Adds a connection to the list of managed connections.
        /// </summary>
        /// <param name="name">The name of the connection.</param>
        /// <param name="password">The password of the connection with the FI3D server.</param>
        /// <param name="IP">The IP of the connection.</param>
        /// <param name="port">The port of the connection.</param>
        /// <param name="setDefault">Whether to set this connection as default, defaults to true.</param>
        /// <returns>The created connection. Null if empty name or a name with an existing connection is given.</returns>
        static public Connection addConnection(
            string name, string password = "admin", string IP = "localhost", int port = 9000, bool setDefault = true) 
        {
            Connection con = new Connection();
            con.Name = name;
            con.IP = IP;
            con.Port = port;
            con.Password = password;

            if (addConnection(con, setDefault)) {
                return con;
            } else {
                return null;
            }
        }

        /// <summary>
        /// Adds the given connection to the stored connections.
        /// </summary>
        /// <param name="connection">The connection information.</param>
        /// <param name="setDefault">Whether to set this connection as default, defaults to true.</param>
        /// <returns>Whether the connection was added successfully.</returns>
        static public bool addConnection(Connection connection, bool setDefault = true) {
            if (connection == null) {
                return false;
            }

            // The connection must have a name.
            if (connection.Name.Equals("")) {
                return false;
            }

            // The name must not be taken.
            for (int i = 0; i < INSTANCE.Connections.Count; i++) {
                if (INSTANCE.Connections[i].Name.Equals(connection.Name)) {
                    return false;
                }
            }

            INSTANCE.Connections.Add(connection);
            if (setDefault) {
                INSTANCE.DefaultConnectionIndex = INSTANCE.Connections.Count - 1;
            }

            INSTANCE.saveSettings();
            return true;
        }

        /// <summary>
        /// Removes the connection that mathces the given name from the managed connections.
        /// </summary>
        /// <param name="name">The name of the connection to remove.</param>
        static public void removeConnection(string name) {
            // The connection must have a name.
            if (name.Equals("")) {
                return;
            }

            // Find the connection
            for (int i = 0; i < INSTANCE.Connections.Count; i++) {
                if (INSTANCE.Connections[i].Name.Equals(name)) {
                    // Update default index before removing the connection
                    if (i == INSTANCE.DefaultConnectionIndex) {
                        // Set to 0 if the removed connection was the default
                        INSTANCE.DefaultConnectionIndex = 0;
                    } else if (i < INSTANCE.DefaultConnectionIndex) {
                        // Decrease by 1 if the removed connection was before the default connection
                        INSTANCE.DefaultConnectionIndex -= 1;
                    }
                    INSTANCE.Connections.RemoveAt(i);
                    INSTANCE.saveSettings();
                    return;
                }
            }
        }

        /// <summary>
        /// Get a copy of the list of stored connections.
        /// </summary>
        /// <returns>The copied list of connections.</returns>
        static public List<Connection> getConnections() {
            List<Connection> cons = new List<Connection>();
            foreach (Connection con in INSTANCE.Connections) {
                Connection conCopy = new Connection();
                conCopy.Name = con.Name;
                conCopy.Password = con.Password;
                conCopy.IP = con.IP;
                conCopy.Port = con.Port;

                cons.Add(conCopy);
            }

            return cons;
        }

        /// <summary>
        /// Sets the connection with the given name as default, nothing happens
        /// if a connection with the given name is not found.
        /// </summary>
        /// <param name="name">The name of the connection.</param>
        static public void setDefaultConnection(string name) {
            if (name.Equals("")) {
                return;
            }

            for (int i = 0; i < INSTANCE.Connections.Count; i++) {
                if (name.Equals(INSTANCE.Connections[i].Name)) {
                    INSTANCE.DefaultConnectionIndex = i;
                    INSTANCE.saveSettings();
                    return;
                }
            }
        }

        /// <summary>
        /// Gets a copy of the default connection.
        /// </summary>
        /// <returns>The connection, null if there are no stored connections.</returns>
        static public Connection getDefaultConnection() {
            if (INSTANCE.Connections.Count == 0) {
                return null;
            }

            Connection con;

            // If the index is out of range, store the 1st connection
            if (INSTANCE.DefaultConnectionIndex < 0 || INSTANCE.DefaultConnectionIndex >= INSTANCE.Connections.Count) {
                INSTANCE.DefaultConnectionIndex = 0;
                con = INSTANCE.Connections[0];
            } else {
                con = INSTANCE.Connections[INSTANCE.DefaultConnectionIndex];
            }

            Connection conCopy = new Connection();
            conCopy.Name = con.Name;
            conCopy.Password = con.Password;
            conCopy.IP = con.IP;
            conCopy.Port = con.Port;

            return conCopy;
        }

        /// <summary>
        /// Loads the settings that were previously stored.
        /// </summary>
        void loadSettings() {
            string path = Application.persistentDataPath + "/config.dat";

            string configText = "";
            if (File.Exists(path)) {
                configText = File.ReadAllText(path);
            } else {
                Debug.Log("No settings file was found, creating one.");
                saveSettings();
                return;
            }

            SimpleJSON.JSONObject config;
            try {
                config = SimpleJSON.JSON.Parse(configText).AsObject;
            } catch (Exception exp) {
                Debug.LogWarning(string.Format("Failed to parse settings file: {0}. Recreating file.", exp.Message));
                saveSettings();
                return;
            }

            if (!config.IsNull) {
                SimpleJSON.JSONArray cons = config["Connections"].AsArray;
                for (int i = 0; i < cons.Count; i++) {
                    SimpleJSON.JSONObject conJson = cons[i].AsObject;

                    Connection con = new Connection();
                    con.Name = conJson["Name"];
                    con.IP = conJson["IP"];
                    con.Port = conJson["Port"].AsInt;
                    con.Password = conJson["Password"];

                    Connections.Add(con);
                }
            }

            DefaultConnectionIndex = config["DefaultConnectionIndex"].AsInt;
            if (DefaultConnectionIndex < 0 || DefaultConnectionIndex >= Connections.Count) {
                DefaultConnectionIndex = 0;
            }
        }

        /// <summary>
        /// Saves to disk the current settings, replacing old settings.
        /// </summary>
        void saveSettings() {
            string path = Application.persistentDataPath + "/config.dat";

            SimpleJSON.JSONArray cons = new SimpleJSON.JSONArray();
            foreach (Connection con in Connections) {
                SimpleJSON.JSONObject conJson = new SimpleJSON.JSONObject();
                conJson["Name"] = con.Name;
                conJson["IP"] = con.IP;
                conJson["Port"] = con.Port;
                conJson["Password"] = con.Password;

                cons.Add(conJson);
            }

            SimpleJSON.JSONObject config = new SimpleJSON.JSONObject();
            config["Connections"] = cons;
            config["DefaultConnectionIndex"] = DefaultConnectionIndex;

            try {
                byte[] bytes = System.Text.ASCIIEncoding.ASCII.GetBytes(config.ToString());
                File.WriteAllBytes(path, bytes);
            } catch (Exception exp) {
                Debug.LogError(string.Format("Failed to save settings: {0}", exp.Message));
            }
        }
    }
}