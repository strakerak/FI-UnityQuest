using UnityEngine;
using System.IO;
using TMPro;
using System;
using UnityEngine.UI;

public class App : fi.App
{
    /// <summary>
    /// The menus which are used to activate/deaticvate them depending on user
    /// input.
    /// </summary>
    public GameObject ModuleListMenuObject;
    public GameObject ConnectionMenuObject;
    public GameObject ARSettingsMenuObject;
    public GameObject ModuleMenuObject;
    public GameObject SettingsMenuObject;
    //public Image ServerStatus;
    public GameObject NeuroDemoControllerPrefab;
    public GameObject CardiacDemoControllerPrefab;
    public GameObject DemosListMenuObject;

    //Pop Up Window For Log File Details Messages
    public GameObject LogPopUpWindow;

    /// <summary>
    /// Sets the application to run on background and sets-up the menus.
    /// </summary>
    protected override void Start()
    {
        base.Start();
        // Get the created main menu and nested under the canvas
        MainMenu.transform.SetParent(ModuleListMenuObject.transform);
        
        //ModuleListMenuObject.transform.position = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        MainMenu.transform.position = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        ModuleListMenuObject.transform.Find("Scroll View(Clone)").gameObject.SetActive(false);

        //Check Size of Log File
        string path = Application.persistentDataPath + "/log.txt";
        Debug.Log(string.Format("Log File: {0}", path));

        if(File.Exists(path))
        {
            FileInfo info = new FileInfo(path);
            if (info.Length >= 5000000) // 5MB
            {
                // Ask user to either push logs to server and clear log file
                // Or Clear Log File Without Sending Log
                GameObject messagebox = LogPopUpWindow.transform.Find("PopUpBox").gameObject;
                messagebox.SetActive(true);
                messagebox.GetComponentInChildren<TextMeshProUGUI>().text = string.Format("Log File Size Reached Limit: {0} MB. Either send all messages to server or clear log file without sending messages."
                     , info.Length/1000000);
            }
        }
    }

    /// <summary>
    /// Check for Updates
    /// </summary>
    /*protected override void Update()
    {
        base.Update();

        while (!base.ServerLogs.IsEmpty && ServerConnection.IsAuthenticated)
        {
            string log;
            ServerLogs.TryDequeue(out log);
            if (log != null)
            {
                ServerConnection.sendMessage(RequestMaker.makeServerLogRequest(log));
            }
        }
    }
    */

    /// <summary>
    /// View/Hide Settings Menu Sidebar  Panel
    /// </summary>
    public void onSettingsButton()
    {
        Debug.Log(string.Format("{0} | Navigation | Settings Menu Button Pressed", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));
        //fi.App.serverLog(string.Format("{0} | Navigation | Settings Menu Button Pressed", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));
        LogMessage(string.Format("{0} | Navigation | Settings Menu Button Pressed", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));

        if (SettingsMenuObject.activeSelf)
            SettingsMenuObject.SetActive(false);
        else
            SettingsMenuObject.SetActive(true);

        if (DemosListMenuObject.activeSelf)
            DemosListMenuObject.SetActive(false);

        if (ARSettingsMenuObject.activeSelf)
            ARSettingsMenuObject.SetActive(false);
        
        if (ModuleListMenuObject.transform.Find("Scroll View(Clone)") != null)
        {
            if (ModuleListMenuObject.transform.Find("Scroll View(Clone)").gameObject.activeSelf)
                ModuleListMenuObject.transform.Find("Scroll View(Clone)").gameObject.SetActive(false);
        }

        if (ConnectionMenuObject.activeSelf)
            ConnectionMenuObject.SetActive(false);
        
    }


