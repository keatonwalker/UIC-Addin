using ArcGIS.Desktop.Framework.Contracts;

namespace UIC_Edit_Workflow
{
    /// <summary>
    /// Button implementation to show the DockPane.
    /// </summary>
    internal class AttributeEditor_ShowButton : Button
    {
        protected override void OnClick()
        {
            FacilityAttributeEditorViewModel.Show();
        }
    }
}