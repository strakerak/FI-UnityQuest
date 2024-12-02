using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class MainMenu : fi.MainMenu
{
    /// <summary>
    /// The game objects that identify where the available and active module lists should be appended to.
    /// </summary>

    /// <summary>
    /// UI Panel where Module List is Added
    /// </summary>
    public GameObject ContentPanel;
   
    /// <summary>
    /// The prefab of the button used to represent an available module.
    /// </summary>
    public AvailableModuleButton AvailableModulePrefab;

    /// <summary>
    /// The prefab of the button used to represent an active module.
    /// </summary>
    public ActiveModuleButton ActiveModulePrefab;

    /* New Code Start */
    /// <summary>
    /// Prefab for Study Button 
    /// </summary>
    public GameObject studyPrefab;

    /// <summary>
    /// The list of module buttons that are currently available.
    /// </summary>
    List<AvailableModuleButton> AvailableModules = new List<AvailableModuleButton>();

    /// <summary>
    /// The list of module buttons that are currently active.
    /// </summary>
    List<ActiveModuleButton> ActiveModules = new List<ActiveModuleButton>();

    /// <summary>
    /// Keep copy of Active and Available Modules
    /// </summary>
    List<fi.AvailableModule> availableModuleList = new List<fi.AvailableModule>();
    List<fi.ActiveModule> activeModuleList = new List<fi.ActiveModule>();

    public override void updateAvailableModuleList(fi.AvailableModule[] modules)
    {
        //Empty Current List
        availableModuleList.Clear();

        // Add the new buttons
        for (int i = 0; i < modules.Length; i++)
        {
            availableModuleList.Add(modules[i]);
        }
    }

    public override void updateActiveModuleList(fi.ActiveModule[] modules)
    {
        //Empty Current List
        activeModuleList.Clear();

        if (modules == null)
        {
            return;
        }
        
        // Add new buttons
        for (int i = 0; i < modules.Length; i++)
        {
            activeModuleList.Add(modules[i]);
        }

        // Create Buttons and Add on UI
        updateList();
    }

    /// <summary>
    /// Function to Create Buttons using Prefabs and Add to UI Panel.
    /// </summary>
    public void updateList()
    {
        App.LogMessage(string.Format("{0} | Background | Updating Study Lists", TimeZoneInfo.ConvertTimeToUtc(DateTime.Now)));

        /*** Add Active Modules Button ***/
        // Remove current buttons
        for (int i = ActiveModules.Count - 1; i >= 0; i--)
        {
            ActiveModuleButton activeModule = ActiveModules[i];
            ActiveModules.RemoveAt(i);

            Destroy(activeModule.gameObject);
        }

        // Add new buttons
        for (int i = 0; i < activeModuleList.Count; i++)
        {
            Debug.Log("Adding Active Module Buttons");
            fi.ActiveModule module = activeModuleList[i];

            ActiveModuleButton newActiveModule = Instantiate(ActiveModulePrefab, Vector3.zero, Quaternion.identity);
            newActiveModule.ModuleName = module.Name;
            newActiveModule.ModuleID = module.ID;

            newActiveModule.transform.SetParent(ContentPanel.transform, false);

            Button[] childbtn = newActiveModule.GetComponentsInChildren<Button>();
            childbtn[0].GetComponentInChildren<Text>().text = module.ID;

            ActiveModules.Add(newActiveModule);

        }

        /*** Add Available Modules Button ***/
        // Remove current buttons
        for (int i = AvailableModules.Count - 1; i >= 0; i--)
        {
            AvailableModuleButton availableModule = AvailableModules[i];
            AvailableModules.RemoveAt(i);

            Destroy(availableModule.gameObject);
        }

        // Add the new buttons
        for (int i = 0; i < availableModuleList.Count; i++)
        {
            Debug.Log("Adding Available Module Buttons");

            //Only Create Module Buttons That are not Active 
            fi.AvailableModule module = availableModuleList[i];
            if (ActiveModules.Find(x => x.ModuleName == module.Name) == null)
            {
                AvailableModuleButton newAvailableModule = Instantiate(AvailableModulePrefab, Vector3.zero, Quaternion.identity);
                newAvailableModule.ModuleName = module.Name;
                newAvailableModule.ModuleAcronym = module.Acronym;
                newAvailableModule.GetComponentInChildren<Text>().text = module.Name;
                newAvailableModule.transform.SetParent(ContentPanel.transform, false);
                AvailableModules.Add(newAvailableModule);

            }
        }
    }
}