    /// <summary>
    /// Show Demos List Menu
    /// </summary>
    public void onDemosListClick()
    {
        Debug.Log(string.Format("{0} | Navigation | Show/Hide Demo List Button Pressed", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));
        //fi.App.serverLog(string.Format("{0} | Navigation | Show Modules Button Pressed", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));
        LogMessage(string.Format("{0} | Navigation | Show/Hide Demo List Button Pressed", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));

        if (SettingsMenuObject.activeSelf)
            SettingsMenuObject.SetActive(false);

        if (DemosListMenuObject.activeSelf)
            DemosListMenuObject.SetActive(false);
        else
            DemosListMenuObject.SetActive(true);

        if (ARSettingsMenuObject.activeSelf)
            ARSettingsMenuObject.SetActive(false);
        
        if (ModuleListMenuObject.transform.Find("Scroll View(Clone)") != null)
        {
            if (ModuleListMenuObject.transform.Find("Scroll View(Clone)").gameObject.activeSelf)
                ModuleListMenuObject.transform.Find("Scroll View(Clone)").gameObject.SetActive(false);
        }

        if (ConnectionMenuObject.activeSelf)
            ConnectionMenuObject.SetActive(false);
    }


    /// <summary>
    /// Launch Neuro Demo Study
    /// </summary>
    public void onNeuroDemoClick()
    {
        GameObject demoStudy = Instantiate<GameObject>(NeuroDemoControllerPrefab);
        DemosListMenuObject.SetActive(false);   
    }

    /// <summary>
    /// Launch Cardiac Demo Study
    /// </summary>
    public void onCardiacDemoClick()
    {
        GameObject demoStudy = Instantiate<GameObject>(CardiacDemoControllerPrefab);
        DemosListMenuObject.SetActive(false);
    }

    /// <summary>
    /// View/Hide ServerConnection Menu Window
    /// </summary>
    public void onServerConnectionButton()
    {
        Debug.Log(string.Format("{0} | Navigation | Show/Hide Server Connection Menu Button Pressed", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));
        //fi.App.serverLog(string.Format("{0} | Navigation | Show/Hide Server Connection Button Pressed", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));
        LogMessage(string.Format("{0} | Navigation | Show/Hide Server Connection Menu Button Pressed", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));

        if (SettingsMenuObject.activeSelf)
            SettingsMenuObject.SetActive(false);

        if (DemosListMenuObject.activeSelf)
            DemosListMenuObject.SetActive(false);

        if (ARSettingsMenuObject.activeSelf)
            ARSettingsMenuObject.SetActive(false);

        if (ModuleListMenuObject.transform.Find("Scroll View(Clone)") != null)
        {
            if (ModuleListMenuObject.transform.Find("Scroll View(Clone)").gameObject.activeSelf)
                ModuleListMenuObject.transform.Find("Scroll View(Clone)").gameObject.SetActive(false);
        }

        if (ConnectionMenuObject.activeSelf)
            ConnectionMenuObject.SetActive(false);
        else
            ConnectionMenuObject.SetActive(true);

        //if (!ServerStatus.gameObject.activeSelf && ConnectionMenu.connected == true)
        //    ServerStatus.gameObject.SetActive(true);
    }

    /// <summary>
    /// View/Hide AR Settings Window
    /// </summary>
    public void onARSettingsButton()
    {
        Debug.Log(string.Format("{0} | Navigation | Show AR Settings Button Pressed", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));
        //fi.App.serverLog(string.Format("{0} | Navigation | Show Modules Button Pressed", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));
        LogMessage(string.Format("{0} | Navigation | Show AR Settings Button Pressed", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));

        if (SettingsMenuObject.activeSelf)
            SettingsMenuObject.SetActive(false);

        if (DemosListMenuObject.activeSelf)
            DemosListMenuObject.SetActive(false);

        if (ARSettingsMenuObject.activeSelf)
            ARSettingsMenuObject.SetActive(false);
        else
            ARSettingsMenuObject.SetActive(true);

        if (ModuleListMenuObject.transform.Find("Scroll View(Clone)") != null)
        {
            if (ModuleListMenuObject.transform.Find("Scroll View(Clone)").gameObject.activeSelf)
                ModuleListMenuObject.transform.Find("Scroll View(Clone)").gameObject.SetActive(false);
        }

        if (ConnectionMenuObject.activeSelf)
            ConnectionMenuObject.SetActive(false);
    }

