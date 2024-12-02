using UnityEngine;

public class ActiveModuleButton : fi.ActiveModuleAction
{
    /// <summary>
    /// Override so that the label can be updated with the module ID
    /// when it is set.
    /// </summary>
    public override string ModuleID
    {
        get
        {
            return base.ModuleID;
        }
        set
        {
            base.ModuleID = value;
        }
    }

    public void unsubscribeClick()
    {
        onUnsubscribeFromModuleClick();
    }
}