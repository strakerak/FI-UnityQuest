using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System;

public class ServerDetails : MonoBehaviour
{ 
    /// <summary>
    /// The interactable script used to update the Buttons click via script.
    /// </summary>
    
    public string ServerName { get; set; }
    
    /// <summary>
    /// Popup new window with ServerList Details IP, Port, Password.
    /// </summary>
    public void onDeleteClick()
    {
        Debug.Log(string.Format("{0} | UI | Delete Connection Parameters", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));
        //fi.App.serverLog(string.Format("{0} | UI | Delete Connection Parameters", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));
        App.LogMessage(string.Format("{0} | UI | Delete Connection Parameters", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));

        fi.Settings.removeConnection(ServerName);

        Debug.Log(string.Format("{0} | Background | Delete Connection Parameters", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));
        App.LogMessage(string.Format("{0} | Background | Delete Connection Parameters", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));

        ConnectionMenu.SetMessage(string.Format("Delete Connection: Name={0}", ServerName));
    }

    /// <summary>
    /// Popup new window with ServerList Details IP, Port, Password.
    /// </summary>
    public void onViewClick()
    {
        Debug.Log(string.Format("{0} | Navigation | View Connection Parameters", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));
        //fi.App.serverLog(string.Format("{0} | Navigation | View Connection Parameters", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));
        App.LogMessage(string.Format("{0} | Navigation | View Connection Parameters Details", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));

        List<fi.Connection> connections = fi.Settings.getConnections();

        fi.Connection connection = connections.Find(x => x.Name == ServerName);

        GameObject popupmessagebox = GameObject.Find("PopUpWindow").transform.Find("PopUpBox").gameObject;
        popupmessagebox.SetActive(true);
        popupmessagebox.GetComponentInChildren<TextMeshProUGUI>().text = string.Format("{0} IP: {1} Port: {2} "
            ,connection.Name, connection.IP, connection.Port);
    }

    /// <summary>
    /// Event handler for when the connect button is clicked.
    /// </summary>
    public void onConnectClick()
    {
        Debug.Log(string.Format("{0} | UI | Connect to Server Button Click", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));
        //fi.App.serverLog(string.Format("{0} | Navigation | Connect to Server Button Click", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));
        App.LogMessage(string.Format("{0} | UI | Connect to Server Button Click", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));

        Debug.Log(string.Format("{0} | To Server | Connect to Server", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));
        //fi.App.serverLog(string.Format("{0} | To Server | Connect to Server", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));
        App.LogMessage(string.Format("{0} | To Server | Connect to Server", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));

        //Debug.Log(this.gameObject.name);
        if (fi.ServerConnection.IsConnected)
        {
            fi.ServerConnection.stopConnection();

            //Button[] btn = this.GetComponentsInChildren<Button>();
            //btn[0] -> Delete || btn[1]-> List Details || btn[2]-> Connect to Server
            //btn[2].GetComponentInChildren<TextMeshProUGUI>().text = "Connect";
        }
        else
        {
            List<fi.Connection> connections;
            connections = fi.Settings.getConnections();

            fi.Connection connection = connections.Find(x => x.Name == ServerName);

            fi.ServerConnection.Connect(connection);

            //Button[] btn = this.GetComponentsInChildren<Button>();
            //btn[0] -> Delete || btn[1]-> List Details || btn[2]-> Connect to Server
            //btn[2].GetComponentInChildren<TextMeshProUGUI>().text = "Disconnect";
        }
    }
}