    /// <summary>
    /// View Module List (Main) Menu Panel
    /// </summary>
    public void onShowModulesButton()
    {
        Debug.Log(string.Format("{0} | Navigation | Show Modules Button Pressed", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));
        //fi.App.serverLog(string.Format("{0} | Navigation | Show Modules Button Pressed", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));
        LogMessage(string.Format("{0} | Navigation | Show Modules Button Pressed", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));

        if (SettingsMenuObject.activeSelf)
            SettingsMenuObject.SetActive(false);

        if (DemosListMenuObject.activeSelf)
            DemosListMenuObject.SetActive(false);

        if (ARSettingsMenuObject.activeSelf)
            ARSettingsMenuObject.SetActive(false);

        if (ModuleListMenuObject.transform.Find("Scroll View(Clone)").gameObject.activeSelf)
            ModuleListMenuObject.transform.Find("Scroll View(Clone)").gameObject.SetActive(false);
        else
        {
            ModuleListMenuObject.transform.Find("Scroll View(Clone)").gameObject.SetActive(true);
            MainMenu.gameObject.SetActive(true);
        }

        if (ConnectionMenuObject.activeSelf)
            ConnectionMenuObject.SetActive(false);
    }

    /// <summary>
    /// Show Hide Bottom Menu Panel
    /// </summary>
    public void onMenuButton()
    {
        Debug.Log(string.Format("{0} | Navigation | Module Interactions Menu Button Pressed", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));
        //fi.App.serverLog(string.Format("{0} | Navigation | Module Interactions Menu Button Pressed", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));
        LogMessage(string.Format("{0} | Navigation | Module Interactions Menu Button Pressed", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));

        GameObject ScrollPanel = ModuleMenuObject.transform.Find("ScrollPanel").gameObject;
        Button btn = ModuleMenuObject.transform.Find("ModuleMenuAnchor").GetComponent<Button>();

        if (ScrollPanel.activeSelf)
        {
            ScrollPanel.SetActive(false);
            btn.transform.GetComponentInChildren<Text>().text = "Menu";
        }
        else
        {
            ScrollPanel.SetActive(true);
            btn.transform.GetComponentInChildren<Text>().text = "Hide";
        }
    }

    /// <summary>
    ///  Add messages to local Log File
    /// </summary>
    /// <param name="message"></param>
    public static void LogMessage(string  logMessage)
    {
        string path = Application.persistentDataPath + "/log.txt";

        if (File.Exists(path))
            File.AppendAllText(path, logMessage + Environment.NewLine);
        else
            File.WriteAllText(path, logMessage + Environment.NewLine);
    }

    /// <summary>
    /// Lof File Menu/Popup Window
    /// </summary>
    public void onLogMenuClick()
    {
        Debug.Log(string.Format("{0} | Navigation | View LogFile Menu Options", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));
        //fi.App.serverLog(string.Format("{0} | Navigation | View Connection Parameters", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));
        LogMessage(string.Format("{0} | Navigation | View LogFile Menu Options", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));

        string path = Application.persistentDataPath + "/log.txt";
        string message;

        if (File.Exists(path))
        {
            FileInfo info = new FileInfo(path);

            if(info.Length >= 1000000) // File Size in MB
                message = string.Format("Log File Size: {0} MB."
                 , info.Length / 1000000);
            else
                if (info.Length >= 1000) // File Size in KB
                message = string.Format("Log File Size: {0} KB."
                 , info.Length / 1000);
            else // File size in Bytes
                message = string.Format("Log File Size: {0} Bytes."
                 , info.Length);

            GameObject messagebox = LogPopUpWindow.transform.Find("PopUpBox").gameObject;
                messagebox.SetActive(true);
            messagebox.GetComponentInChildren<TextMeshProUGUI>().text = message;
        }
    }


    /// <summary>
    /// Send Local Log File to Server. 
    /// </summary>
    public void onSendLogClick()
    {
        Debug.Log(string.Format("{0} | Navigation | Send Log Button Pressed", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));
        LogMessage(string.Format("{0} | Navigation | Send Log Button Pressed", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));

        Debug.Log(string.Format("{0} | To Server | Sending Log", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));
        LogMessage(string.Format("{0} | To Server | Sending Log", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));

        Debug.Log(string.Format("{0} | Background | End of Log", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));
        LogMessage(string.Format("{0} | Background | End of Log", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));

        string path = Application.persistentDataPath + "/log.txt";

        if (File.Exists(path))
        {
            if (fi.ServerConnection.IsConnected)
            {
                string[] logText = File.ReadAllLines(path);

                foreach (string line in logText)
                    fi.App.serverLog(line);

                //Clear Device's log file.
                File.WriteAllText(path, string.Empty);
            }
            else
                Debug.Log("Not Connected to Server.");
        }
        else
        {
            Debug.Log("No Log messages Found.");
        }
        return;
    }

