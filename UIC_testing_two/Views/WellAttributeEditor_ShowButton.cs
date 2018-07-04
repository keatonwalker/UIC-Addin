using ArcGIS.Desktop.Framework.Contracts;

namespace UIC_Edit_Workflow.Views
{
    /// <summary>
    /// Button implementation to show the DockPane.
    /// </summary>
    internal class WellAttributeEditor_ShowButton : Button
    {
        protected override void OnClick()
        {
            WellAttributeEditorViewModel.Show();
        }
    }
}