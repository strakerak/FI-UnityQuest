using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class ConnectionMenu : MonoBehaviour
{
    /// <summary>
    /// Represents the singleton of this class.
    /// </summary>
    static ConnectionMenu INSTANCE;

    public TMP_InputField MessageText;
    //To Add new Conenction
    public TMP_InputField newIPText, newPortText, newNameText;
    
    // For Connection Status
    public Button ServerStatus;
    public Text connectionLabel;
    public TextMeshProUGUI Messagebox;

    //Prefabs
    public ServerDetails ServerListButtonPrefab;

    // UI Panels
    public GameObject UIPanel;
    public GameObject addNewPanel;
    public GameObject detailPanel;
    public GameObject MainPanel;

    public bool AutoConnect;
    public static bool connected = false;
    static int count = 0;
    
    private string DefaultIP = "fi3dtest.ddns.net";
    private string DefaultPort = "9000";

    private List<fi.Connection> connections;
    /// <summary>
    /// Changes the default IP text on the text box.
    /// </summary>
    public void OnValidate()
    {
        //IPText.text = DefaultIP;
        //PortText.text = DefaultPort;
    }

    /// <summary>
    /// Set the singleton and required objects
    /// </summary>
    public void Awake()
    {
        if (INSTANCE != null)
        {
            Destroy(gameObject);
        }
        else
        {
            INSTANCE = this;
            GameObject.DontDestroyOnLoad(gameObject);
        }
    }

    /// <summary>
    /// If auto connect is enabled, automatically try to connect to the server.
    /// </summary>
    public void Start()
    {
        App.LogMessage(string.Format("{0} | Background | Fetch Saved Server Lists", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));
        //Fetch All Server List Data From File
        connections = fi.Settings.getConnections();
        if (connections.Count == 0)
        {
            fi.Connection connection = new fi.Connection();
            connection.Name = "MRLCloud";
            connection.IP = DefaultIP;

            try
            {
                connection.Port = Int32.Parse(DefaultPort);
            }
            catch (Exception)
            {
                connection.Port = 9000;
                Debug.LogError("Failed to convert port to a number.");
            }

            fi.Settings.addConnection(connection);
        }

        connections = fi.Settings.getConnections();

        Debug.Log(string.Format("Saved Servers: {0}", connections.Count));
        SetMessage(string.Format("Saved Servers: {0}", connections.Count));

        count = connections.Count;
        //Populate The List on Screen 
        populatePanel();

        /*if (AutoConnect)
        {
            this.OnConnectClick();
        }*/
    }

    public void Update()
    {
        connections = fi.Settings.getConnections();
        if (connections.Count != count)
        {
            count = connections.Count;
            populatePanel();
        }
    }

    /// <summary>
    /// Populate UI Panel List With Connection Parameters.
    /// </summary>
    public void populatePanel()
    {
        Dictionary<string, GameObject> existing = new Dictionary<string, GameObject>();

        connections = fi.Settings.getConnections();
        Debug.Log(string.Format("Existing Servers: {0} ", UIPanel.transform.childCount));
        if (UIPanel.transform.childCount > 0)
        {
            Debug.Log("Get Existing Servers");
            
            foreach (Transform child in UIPanel.transform)
            {
                Button[] btns = child.GetComponentsInChildren<Button>();
                //Get Server Name
                string name = btns[1].GetComponentInChildren<TextMeshProUGUI>().text;
                existing.Add(name,child.gameObject);
            }
        }

        // To Remove deleted Server Detail from List
        if (existing.Count > connections.Count)
        {
            foreach (string keyVal in existing.Keys)
            {
                if (connections.Find(x => x.Name == keyVal) == null)
                {
                    Destroy(existing[keyVal]);
                }
            }
        }

        // To Add new Server Detail  in List
        foreach (fi.Connection connection in connections)
        {
            // Check if Object Already Exists Else Create new Object
            if (!existing.ContainsKey(connection.Name))
            {
                ServerDetails newserver = Instantiate(ServerListButtonPrefab, Vector3.zero, Quaternion.identity);
                newserver.ServerName = connection.Name;
                Button[] btn = newserver.GetComponentsInChildren<Button>();
                //btn[0] -> Delete || btn[1]-> List Details || btn[2]-> Connect to Server
                btn[1].GetComponentInChildren<TextMeshProUGUI>().text = connection.Name;
                newserver.transform.SetParent(UIPanel.transform);
            }
        }
    }

    /// <summary>
    /// Sets the given message in the message text box in settings menu screen.
    /// </summary>
    /// <param name="message">The message to show.</param>
    public static void SetMessage(string message)
    {
        try
        {
            INSTANCE.MessageText.text = message;
        }
        catch (Exception)
        {   
            Debug.LogWarning("MessageTextBox Inactive.");
        }    
    }

    /// <summary>
    /// Add New Connection Parameters.
    /// </summary>
    public void onAddNewClick()
    {
        App.LogMessage(string.Format("{0} | Navigation | Add New Server Details Button Click", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));
        showAddNew();
    }

    /// <summary>
    /// Sets the given message in the message text box.
    /// </summary>
    /// <param name="state">The current state of the connection.</param>
    public void onServerStateChange(fi.ServerConnection.ConnectionState state)
    {
        Debug.Log(string.Format("Server State: {0}", state));
        switch (state)
        {
            case fi.ServerConnection.ConnectionState.CONNECTED:
                //SetMessage(string.Format("Connected Successfully To {0}", fi.ServerConnection.Connection.Name));
                SetMessage(string.Format("Connected Successfully \n Name: {0} \n IP: {1} \n Port: {2}", fi.ServerConnection.Connection.Name, fi.ServerConnection.Connection.IP, fi.ServerConnection.Connection.Port));
                ServerStatus.gameObject.SetActive(true);
                connected = true;
                changeConnectText(fi.ServerConnection.Connection.Name, true);
                connectionLabel.text = string.Format("{0} \n {1} \n {2}",fi.ServerConnection.Connection.Name, fi.ServerConnection.Connection.IP, fi.ServerConnection.Connection.Port);
                Messagebox.text = string.Format("{0} \n {1} \n {2}", fi.ServerConnection.Connection.Name, fi.ServerConnection.Connection.IP, fi.ServerConnection.Connection.Port);
                break;
            case fi.ServerConnection.ConnectionState.CONNECTING:
                SetMessage("Attempting Connection...");
                ServerStatus.gameObject.SetActive(false);
                connected = false;
                connectionLabel.text = string.Format("Not Connected");
                Messagebox.text = string.Format("Not Connected");
                break;
            case fi.ServerConnection.ConnectionState.DISCONNECTED:
                SetMessage("Disconnected.");
                ServerStatus.gameObject.SetActive(false);
                connected = false;
                changeConnectText(fi.ServerConnection.Connection.Name, false);
                connectionLabel.text = string.Format("Not Connected");
                Messagebox.text = string.Format("Not Connected");
                break;
            case fi.ServerConnection.ConnectionState.ERROR_NO_SERVER_RESPONSE:
                SetMessage("Connection failed, no server response");
                ServerStatus.gameObject.SetActive(false);
                connected = false;
                changeConnectText(fi.ServerConnection.Connection.Name, false);
                connectionLabel.text = string.Format("Not Connected");
                Messagebox.text = string.Format("Not Connected");
                break;
            case fi.ServerConnection.ConnectionState.ERROR_UNKNOWN:
                SetMessage("Connection with Server was lost");
                ServerStatus.gameObject.SetActive(false);
                connected = false;
                changeConnectText(fi.ServerConnection.Connection.Name, false);
                connectionLabel.text = string.Format("Not Connected");
                Messagebox.text = string.Format("Not Connected");
                break;
        }

    }

    /// <summary>
    /// Change the Text of Connec Button According to Connection Status
    /// </summary>

    public void changeConnectText(string name, bool set)
    {
        if (UIPanel.transform.childCount > 0)
        {
            Debug.Log("Change Connection Status Button");

            foreach (Transform child in UIPanel.transform)
            {
                Button[] btns = child.GetComponentsInChildren<Button>();
                //Get Server Name
                if (btns[1].GetComponentInChildren<TextMeshProUGUI>().text == name)
                {
                    if (set)
                        child.GetChild(2).GetComponentInChildren<TextMeshProUGUI>().text = "Disconnect";
                    else
                        child.GetChild(2).GetComponentInChildren<TextMeshProUGUI>().text = "Connect";
                }
            }
        }
    }

    /// <summary>
    /// Populate Connection Details on the Connection Image Button 
    /// </summary>
    public void connectionIconClick()
    {
        if (connectionLabel.gameObject.activeSelf)
            connectionLabel.gameObject.SetActive(false);
        else
            connectionLabel.gameObject.SetActive(true);
    }

    // Add New Details Window Functions Start */ 

    /// <summary>
    /// Close Add New Connection Panel.
    /// </summary>
    public void onAddNewCloseClick()
    {
        App.LogMessage(string.Format("{0} | Navigation | Close Add New Server List Panel Click", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));
        showList();
    }

    /// <summary>
    /// Save Connection Parameters to File.
    /// </summary>
    public void onSaveClick()
    {
        Debug.Log(string.Format("{0} | UI | Save New Connection Parameters", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));
        //fi.App.serverLog(string.Format("{0} | Navigation | Save New Connection Parameters", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));
        App.LogMessage(string.Format("{0} | UI | Save New Connection Parameters", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));

        fi.Settings.removeConnection(newNameText.text);

        fi.Connection connection = new fi.Connection();
        connection.Name = newNameText.text;
        connection.IP = newIPText.text;

        try
        {
            connection.Port = Int32.Parse(newPortText.text);
        }
        catch (Exception)
        {
            connection.Port = 9000;
            Debug.LogError("Failed to convert port to a number.");
        }

        fi.Settings.addConnection(connection);

        App.LogMessage(string.Format("{0} | Background | Save New Connection Parameters", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));

        SetMessage(string.Format("Saved New Connection: Name={0} IP={1}, Port={2}", newNameText.text, newIPText.text, newPortText.text));
        count++;
        populatePanel();
    }

    // Add New Details Window Functions End */



    // View Details Window Functions Start */ 

    /// <summary>
    /// Close Details Panel.
    /// </summary>
    public void onDetailsCloseClick()
    {
        App.LogMessage(string.Format("{0} | Navigation | Close Server Details Panel", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));
        showList();
    }

    // View Details Window Functions End */


    // Common Display Functions Start */

    /// <summary>
    /// Display the Available Server List Panel
    /// </summary>
    public void showList()
    {
        if (!MainPanel.gameObject.activeSelf)
            MainPanel.gameObject.SetActive(true);

        if (detailPanel.gameObject.activeSelf)
            detailPanel.gameObject.SetActive(false);

        if (addNewPanel.gameObject.activeSelf)
            addNewPanel.gameObject.SetActive(false);
    }

    /// <summary>
    /// Display Add New Connection Panel
    /// </summary>
    public void showAddNew()
    {
        if (MainPanel.gameObject.activeSelf)
            MainPanel.gameObject.SetActive(false);

        if (detailPanel.gameObject.activeSelf)
            detailPanel.gameObject.SetActive(false);

        if (!addNewPanel.gameObject.activeSelf)
            addNewPanel.gameObject.SetActive(true);
    }

    // Common Display Functions End */
}