    /// <summary>
    /// Send Local Log File to Server. 
    /// </summary>
    public void onClearLogClick()
    {
        Debug.Log(string.Format("{0} | Navigation | Clear Log Button Pressed", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));
        LogMessage(string.Format("{0} | Navigation | Clear Log Button Pressed", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));

        Debug.Log(string.Format("{0} | Background | Clear Log", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));
        LogMessage(string.Format("{0} | Background | Clear Log", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));

        string path = Application.persistentDataPath + "/log.txt";

        if (File.Exists(path))
        {
            //Clear Device's log file.
            File.WriteAllText(path, string.Empty);
        }
        else
        {
            Debug.Log("No Log messages Found.");
            return;
        }
    }

    /*
    /// <summary>
    /// Lof File Menu/Popup Window
    /// </summary>
    public void onInfoMenuClick()
    {
        Debug.Log(string.Format("{0} | Navigation | View App Information", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));
        //fi.App.serverLog(string.Format("{0} | Navigation | View Connection Parameters", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));
        LogMessage(string.Format("{0} | Navigation | View App Information", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));

        GameObject infobox = InfoPopUpWindow.transform.Find("PopUpBox").gameObject;
        infobox.SetActive(true); 
    }
    */

    /// <summary>
    /// Quit Application Only For Android 
    /// </summary>
    /*public void onQuitClick()
    {
        Debug.Log(string.Format("{0} | Navigation | Quit Application Pressed", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));
        LogMessage(string.Format("{0} | Navigation | Quit Application Pressed", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));

        Application.Quit();
    }*/

    /// <summary>
    /// Creates a module and its objects.
    /// </summary>
    /// <param name="moduleID">The ID of the module to create.</param>
    /// <returns>Whether the module was created successfully.</returns>
    protected override bool createModule(string moduleID)
    {
        Debug.Log(string.Format("{0} | UI | Subscribe to Module", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));
        //fi.App.serverLog(string.Format("{0} | Navigation | Subscribe to Module", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));
        LogMessage(string.Format("{0} | UI | Subscribe to Module", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));

        if (SubscribedModules.Count>0)
        {
            string mod = "";
            foreach(string id in SubscribedModules.Keys)
            {
                mod = id;
            }
            Debug.Log("Removing All Active Study");

            //Remove Interactions
            GameObject ScrollPanel = GameObject.Find("ModulesMenuUI").transform.Find("ScrollPanel").gameObject;
            GameObject Content = ScrollPanel.transform.Find("Viewport").transform.Find("Content").gameObject;
            GameObject btn = ModuleMenuObject.transform.Find("ModuleMenuAnchor").gameObject;

            // Remove From ScrollView
            Debug.Log(string.Format("{0} | Background | Deleting {1} Interactions", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now), Content.transform.childCount));
            LogMessage(string.Format("{0} | Background | Deleting Interactions", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));
            
            foreach (Transform child in Content.transform)
            {
                Debug.Log("Deleting " + child.name);
                Destroy(child.gameObject);
            }

            //Hide Bottom Panels
            //GameObject.Find("ModulesMenuUI").transform.Find("SettingsPanel").gameObject.SetActive(false);
            ScrollPanel.SetActive(false);
            btn.SetActive(false);

            // Call UnsubscribeToModule
            unsubscribeToModule(mod);
        }

        if (GameObject.Find("Scene(Clone)") != null)
        {
            GameObject study = GameObject.Find("Scene(Clone)").transform.parent.gameObject;
            //ClearAll Active Scenes
            Debug.Log(string.Format("Found Active Study: {0}", study.name));
            if (study.name == "DemoController(Clone)")
            {
                Debug.Log("Cleanup Active Demo");
                Cleanup.cleanupStudy();
            }
        }

        Debug.Log(string.Format("{0} | To Server | Subscribe to Module", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));
        LogMessage(string.Format("{0} | To Server | Subscribe to Module", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));

        ARSceneHandler.Reset();
        return base.createModule(moduleID);
    }
}