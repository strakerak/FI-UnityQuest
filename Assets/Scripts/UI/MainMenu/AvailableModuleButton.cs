using UnityEngine.UI;

public class AvailableModuleButton : fi.AvailableModuleAction
{
    /// <summary>
    /// Label used to display the name of the module.
    /// </summary>
    Text ModuleNameLabel;

    /// <summary>
    /// Override so that the label can be updated with the module name
    /// when it is set.
    /// </summary>
    public override string ModuleName
    {
        get {
            return base.ModuleName;
        } set {
            base.ModuleName = value;

            if (ModuleNameLabel != null)
            {
                ModuleNameLabel.text = ModuleName;
            }
        }
    }
}
