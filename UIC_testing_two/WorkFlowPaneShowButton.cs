using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using Serilog;

namespace UIC_Edit_Workflow {
    /// <summary>
    ///     Button implementation to show the DockPane.
    /// </summary>
    internal class WorkFlowPaneShowButton : Button {
        protected override async void OnClick() {
            var layer = await UicWorkflowModule.FindLayerAsync("uicfacility");
            if (layer == null) {
                Log.Warning("facility layer not in map when button clicked");

                FrameworkApplication.AddNotification(new Notification {
                    Message = "The facilities layer could not be found. Do you need to open a different project?"
                });

                return;
            }

            WorkFlowPaneViewModel.Show();
        }
    }
}
