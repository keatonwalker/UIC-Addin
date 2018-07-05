using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;

namespace UIC_Edit_Workflow
{
    /// <summary>
    /// Button implementation to show the DockPane.
    /// </summary>
    internal class WorkFlowPane_ShowButton : Button
    {
        protected override async void OnClick()
        {
            var layer = await Module1.FindLayerAsync("uicfacility");
            if (layer == null)
            {
                FrameworkApplication.AddNotification(new Notification
                {
                    Message = "The facilities layer could not be found. Do you need to open a different project?"
                });

                return;
            }
            //WorkFlowPaneViewModel.subRowEvent();
            WorkFlowPaneViewModel.Show();
        }
    }
}
